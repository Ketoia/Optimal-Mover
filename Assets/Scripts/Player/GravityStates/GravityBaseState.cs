using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GravityBaseState
{
    public abstract void EnterState(GravityStateManager item);
    public abstract void UpdateState(GravityStateManager item);
    public abstract void FixedUpdateState(GravityStateManager item);
    public abstract void ExitState(GravityStateManager item);
}
