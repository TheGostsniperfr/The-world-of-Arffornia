using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

public class PlayerAttack : NetworkBehaviour
{
    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;

    [SerializeField] private CharacterController characterController;


    //attack with fireball
    [Header("Fireball")]
    public List<GameObject> vfx = new List<GameObject>();
    private GameObject effectToSpawn;

    [SerializeField] private float timeBeforeThrowEffect = 0.3f;
    private float timeLastAttack;
    [SerializeField] private float cooldownNextAttack = 0.5f;
    private bool fireBallThrow;


    [SerializeField] private Player player;
    [SerializeField] private float attackEnergyCost = 30f;


    //regen
    [Header("Regen")]
    [SerializeField] private float timeAfterStartRegen = 2f;

     //Animator
    [Header("Animator")]
    [SerializeField]
    private Animator anim;
    [SerializeField] private NetworkAnimator netAnim;

    private FireballController fireballController;


    //Player aimbot 
    [Header("Aimbot")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float aimBot_Range = 10;
    [SerializeField] private Collider aimBot_ActualTarget;
    [SerializeField] private int aimBot_maxEntitiesBeforeLivingTarget = 3;
    [SerializeField] private List<string> aimBot_filterTag;
    [SerializeField] private List<Collider> targetsList;




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

            //check regen
            if(!player.isRegen && (timeLastAttack + timeAfterStartRegen) <= Time.time)
            {
                Debug.Log("isRegen is turn on true");
                player.isRegen = true;
            }



            if (Input.GetButtonDown("Fire1") && ((timeLastAttack + cooldownNextAttack) <= Time.time) && characterController.isGrounded && player.isEnergySufficient(attackEnergyCost))
            {
                player.isRegen = false;
                


                if (aimBot_ActualTarget != null)
                {
                    //camera target focus
                    playerController.aimBot_ApplyTarget();
                }
                netAnim.SetTrigger("throwProjectil");


                timeLastAttack = Time.time;
                fireBallThrow = true;
            }

            if(fireBallThrow && ((timeLastAttack + timeBeforeThrowEffect) <= Time.time))
            {
                fireBallThrow = false;
                FireBallAttack();

            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                //reset aimBot focus
                aimBot_ActualTarget = null;
                selectTarget();
                sendTarget();
            }
        }
    }

  

    private void aimBot()
    { 
        Collider newTarget = aimBot_ActualTarget;

        if(aimBot_ActualTarget == null)
        {
            //no target
            selectTarget();
        }
        else
        {
            //have target 
            //check if target is too far

            if (targetTooFar())
            {

                selectTarget();
            }
            
        }


        //check if target has updated
        if (aimBot_ActualTarget != newTarget)
        {
            //update target
            sendTarget();
        }   
    }

    private void sendTarget()
    {
        if (aimBot_ActualTarget != null)
        {
            playerController.aimBot_Target = aimBot_ActualTarget.gameObject;
        }
    }

    private bool selectTarget()
    {
        //search potential target
        //return true if has found a target

        if (targetsList.Count != 0)
        {
            //take the closest target
            aimBot_ActualTarget = targetsList[0];
            return true;
        }
        
        //no target in area
        aimBot_ActualTarget = null;
        return false;
            
    }

    private bool targetTooFar()
    {
        
            int targetIndex = targetsList.IndexOf(aimBot_ActualTarget);

            //search if target is too far
            if (!GameObject.Find(aimBot_ActualTarget.gameObject.name) || !(targetIndex <= aimBot_maxEntitiesBeforeLivingTarget-1 && targetIndex != -1))
            {
            // search new target
                return true;
            }

        return false;
    }

    private List<Collider> targetList()
    {
        //return list of all target filter by tag and not the localplayer

        Collider[] _targetsList = Physics.OverlapSphere(transform.position, aimBot_Range);
        _targetsList = _targetsList.OrderBy(x => Vector3.Distance(this.transform.position, x.transform.position)).ToArray();

        List<Collider> newTargetsList = new();

        foreach (Collider target in _targetsList)
        {
            if (aimBot_filterTag.Contains(target.gameObject.tag))
            {
                if (target.gameObject != this.gameObject)
                {
                    newTargetsList.Add(target);
                }
            }
        }

        targetsList = newTargetsList;

        return targetsList;
    }

    [Command(requiresAuthority = false)]
    private void FireBallAttack()
    {
        GameObject vfx;

        Player player = GetComponent<Player>();

        if (player != null)
        {
            vfx = Instantiate(effectToSpawn, player.transform.position, Quaternion.identity);

            vfx.transform.localRotation = player.transform.rotation;


            if (aimBot_ActualTarget != null)
            {
                //calcule difference angle between player and target

                /*
                Vector3 dir = aimBot_ActualTarget.transform.position - transform.position;

                float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg + 90;
                
                vfx.transform.Rotate(-angle, 0, 0, Space.Self);
                */

                float angle = Mathf.Atan(aimBot_ActualTarget.transform.position.y - transform.position.y) * Mathf.Rad2Deg;
                vfx.transform.Rotate(-angle/2, 0, 0);

                //Debug.Log("angle : " + angle + " target : " + aimBot_ActualTarget);

            }

            fireballController = vfx.GetComponent<FireballController>();
            fireballController.playerOrigine = transform.name;








            NetworkServer.Spawn(vfx);
        }
    }
}