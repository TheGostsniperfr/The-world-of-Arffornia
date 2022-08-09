using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

public class PlayerAttack : NetworkBehaviour
{
    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;


    //attack with fireball
    public List<GameObject> vfx = new List<GameObject> ();
    private GameObject effectToSpawn;

    [SerializeField] private float timeBeforeThrowEffect = 0.3f;
    private float timeThrowEffect;
    [SerializeField] private float cooldownNextAttack = 0.5f;
    private bool fireBallThrow;


    //Animator
    [SerializeField]
    private Animator anim;
    [SerializeField] private NetworkAnimator netAnim;

    private FireballController fireballController;


    //Player aimbot 
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float aimBot_Range;
    [SerializeField] private float aimBot_TurnDegree;

    [SerializeField] private GameObject aimBot_ActualTarget;
    [SerializeField] private float aimBot_maxRangeTarget;




    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("Pas de cam�ra renseign�e sur le syst�me de tir.");
            this.enabled = false;
        }

        effectToSpawn = vfx[0];
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetButtonDown("Fire1") && ((timeThrowEffect+cooldownNextAttack) <= Time.time))
            {
                //Check target

                //Attack
                netAnim.SetTrigger("throwProjectil");


                timeThrowEffect = Time.time;
                fireBallThrow = true;
            }

            if(fireBallThrow && ((timeThrowEffect+timeBeforeThrowEffect) <= Time.time))
            {
                fireBallThrow = false;
                FireBallAttack();

            }
        }
    }


    private void aimBot()
    {
        //check no target currently
        if(aimBot_ActualTarget == null)
        {
            //search potential target
            Collider[] TargetList = Physics.OverlapSphere(transform.position, aimBot_Range);
            TargetList.OrderBy(x => Vector3.Distance(this.transform.position, x.transform.position)).ToArray();

            //delete first element ( it's the localplayer )
            TargetList = TargetList.Where(x => x != TargetList[0]).ToArray();
            

            if (TargetList.Length != 0)
            {
                //take the closest target
                //save target
                aimBot_ActualTarget = TargetList[0].gameObject;

                //rotate player to target direction


            }
            else
            {
                //pas de cible dans la zone
            }

        }
        else
        {
            //check Target is alive

            //rotate player to target direction

        }
    }


    [Command]
    private void FireBallAttack()
    {
        GameObject vfx;

        Player player = GetComponent<Player>();

        if (player != null)
        {
            vfx = Instantiate(effectToSpawn, player.transform.position, Quaternion.identity);
            
            vfx.transform.localRotation = player.transform.rotation;
            

            fireballController = vfx.GetComponent<FireballController>();
            fireballController.playerOrigine = transform.name;


            Debug.Log("islocalPlayer /!\\ : " + transform.name);



            NetworkServer.Spawn(vfx);
        }
  
    }
}