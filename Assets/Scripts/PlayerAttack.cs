using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.Networking;

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
            if (Input.GetButtonDown("Fire1") && ((timeThrowEffect+cooldownNextAttack) <= Time.time))
            {
                Debug.Log("clique gauche détecté");
                //Attack();
                anim.SetTrigger("throwProjectil");

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

    /*
    [Client]
    private void Attack()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask))
        {
            if (hit.collider.tag == "Player")
            {
                CmdPlayerAttack(hit.collider.name, weapon.domage);
            }
        }
    }*/
    [Command]
    private void FireBallAttack()
    {
        GameObject vfx;
        Player player = GetComponent<Player>();

        if (player != null)
        {
            vfx = Instantiate(effectToSpawn, player.transform.position, Quaternion.identity);
            vfx.transform.localRotation = player.transform.rotation;

            NetworkServer.Spawn(vfx);
        }
        else
        {
            Debug.Log("No player");
        }
    }
}