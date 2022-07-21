using UnityEngine;

public class FireballController : MonoBehaviour
{
    [SerializeField] private Player playerOwner;
    //[SerializeField] private Player playerTouched = null;

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
            if (collider.gameObject != playerOwner)
            {
                Debug.Log(collider.name + " a été touché");
                isTouched = true;
                //playerTouched = collider.gameObject.GetComponent<Player>();
            }
            else
            {
                Debug.Log("le projectile a touché le joueur qui la lancé /!\\ ");
            }
        }
        else
        {
            //The projectile hit other collider ( wall, ... )
            isTouched = true;
            Debug.Log("Other collider hit");
        }
    }
   
}
