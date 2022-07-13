
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

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("key esapce pressed");

            motor.jump();

            isGrounded = false;

        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "floor")
        {
            Debug.Log("collision avec le sol");
            isGrounded = true;
        }
        else
        {
            //isGrounded = false;
        }
    }
}
