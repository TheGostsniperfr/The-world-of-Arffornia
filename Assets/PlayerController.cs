
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{

    //Player speed var init
    [SerializeField]
    private float speed = 3f;
    [SerializeField]
    private float turnSmoothTime = 0.1f;
    private Vector3 Velocity;


    public CharacterController characterController;

    float turnSmoothVelocity;

    [SerializeField]
    private Transform cam;

    [SerializeField]
    private bool isJumping = false;

    [SerializeField]
    private float jumpTimeCounter;

    [SerializeField]
    private float jumpTime = 0.3f;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float gravity;




    private void Update()
    {
        MovePlayer();
    }


    private void MovePlayer()
    {
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(xMov, 0f, zMov).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);


            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (characterController.isGrounded)
        {
            Velocity.y = -1f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Velocity.y = jumpForce;
            }
        }
        else
        {
            Velocity.y -= gravity * -2f * Time.deltaTime;
        }

        characterController.Move(Velocity * Time.deltaTime);

    }
}
