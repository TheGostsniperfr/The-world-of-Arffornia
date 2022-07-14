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
            Debug.LogError("Pas de caméra renseignée sur le système de tir.");
            this.enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("clique gauche détecté");
            Attack();
        }
    }

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
    }

    [Command]
    private void CmdPlayerAttack(string playerName, float damage)
    {
        Debug.Log(playerName + "à été touché");

        Player player = GameManager.GetPlayer(playerName);
        player.RpcTakeDomage(damage);
    }



}