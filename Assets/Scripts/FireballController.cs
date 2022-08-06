using UnityEngine;
using Mirror;

public class FireballController : NetworkBehaviour
{

    [SerializeField] private float speed = 1;
    [SerializeField] private float maxFireballRange = 10;
    [SerializeField] public bool isTouched = false;
    private Vector3 originePosition;


    private void Start()
    {
        originePosition = transform.position;
    }



    private void Update()
    {
        if (!isTouched && Vector3.Distance(originePosition, transform.position) <= maxFireballRange)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        else
        {
            Debug.Log("Destroy GameObject name's : " + transform.name);
            Destroy(gameObject);
        }

    }
    
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
        //check projectile hit the player himself
            
            
            Debug.Log(collider.name + " a �t� touch�");
            isTouched = true;
            //playerTouched = collider.gameObject.GetComponent<Player>();

            CmdPlayerAttack(collider.name, 30f);

        }
        else
        {
            //The projectile hit other collider ( wall, ... )
            isTouched = true;
            Debug.Log("Other collider hit");
        }
    }


    [Command(requiresAuthority = false)]
    private void CmdPlayerAttack(string playerName, float damage)
    {
        Debug.Log(playerName + "� �t� touch�");

        Player player = GameManager.GetPlayer(playerName);
        player.RpcTakeDamage(damage);
    }
   
}
