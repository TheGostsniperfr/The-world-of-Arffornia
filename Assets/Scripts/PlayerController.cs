
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{

    //Player speed var init
    [SerializeField]
    private float speed = 3f;


    //Mouse sensi var init
    [SerializeField]
    private float mouseSensitivityX = 5f;

    [SerializeField]
    private float mouseSensitivityY = 5f;


    //Jump var init
    [SerializeField]
    private bool isGrounded = true;

    [SerializeField]
    private bool isJumping = false;

    [SerializeField]
    private float jumpTimeCounter;

    [SerializeField]
    private float jumpTime = 0.3f;

    [SerializeField]
    private float jumpForce;

    private Vector3 jumpVelocity;
    private PlayerMotor motor;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    
    private void Update()
    {
        //Player velocity calcul

        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

        motor.Move(velocity);


        //Player rotation calcul
        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 Playerrotation = new Vector3(0, yRot, 0) * mouseSensitivityX;
        motor.Rotate(Playerrotation);

        float cameraRotationX = xRot * mouseSensitivityY;
        motor.CameraRotate(cameraRotationX);




        //Player jump calcul
        jumpVelocity = Vector3.zero;

        //player presses jump button and touched the ground
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("key espace pressed");

            isJumping = true;
            jumpTimeCounter = jumpTime;
            jumpVelocity = Vector3.up * jumpForce;


        }
        if (isJumping && Input.GetButton("Jump")) 
        { 
            if(jumpTimeCounter > 0)
            {
                jumpVelocity = Vector3.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping=false;
            }

        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        motor.JumpVelocity(jumpVelocity);




    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "floor")
        {
            Debug.Log("collision avec le sol");
            isGrounded = true;
            isJumping=false;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        //isGrounded = false;
    }

}
