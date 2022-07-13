using UnityEngine;
using Mirror;

public class PlayerAttack : NetworkBehaviour
{
    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("Pas de cam�ra renseign�e sur le syst�me de tir.");
            this.enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("clique gauche d�tect�");
            Shoot();
        }
    }

    [Client]
    private void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask))
        {
            if (hit.collider.tag == "Player")
            {
                CmdPlayerShot(hit.collider.name);
            }
        }
    }

    [Command]
    private void CmdPlayerShot(string playerName)
    {
        Debug.Log(playerName + "� �t� touch�");
    }



}