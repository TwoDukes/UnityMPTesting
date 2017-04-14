using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {
    [Header("Movement Settings:")]
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;
    [SerializeField]
    private float thrusterForce = 1000f;

    [Header("Spring Settings:")]
    [SerializeField]
    private float jointSpring;
    [SerializeField]
    private float jointMaxForce;

    //Component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        setJointSettings(jointSpring); //sets initial configurable joint values
    }

    private void Update()
    {
        CalulateVelocity();
        CalulateRotation();
        CalulateCamRotation();
        CalulateThruster();
    }
    
    private void setJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive { //JointDrive is a struct
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }

    private void CalulateVelocity()
    {
        //Calculate movement velocity as a 3d vector
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        //Individual movement vectors
        Vector3 _moveHorizontal = transform.right * _xMov;
        Vector3 _moveVertical = transform.forward * _zMov;

        //Final movement vector
        Vector3 _velocity = (_moveHorizontal + _moveVertical) * speed;

        //animate movement
        animator.SetFloat("ForwardVelocity", _zMov);

        //apply movement
        motor.Move(_velocity);
    }


    private void CalulateRotation()
    {
        //Calculate Rotation as a 3D vector (Turning around)
        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0, _yRot, 0) * lookSensitivity;

        //Apply rotation
        motor.Rotate(_rotation);
    }

    private void CalulateCamRotation()
    {
        //Calculate Camera Rotation as a 3D vector (Turning around)
        float _xRot = Input.GetAxisRaw("Mouse Y");

        float _cameraRotationX = _xRot * lookSensitivity;

        //Apply Camera rotation
        motor.RotateCamera(_cameraRotationX);
    }

    private void CalulateThruster()
    {
        //Calculate thruster force
        Vector3 _thrusterForce = Vector3.zero;

        if (Input.GetButton("Jump"))
        {
            _thrusterForce = Vector3.up * thrusterForce;
            setJointSettings(0f); //turns off spring while jumping
        }
        else
        {
            setJointSettings(jointSpring); //turns on spring while not jumping
        }
        //apply thruster force
        motor.ApplyThruster(_thrusterForce);
    }
}
