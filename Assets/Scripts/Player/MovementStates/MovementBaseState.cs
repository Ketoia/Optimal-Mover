using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementBaseState
{
    public abstract void EnterState(MovementStateManager item);
    public abstract void UpdateState(MovementStateManager item);
    public abstract void FixedUpdateState(MovementStateManager item);
    public abstract void ExitState(MovementStateManager item);
    public abstract void OnCollisionEnter(MovementStateManager item, Collider other);
    public abstract void OnCollisionExit(MovementStateManager item, Collider other);
}
