using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody rb;
    Vector3 startDir;
    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        startDir = transform.up;
       // rb.rotation *= Quaternion.Euler(-90, 0, 0);
    }

    public float speed = 12f;
    public int distance = 5;
    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.forward * z * speed;
        Debug.DrawRay(transform.position, transform.up * 10f, Color.blue);
        Debug.DrawRay(transform.position, transform.right * 10f, Color.red);
        //Quaternion rotation = Quaternion.Euler(x * transform.up * speed * Time.fixedDeltaTime);
        //Quaternion rotationz = Quaternion.Euler(z * transform.right * speed * Time.fixedDeltaTime);
        //Quaternion rot = Quaternion.AngleAxis(x * speed * Time.fixedDeltaTime, -transform.up);
        //rb.AddForce(move * speed * Time.deltaTime);
        //transform.localRotation *= rot;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(x,0, z * speed));

    }
}
