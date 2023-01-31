using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float yRotation = 0;
    [SerializeField] private float test = 0;

    void Update()
    {
        yRotation += Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        Quaternion rotation = Quaternion.identity;
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            Vector3 pasta = hit.normal;
            //pasta.z = 0;
            rotation = Quaternion.LookRotation(pasta);
            rotation = rotation * /*Quaternion.AngleAxis(yRotation, hit.normal);*/ Quaternion.Euler(0, 0, test);
            transform.rotation = rotation;
        }
    }
}
