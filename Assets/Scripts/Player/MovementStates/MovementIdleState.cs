using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementIdleState : MovementBaseState
{
    public float horizontal;
    public float vertical;
    public float speed = 10f;

    public override void EnterState(MovementStateManager item)
    {
    }
    public override void UpdateState(MovementStateManager item)
    {
        if (item.AxisX != 0 || item.AxisZ != 0)
        {
            item.SwitchState(item.movingState);
        }
    }

    public override void FixedUpdateState(MovementStateManager item)
    {

    }
    public override void ExitState(MovementStateManager item)
    {

    }

    public override void OnCollisionEnter(MovementStateManager item, Collider other)
    {

    }
    public override void OnCollisionExit(MovementStateManager item, Collider other)
    {

    }
}
