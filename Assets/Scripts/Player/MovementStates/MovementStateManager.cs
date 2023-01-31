using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Tilemaps;

public class MovementStateManager : MonoBehaviour
{
    public MovementBaseState currentState;
    public MovementIdleState idleState = new MovementIdleState();
    public MovementMovingState movingState = new MovementMovingState();

    [SerializeField] private float speed = 12;
    [SerializeField] private float rotationSpeed = 12;
    [SerializeField] private float gravityRotationSpeed = 12;
    [SerializeField] private Transform testPlayer;

    private Rigidbody rb;
    private float axisX;
    private float axisZ;

    public Transform TestPlayer { get { return testPlayer; } set { testPlayer = value;} }
    public float Speed { get { return speed; } }
    public float AxisX { get { return axisX; } }
    public float AxisZ { get { return axisZ; } }
    public Rigidbody Rb { get { return rb; } }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        currentState = idleState;

        currentState.EnterState(this);

        TestPlayer.position = Rb.transform.position;
        TestPlayer.localRotation = Rb.transform.localRotation;
    }
    

    void Update()
    {
        axisX = Input.GetAxisRaw("Horizontal");
        axisZ = Input.GetAxisRaw("Vertical");
        currentState.UpdateState(this);
        Debug.Log(currentState);


        TestPlayer.position = Vector3.Lerp(TestPlayer.position, rb.transform.position, 0.1f);
        TestPlayer.rotation = Quaternion.Lerp(TestPlayer.rotation, rb.transform.rotation, 0.1f * Quaternion.Dot(TestPlayer.rotation, rb.transform.rotation));
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
    }

    public void SwitchState(MovementBaseState state)
    {
        state.ExitState(this);

        currentState = state;
        state.EnterState(this);

    }

    private void OnTriggerEnter(Collider other)
    {
        currentState.OnCollisionEnter(this, other);
    }
    private void OnTriggerExit(Collider other)
    {
        currentState.OnCollisionExit(this, other);
    }

}
