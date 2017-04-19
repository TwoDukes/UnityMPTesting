using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    #region Variables  

    [Header("Movement Settings:")]
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;
    [SerializeField]

    [Header("Thruster Settings:")]
    private float thrusterForce = 1000f;
    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;
    [SerializeField]
    private LayerMask enviromentMask;

    [Header("Spring Settings:")]
    [SerializeField]
    private float jointSpring;
    [SerializeField]
    private float jointMaxForce;

    //Component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    #endregion

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        setJointSettings(jointSpring); //sets initial configurable joint values
    }

    private void Update()
    {

        CalulateGround();
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

    #region Calculate Functions
    private void CalulateGround() //sets target position for the spring
    {
        RaycastHit hit;
        //looks for object below. if found set spring target to object, else set to ground
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 100f, enviromentMask))
        {
            joint.targetPosition = new Vector3(0f, -hit.point.y, 0f);
        }else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }
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
        float _yRot = Input.GetAxis("Mouse X");

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

        if (Input.GetButton("Jump") && thrusterFuelAmount > 0 && !PauseMenu.isOn)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime; //Burns off fuel while flying

            if(thrusterFuelAmount >= 0.01f)
            {
                _thrusterForce = Vector3.up * thrusterForce;
                setJointSettings(0f); //turns off spring while jumping
            }
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime; //Regens fuel while not flying

            setJointSettings(jointSpring); //turns on spring while not jumping
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1.0f); //keeps thruster fuel from going over 100%

        //apply thruster force
        motor.ApplyThruster(_thrusterForce);
    }
    #endregion

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }
}
