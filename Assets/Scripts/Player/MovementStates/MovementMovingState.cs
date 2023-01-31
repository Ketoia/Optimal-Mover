using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementMovingState : MovementBaseState
{
    private Rigidbody rb;
    private Vector3 move;
    //private Quaternion rotation = Quaternion.identity;
    //private Quaternion gravRotation = Quaternion.identity;
    //private Quaternion gravRotation2 = Quaternion.identity;

    private float angleX;
    private float angleZ;

    //public Vector3 smoothNormal;
    public override void EnterState(MovementStateManager item)
    {
        rb = item.Rb;
    }
    public override void UpdateState(MovementStateManager item)
    {
        if (item.AxisX == 0 && item.AxisZ == 0 && rb.velocity.x == 0 && rb.velocity.z == 0)
        {
            item.SwitchState(item.idleState);
        }
        else
        {
            float offset = 0.1f;
            move =  item.transform.forward * item.AxisZ;
            //rotation = Quaternion.Euler(item.AxisX * item.transform.up * item.RotationSpeed * Time.fixedDeltaTime);
            Ray ray = new Ray(item.transform.position, -item.transform.up);

            Ray ray2 = new Ray(item.transform.position + item.transform.right * 0.5f, -item.transform.up);
            Ray ray3 = new Ray(item.transform.position - item.transform.right * 0.5f, -item.transform.up);

            Ray ray4 = new Ray(item.transform.position + item.transform.forward * 0.5f, -item.transform.up);
            Ray ray5 = new Ray(item.transform.position - item.transform.forward * 0.5f, -item.transform.up);
            Debug.DrawRay(item.transform.position, -item.transform.up * 10f, Color.red);

            RaycastHit hit;
            RaycastHit hit2;
            RaycastHit hit3;

            RaycastHit hit4;
            RaycastHit hit5;

            if (Physics.Raycast(ray, out hit, 5f) && Physics.Raycast(ray2, out hit2, 5f) && Physics.Raycast(ray3, out hit3, 5f))
            {
                float distance2 = hit2.distance;
                float distance3 = hit3.distance;
                if (distance2 > distance3 + offset)
                {
                    angleX = Mathf.Atan(distance2 - distance3) * Mathf.Rad2Deg * 0.5f;
                }
                else if (distance2 < distance3 - offset)
                {
                    angleX = -Mathf.Atan(distance3 - distance2) * Mathf.Rad2Deg * 0.5f;
                }
                else
                    angleX = 0;
            }
            else
                angleX = 0;
            if (Physics.Raycast(ray, out hit) && Physics.Raycast(ray4, out hit4, 5f) && Physics.Raycast(ray5, out hit5, 5f))
            {
                float distance4 = hit4.distance;
                float distance5 = hit5.distance;
                if (distance4 > distance5 + offset)
                {
                    angleZ = Mathf.Atan2(distance4 - distance5, 1f) * Mathf.Rad2Deg * 0.5f;
                }
                else if (distance4 < distance5 - offset)
                {
                    angleZ = -Mathf.Atan2(distance5 - distance4, 1f) * Mathf.Rad2Deg * 0.5f;
                }
                else
                    angleZ = 0;
            }
            else
                angleZ = 0;
        }

    }

    public override void FixedUpdateState(MovementStateManager item)
    {
        rb.AddForce(move * item.Speed, ForceMode.Force);

        Vector3 angle = new Vector3(angleZ /** item.GravityRotationSpeed*/, item.AxisX /** item.RotationSpeed*/, -angleX /** item.GravityRotationSpeed*/);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(angle));
        //rb.rotation = Quaternion.Lerp(rb.rotation, rb.rotation * Quaternion.Euler(angle),0.5f);
    }

    public override void ExitState(MovementStateManager item)
    {

    }

    public override void OnCollisionEnter(MovementStateManager item, Collider other)
    {
        if (other.transform.CompareTag("Door"))
        {
            Debug.Log("Enter");
        }
    }

    public override void OnCollisionExit(MovementStateManager item, Collider other)
    {
        if (other.transform.CompareTag("Door"))
        {
            Debug.Log("Exit");
        }
    }
}
