using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;

    [SyncVar]
    private float currentHealth;

    private void Awake()
    {
        SetDefaults();
    }


    public void SetDefaults()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float domage)
    {
        if(currentHealth - domage <= 0)
        {
            //player Died
            currentHealth = 0;
        }
        else
        {
            currentHealth -= domage;
        }

        Debug.Log(transform.name + " a : " + currentHealth + " HP");
    }
}
