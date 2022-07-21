using UnityEngine;
using Mirror;
using System.Collections.Generic;

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
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("clique gauche détecté");
            //Attack();

            FireBallAttack();
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
    [Client]
    private void FireBallAttack()
    {
        GameObject vfx;
        Player player = GetComponent<Player>();

        if (player != null)
        {
            vfx = Instantiate(effectToSpawn, player.transform.position, Quaternion.identity);
            vfx.transform.localRotation = player.transform.rotation;

        }
        else
        {
            Debug.Log("No player");
        }
    }


    [Command]
    private void CmdPlayerAttack(string playerName, float damage)
    {
        Debug.Log(playerName + "à été touché");

        Player player = GameManager.GetPlayer(playerName);
        player.RpcTakeDamage(damage);
    }



}