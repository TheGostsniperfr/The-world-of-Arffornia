﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 jumpVelocity;
    private Vector3 rotation;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Rigidbody rb;

    [SerializeField]
    private Camera cam;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }



    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    public void JumpVelocity(Vector3 _jumpVelocity)
    {
        jumpVelocity = _jumpVelocity;
    }

    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    public void CameraRotate(float _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;
    }

  


    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    private void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if(jumpVelocity != Vector3.zero)
        {
            rb.velocity = jumpVelocity;
        }

        
    }
    private void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }





}
