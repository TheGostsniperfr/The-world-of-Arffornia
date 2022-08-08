using UnityEngine;
using Mirror;
public class PlayerController : NetworkBehaviour
{

    //Player speed var init
    [SerializeField]
    private float speed = 3f;
    [SerializeField]
    private float turnSmoothTime = 0.2f;
    private Vector3 Velocity;

    [SerializeField]
    private bool isSprinting = false;
    [SerializeField]
    private float speedSprintMultiplicator = 1.75f;
    [SerializeField]
    private float speedTurnSmoothMultiplicator = 0.5f;



    public CharacterController characterController;

    float turnSmoothVelocity;

    [SerializeField]
    private Transform cam;

    [SerializeField]
    private bool isJumping = false;

    private float jumpTimeCounter;

    [SerializeField]
    private float jumpTime = 0.3f;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float gravity;


    //Animator
    [SerializeField] 
    private Animator anim;
    [SerializeField] NetworkAnimator netAnim;

    private void Update()
    {
        if (isLocalPlayer)
        {
            MovePlayer();
        }

    }


    private void MovePlayer()
    {
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSprinting)
        {
            isSprinting = true;
            speed *= speedSprintMultiplicator;
            turnSmoothTime *= speedTurnSmoothMultiplicator;

            

            if (!anim.GetBool("isSprinting"))
            {
                //Debug.Log("isSprinting true");
                anim.SetBool("isSprinting", true);
                
            }

        }
        else
        {
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                isSprinting = false;
                speed /= speedSprintMultiplicator;
                turnSmoothTime /= speedTurnSmoothMultiplicator;

                if (anim.GetBool("isSprinting"))
                {
                    //Debug.Log("isSprinting false");
                    anim.SetBool("isSprinting", false);
                }
            }
                
            

        }

        

        Vector3 direction = new Vector3(xMov, 0f, zMov).normalized;

        

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);


            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * speed * Time.deltaTime);

            if (!anim.GetBool("isWalking"))
            {
                //Debug.Log("isWalking true");
                anim.SetBool("isWalking", true);
            }
        }
        else
        {

            if (anim.GetBool("isWalking"))
            {
                //Debug.Log("isWalking false");
                anim.SetBool("isWalking", false);
            }
        }

    

        if (characterController.isGrounded)
        {

            Velocity.y = -1f;

            if (!isJumping && Input.GetKey(KeyCode.Space))
            {
                //Debug.Log("key espace pressed");
                Velocity.y = jumpForce;
                isJumping = true;
                jumpTimeCounter = jumpTime;
                netAnim.SetTrigger("jump");
                anim.SetBool("isJumping", true);

            }
        }
        else
        {
            Velocity.y -= gravity * -2f * Time.deltaTime;
        }

        if (isJumping && Input.GetButton("Jump"))
        {
            if (jumpTimeCounter > 0)
            {
                Velocity.y = jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
                anim.SetBool("isJumping", false);

            }

        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            anim.SetBool("isJumping", false);

        }


        characterController.Move(Velocity * Time.deltaTime);



        if(!characterController.isGrounded && Velocity.y == -1f)
        {
            //Debug.Log("is not grounded");
        }

        

    }
}
