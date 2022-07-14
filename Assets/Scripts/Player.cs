using Mirror;
using UnityEngine;
using System.Collections;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;
    [SyncVar]
    private bool _isAlive = true;
    public bool isAlive
    {
        get { return _isAlive; }
        protected set { _isAlive = value; }
    }

    [SyncVar]
    private float currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnableOnStart; 

    public void Setup()
    {
        wasEnableOnStart = new bool[disableOnDeath.Length];
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            wasEnableOnStart[i] = disableOnDeath[i].enabled;
        }
        SetDefaults();
    }


    public void SetDefaults()
    {
        isAlive = true;
        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnableOnStart[i];
        }

        Collider collider = GetComponent<Collider>();
        if(collider != null)
        {
            collider.enabled = true;
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.gameSettings.respawnTimer);
        SetDefaults();
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }


    [ClientRpc ]
    public void RpcTakeDomage(float domage)
    {
        if (!isAlive)
        {
            return;
        }

        if(currentHealth - domage <= 0)
        {
            //player Died
            Die();
            currentHealth = 0;
        }
        else
        {
            currentHealth -= domage;
        }

        Debug.Log(transform.name + " a : " + currentHealth + " HP");
    }

    private void Die()
    {
        isAlive = false;
        

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        Debug.Log(transform.name + " a ete elemine !");


        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        StartCoroutine(Respawn());
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                RpcTakeDomage(100);
            }
        }
    }
}
