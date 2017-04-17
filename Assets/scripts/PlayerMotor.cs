using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {
    

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 thrusterForce = Vector3.zero;

    [Header("Camera settings:")]
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float cameraRotationLimit = 85f;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;

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

        if(thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    private void PerformRotation() //rotates player and camera
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation)); //apply player rotation
        if (cam != null)
        {
            //set rotation and clamp it
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
            //apply camera rotation
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
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

    public void RotateCamera(float _cameraRotationX) //public member function for setting cameraRotation
    {
        cameraRotationX = _cameraRotationX; 
    }

    public void ApplyThruster(Vector3 _thrusterForce) //public member function for setting thrusterForce
    {
        thrusterForce = _thrusterForce;
    }

}
