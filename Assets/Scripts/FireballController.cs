using UnityEngine;
using Mirror;

public class FireballController : NetworkBehaviour
{

    [SerializeField] private float speed = 1;
    [SerializeField] private float maxFireballRange = 10;
    


    private bool isTouched = false;
    private Vector3 originePosition;
    public string playerOrigine;



    [SerializeField] private GameObject FireballExplosion;
    [SerializeField] private float explosionRadius = 3f;




    private void Start()
    {
        originePosition = transform.position;
        Debug.Log("Fireball playerThrow : " + playerOrigine);
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
                Explode();
                FireballDestroy();

            }
        }

    }

    private void OnTriggerEnter(Collider collider)
    {

        //empeche bug lors de l'init
        if (playerOrigine != "")
        {

            if (collider.transform.name != playerOrigine)
            {
                Debug.Log("colliderName : " + collider.name);

                Explode();
                FireballDestroy();

            }
        }
        else
        {
            Debug.Log("playerOrigine est null");
        }
    }


    [Command(requiresAuthority = false)]
    private void CmdPlayerAttack(string playerName, float damage)
    {

        Player player = GameManager.GetPlayer(playerName);
        player.RpcTakeDamage(damage);

    }




    [Command(requiresAuthority = false)]
    private void FireballDestroy()
    {
        Destroy(gameObject);
    }



    [Command(requiresAuthority = false)]
    private void Explode()
    {

        // explosion effect
        GameObject exploVFX = Instantiate(FireballExplosion, transform.position, transform.rotation);
        NetworkServer.Spawn(exploVFX);

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
    }   
}
