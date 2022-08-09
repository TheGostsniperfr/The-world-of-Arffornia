using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using System;

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
    [SerializeField] private bool isTarget;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float aimBot_Range = 10;
    [SerializeField] private float aimBot_TurnDegree = 180;

    [SerializeField] private Collider aimBot_ActualTarget;
    [SerializeField] private int aimBot_maxEntitiesBeforeLivingTarget = 3;
    [SerializeField] private List<string> aimBot_filterTag;

    Collider[] targetsList;




    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("Pas de caméra renseignée sur le système de tir.");
            this.enabled = false;
        }

        effectToSpawn = vfx[0];
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            //Check target
            targetsList = targetList();
            aimBot();


            if (Input.GetButtonDown("Fire1") && ((timeThrowEffect+cooldownNextAttack) <= Time.time))
            {
                
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
        //Check target and choose him

        if (aimBot_ActualTarget != null)
        {
            if (!targetTooFar())
            {
                //send target position to playerController for it to apply the rotation on player
                Debug.Log("send target to player controller");
                playerController.aimBot_Target(aimBot_ActualTarget.gameObject);
            }
            else
            {
                Debug.Log("target too far");
            }
            
        }
        else
        {
            //choose a target
            Debug.Log("chosse target");
            selectTarget();
        }

    }

    private void selectTarget()
    {
        //search potential target

        if (targetsList.Length != 0)
        {
            isTarget = true;
            //take the closest target
            aimBot_ActualTarget = targetsList[0];
            int potentialTargetCounter = 0;

            foreach (Collider targets in targetsList)
            {
                if (aimBot_filterTag.Contains(targets.gameObject.tag))
                {
                    potentialTargetCounter++;

                    if (targets.gameObject != this.gameObject)
                    {
                        aimBot_ActualTarget = targets;
                        break;
                    }
                }
            }
        }
        else
        {
            //no target in area
            isTarget = false;
            Debug.Log("no target in area");
        }
    }

    private bool targetTooFar()
    {

        //search if target is too far

        if (targetIsAlive(aimBot_ActualTarget.gameObject))
        {
            int targetIndex = Array.IndexOf(targetsList, aimBot_ActualTarget);

            if (targetIndex <= aimBot_maxEntitiesBeforeLivingTarget && targetIndex != -1)
            {
                return false;
            }
            else
            {
                Debug.Log("max entities before living target nb: " + targetIndex);
            }
        }
        else
        {
            Debug.Log("target not alive");

        }


        //target is too far or is not alive
        aimBot_ActualTarget = null;
        return true;
    }

    private Collider[] targetList()
    {
        //list all potentional target in area range

        Collider[] targetsList = Physics.OverlapSphere(transform.position, aimBot_Range);
        targetsList.OrderBy(x => Vector3.Distance(this.transform.position, x.transform.position)).ToArray();

        //delete first element ( it's the localplayer )
        targetsList = targetsList.Where(x => x != targetsList[0]).ToArray();


        return targetsList;
    }

    private bool targetIsAlive(GameObject _target)
    {
        return GameObject.Find(_target.gameObject.name);
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