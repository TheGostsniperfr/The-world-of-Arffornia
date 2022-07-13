
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 3f;

    [SerializeField]
    private float mouseSensitivityX = 5f;

    [SerializeField]
    private float mouseSensitivityY = 5f;

    [SerializeField]
    private bool isGrounded = true;

    //max time to jump
    [SerializeField]
    private float jumpMaxTime = 0.3f;

    [SerializeField]
    private bool isJumping = false;


    //save time when jum started
    private float jumpTimeStart;

    

    



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

        //player presses jump button and touched the ground
        if (Input.GetButton("Jump") && (isGrounded || isJumping))
        {
            Debug.Log("key espace pressed");



            //if player start to pressed jump button:
            if (isJumping == false)
            {
                isJumping = true;
                jumpTimeStart = Time.time;

                //player jump, so he quit the ground
                isGrounded = false;
            }     

            //time juping calcul
            if(Time.time < (jumpTimeStart + jumpMaxTime))
            {
                motor.jump();
            }
        }
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
}
