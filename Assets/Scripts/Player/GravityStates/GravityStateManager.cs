using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Tilemaps;

public class GravityStateManager : MonoBehaviour
{
    public GravityBaseState currentState;
    public GravityIdleState idleState = new GravityIdleState();

    [SerializeField] private int gravity = 12;
    [SerializeField] private GameObject player;
    [SerializeField] private int pointsCount;
    [SerializeField] private float radius;

    private Rigidbody rb;

    public GameObject Player { get { return player; } }
    public int Gravity { get { return gravity; } }
    public Rigidbody Rb { get { return rb; } }

    void Start()
    {
        rb = Player.GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        currentState = idleState;

        currentState.EnterState(this);
    }    

    void Update()
    {
        currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
    }
    public void SwitchState(GravityBaseState state)
    {
        state.ExitState(this);

        currentState = state;
        state.EnterState(this);

    }

    public Vector3 FindNearestPoint()
    {
        Vector3 pos = transform.position;
        pos.z = 0;
        pos = pos.normalized * radius;

        return pos;
    }
}
