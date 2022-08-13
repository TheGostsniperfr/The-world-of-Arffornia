using UnityEngine;
using Mirror;
using System.Collections;
using Cinemachine;

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


    //aimBot 
    [SerializeField] private CinemachineFreeLook cinemachineCamera;
    [SerializeField] public bool aimBot_IsTarget;
    [SerializeField] public GameObject aimBot_Target;
    [SerializeField] private float aimBot_TimeNoMove = 0.25f;
    [SerializeField] private float aimBot_TurnSmoothTime = 0.5f;
    private float aimBot_TurnSmoothVelocity;
    [SerializeField] private float lookAtSpeed = 3f;


    private void Update()
    {
        if (isLocalPlayer)
        {
            if (!aimBot_IsTarget)
            {
                MovePlayer();
            }
            else
            {
                //stop all move animation
                if (isSprinting)
                {
                    isSprinting = false;
                    speed /= speedSprintMultiplicator;
                    turnSmoothTime /= speedTurnSmoothMultiplicator;
                }

                anim.SetBool("isWalking", false);
                anim.SetBool("isSprinting", false);
            }
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
            float targetAngle = targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
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

    public void aimBot_ApplyTarget()
    {
        //on attack button clicked
        //rotate player and camera
        StartCoroutine(aimBot_lookAt(aimBot_Target));

        //stop player
        aimBot_IsTarget = true;

    }

    

    private IEnumerator aimBot_lookAt(GameObject target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        float time = 0;
        
        

        while(time < 1)
        {
            //calcul rotate
            Quaternion rotationToTarget = Quaternion.Slerp(transform.rotation, lookRotation, time);
                       
            //Apply rotate player body to target
            transform.rotation = Quaternion.Euler(new Vector3(0f, rotationToTarget.eulerAngles.y, 0f));


            //Apply rotate player camera to target
            float _angle = Mathf.SmoothDampAngle(cinemachineCamera.m_XAxis.Value, rotationToTarget.eulerAngles.y, ref aimBot_TurnSmoothVelocity, aimBot_TurnSmoothTime);
            cinemachineCamera.m_XAxis.Value = _angle;

            time += Time.deltaTime * lookAtSpeed;

            yield return null;
        }
        yield return new WaitForSeconds(aimBot_TimeNoMove);

        aimBot_IsTarget = false;

        StopCoroutine(aimBot_lookAt(null));

    }

    

}
