using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public enum SpeedUnit
{
    mph,
    kmh
}

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    public GameObject speedTextObject;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;

    private float currentSpeed;
 
    private SpeedUnit speedUnit = SpeedUnit.kmh;
    private int decimalPlaces;
    Rigidbody rb;




    private double speed;



    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }


    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();       
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot
;       wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("radar"))
        {
            other.gameObject.SetActive(false);
            if(currentSpeed>40){
                speedTextObject.SetActive(true);
            }
        }
    }

    void Start(){
        rb = GetComponent<Rigidbody>();
        speedTextObject.SetActive(false);
    }

    void Update(){

         if (speedUnit == SpeedUnit.mph)
        {
            // 2.23694 is the constant to convert a value from m/s to mph.
            currentSpeed = (float)Math.Round(rb.velocity.magnitude * 2.23694f, decimalPlaces);
        }

        else 
        {
            // 3.6 is the constant to convert a value from m/s to km/h.
            currentSpeed = (float)Math.Round(rb.velocity.magnitude * 3.6f, decimalPlaces);
        }
    }

    IEnumerator CalculateSpeed(){
        Vector3 lastPosition = transform.position;
        yield return new WaitForFixedUpdate();
        speed = (lastPosition - transform.position).magnitude * 2.237*Time.deltaTime;

    }
}
