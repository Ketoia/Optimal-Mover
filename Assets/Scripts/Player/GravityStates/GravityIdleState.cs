using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityIdleState : GravityBaseState
{
    private Vector3 gravityVector;

    public override void EnterState(GravityStateManager item)
    {
        //listCount = item.WorldsPoints.Count;
    }
    public override void UpdateState(GravityStateManager item)
    {
        //gravityVector = item.FindNearestPoint() - item.Player.transform.position;
        gravityVector = item.transform.position - item.FindNearestPoint();
        gravityVector = -gravityVector.normalized;
        Debug.DrawLine(gravityVector + item.transform.position, item.transform.position, Color.blue);
    }

    public override void FixedUpdateState(GravityStateManager item)
    {
        item.Rb.AddForce(gravityVector * item.Gravity);
    }
    public override void ExitState(GravityStateManager item)
    {

    }
}
