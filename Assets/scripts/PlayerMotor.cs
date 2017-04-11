using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;


    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //runs every physics iteration
    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    private void PerformMovement() //moves player
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    private void PerformRotation() //rotates player and camera
    {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
            if (cam != null)
            {
            cam.transform.Rotate(-cameraRotation);
            }
    }

    ///////////PUBLIC MEMEBER FUNCTIONS BELOW
    public void Move (Vector3 _velocity) //public member function for setting velocity
    {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _rotation) //public member function for setting rotation
    {
        rotation = _rotation;
    }

    public void RotateCamera(Vector3 _cameraRotation) //public member function for setting cameraRotation
    {
        cameraRotation = _cameraRotation; 
    }




}
