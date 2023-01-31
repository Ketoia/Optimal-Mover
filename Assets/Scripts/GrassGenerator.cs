using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GrassGenerator : MonoBehaviour
{
    public float offset;
    public Mesh grassMesh;
    public Material grassMaterial;
    public GrassMask grassMask;
    public GameObject TerrainGameObject;
    public GameObject playerGameobject;

    private int grassMaskSize = 1024;
    private int numOfInstances, numThreadGroups, numVoteThreadGroups, numGroupScanThreadGroups;
    private float radius;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer instancePropertiesBuffer, instancePropertiesOutBuffer, positionUVBuffer, voteBuffer, scanBuffer, groupSumArrayBuffer, scannedGroupSumBuffer;

    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    private Vector2[] instanceUVPosition;
    private InstanceProperties[] instanceProperties;

    void Start()
    {
        Init();

        SetInstancesPropertiesData();
        SetInstancesUVData();
        SetArgsForInstancing();
    }

    void Update()
    {
        //if (Input.GetKey(KeyCode.Space))
            UpdateGrassBuffers();
    }

    private void OnDestroy()
    {
        if (instancePropertiesBuffer != null)
            instancePropertiesBuffer.Release();
        instancePropertiesBuffer = null;

        if (argsBuffer != null)
            argsBuffer.Release();
        argsBuffer = null;

    }

    private void Init()
    {
        grassMask = new GrassMask(grassMaskSize, grassMaskSize);
        radius = TerrainGameObject.transform.lossyScale.x * (1.0f / 3.0f);
        numOfInstances = TerrainGameObject.GetComponentInChildren<MeshFilter>().mesh.vertices.Length;

        //set up nice numbers
        numVoteThreadGroups = Mathf.CeilToInt(numOfInstances / 128.0f);
        numGroupScanThreadGroups = Mathf.CeilToInt(numOfInstances / 1024.0f);
        numThreadGroups = Mathf.CeilToInt(numOfInstances / 128.0f);

        if (numThreadGroups > 128)
        {
            int powerOfTwo = 128;
            while (powerOfTwo < numThreadGroups)
                powerOfTwo *= 2;

            numThreadGroups = powerOfTwo;
        }
        else
        {
            while (128 % numThreadGroups != 0)
                numThreadGroups++;
        }

        voteBuffer = new ComputeBuffer(numOfInstances, 4);
        scanBuffer = new ComputeBuffer(numOfInstances, 4);
        groupSumArrayBuffer = new ComputeBuffer(numThreadGroups, 4);
        scannedGroupSumBuffer = new ComputeBuffer(numThreadGroups, 4);
        instancePropertiesOutBuffer = new ComputeBuffer(numOfInstances, sizeof(float) * 6);
        instancePropertiesBuffer = new ComputeBuffer(numOfInstances, sizeof(float) * 6);

        ComputeShader computeShader = grassMask.grassCutoutComputeShader;

        grassMaterial.SetBuffer("_InstanceProperties", instancePropertiesOutBuffer);

        //Vote
        computeShader.SetBuffer(0, "_VoteBuffer", voteBuffer);

        // Scan Instances
        computeShader.SetBuffer(2, "_VoteBuffer", voteBuffer);
        computeShader.SetBuffer(2, "_ScanBuffer", scanBuffer);
        computeShader.SetBuffer(2, "_GroupSumArray", groupSumArrayBuffer);

        // Scan Groups
        computeShader.SetInt("_NumOfGroups", numThreadGroups);
        computeShader.SetBuffer(3, "_GroupSumArrayIn", groupSumArrayBuffer);
        computeShader.SetBuffer(3, "_GroupSumArrayOut", scannedGroupSumBuffer);

        // Compact
        computeShader.SetBuffer(4, "_VoteBuffer", voteBuffer);
        computeShader.SetBuffer(4, "_ScanBuffer", scanBuffer);
        computeShader.SetBuffer(4, "instancePropertiesOutput", instancePropertiesOutBuffer);
        computeShader.SetBuffer(4, "_GroupSumArray", scannedGroupSumBuffer);
    }

    private void SetInstancesPropertiesData()
    {
        Mesh terrainMesh = TerrainGameObject.GetComponentInChildren<MeshFilter>().mesh;
        Vector3[] plainVertices = terrainMesh.vertices;

        instanceProperties = new InstanceProperties[numOfInstances];
        instanceUVPosition = new Vector2[numOfInstances];

        for (int i = 0; i < numOfInstances; i++)
        {
            float offset = this.offset;
            Vector3 pos = TerrainGameObject.transform.TransformPoint(plainVertices[i]);
            Vector3 randomisedPos = new Vector3(UnityEngine.Random.Range(-offset, offset), UnityEngine.Random.Range(-offset, offset), UnityEngine.Random.Range(-offset, offset));
            Vector3 newPos = pos + randomisedPos;
            Vector3 nearestPoint = FindNearestPoint(pos);

            Vector3 newPosOnTorus = (newPos - nearestPoint).normalized * radius / 2 + nearestPoint;

            instanceProperties[i].position = newPosOnTorus;
            instanceProperties[i].direction = (newPosOnTorus - nearestPoint).normalized;
        }

        instancePropertiesBuffer.SetData(instanceProperties);

        ComputeShader computeShader = grassMask.grassCutoutComputeShader;

        computeShader.SetBuffer(0, "instanceProperties", instancePropertiesBuffer);
        computeShader.SetBuffer(computeShader.FindKernel("Compact"), "instanceProperties", instancePropertiesBuffer);
    }

    private void SetInstancesUVData() //Be sure to make it after setting instance properties
    {
        positionUVBuffer = new ComputeBuffer(numOfInstances, sizeof(float) * 2);

        for (int i = 0; i < numOfInstances; i++)
        {
            var instance = instanceProperties[i];
            if (Physics.Raycast(instance.position + instance.direction, -instance.direction, out RaycastHit hitInfo, 100, LayerMask.GetMask("Ground")))
            {
                instanceUVPosition[i] = hitInfo.textureCoord;
            }
        }

        positionUVBuffer.SetData(instanceUVPosition);
        grassMask.grassCutoutComputeShader.SetBuffer(0, "_InstanceUVPosition", positionUVBuffer);
    }

    private void SetArgsForInstancing() //Be sure to make it after setting instance properties 
    {
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

        if (grassMesh != null)
        {
            args[0] = (uint)grassMesh.GetIndexCount(0);
            args[1] = (uint)instanceProperties.Length;
            args[2] = (uint)grassMesh.GetIndexStart(0);
            args[3] = (uint)grassMesh.GetBaseVertex(0);
        }
        else
        {
            args[0] = args[1] = args[2] = args[3] = 0;
        }

        argsBuffer.SetData(args);
        grassMask.grassCutoutComputeShader.SetBuffer(0, "_ArgsBuffer", argsBuffer);
        grassMask.grassCutoutComputeShader.SetBuffer(4, "_ArgsBuffer", argsBuffer);

    }

    private Vector3 FindNearestPoint(Vector3 position)
    {
        Vector3 pos = position;
        pos.z = 0;
        pos = pos.normalized * radius;

        return pos;
    }

    private void UpdateGrassBuffers()
    {
        CalculateCameraPositionOnUVPos();

        //Reset args
        argsBuffer.SetData(args);

        //Calculate everything
        ComputeShader computeShader = grassMask.grassCutoutComputeShader;
        computeShader.Dispatch(0, numVoteThreadGroups, 1, 1);
        computeShader.Dispatch(2, numThreadGroups, 1, 1);
        computeShader.Dispatch(3, numGroupScanThreadGroups, 1, 1);
        computeShader.Dispatch(4, numThreadGroups, 1, 1);

        Graphics.DrawMeshInstancedIndirect(grassMesh, 0, grassMaterial, new Bounds(Vector3.zero, Vector3.one * 100), argsBuffer);
    }

    private void CalculateCameraPositionOnUVPos()
    {
        Vector3 playerPosition = playerGameobject.transform.position;// UnityEditor.SceneView.lastActiveSceneView.camera.transform.position;

        Vector3 nearestPoint = FindNearestPoint(playerPosition);
        Vector3 direction = (playerPosition - nearestPoint).normalized;

        if (Physics.Raycast(playerPosition + direction, -direction, out RaycastHit hitInfo, 100, LayerMask.GetMask("Ground")))
        {
            grassMask.UpdatePlayerPosition(hitInfo.textureCoord);
        }
    }

    [Serializable]
    public struct InstanceProperties
    {
        public Vector3 position;
        public Vector3 direction;
    }
}