using UnityEngine;
using Mirror;

public class FireballController : NetworkBehaviour
{

    [SerializeField] private float speed = 1;
    [SerializeField] private float maxFireballRange = 10;
    


    private bool isTouched = false;
    private Vector3 originePosition;


    [SerializeField] private GameObject FireballExplosion;
    [SerializeField] private float explosionRadius = 3f;



    private void Start()
    {
        originePosition = transform.position;
    }




    private void Update()
    {
        if (!isTouched)
        {
            if(Vector3.Distance(originePosition, transform.position) <= maxFireballRange)
            {

                transform.position += transform.forward * speed * Time.deltaTime;
            }
            else
            {
                Debug.Log("Destroy GameObject name's out of range : " + transform.name);
                Destroy(gameObject);
            }
        }

    }
    
    private void OnTriggerEnter(Collider collider)
    {

        if (Vector3.Distance(originePosition, transform.position) >= 1)
        {
            Explode();
        }
        /*
        if (collider.gameObject.tag == "Player")
        {
        //check projectile hit the player himself
            
            
            Debug.Log(collider.name + " a été touché");
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
        */
    }


    [Command(requiresAuthority = false)]
    private void CmdPlayerAttack(string playerName, float damage)
    {
        Debug.Log(playerName + "à été touché");

        Player player = GameManager.GetPlayer(playerName);
        player.RpcTakeDamage(damage);

    }


    private void Explode()
    {


        // explosion effect
        //Instantiate(FireballExplosion, transform.position, transform.rotation);

        //Get nearby object
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach(Collider player in colliders)
        {

            if(player.gameObject.tag == "Player")
            {
                //take damage
                CmdPlayerAttack(player.name, 30f);
            }

        }


        //kill object
        Debug.Log("Destroy GameObject name's : " + transform.name);
        Destroy(gameObject);


    }
   
}
