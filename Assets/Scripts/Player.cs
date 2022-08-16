using Mirror;
using UnityEngine;
using System.Collections;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;
    [SerializeField] private PlayerUI playerUI;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabledOnStart;

    public CharacterController characterController;
    [SerializeField] private PlayerSetup playerSetup;

    public void Setup()
    {
        wasEnabledOnStart = new bool[disableOnDeath.Length];
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            wasEnabledOnStart[i] = disableOnDeath[i].enabled;
        }

        SetDefaults();


    }

    public void SetDefaults()
    {
        playerUI = playerSetup.playerUIInstance.GetComponent<PlayerUI>();

        isDead = false;
        currentHealth = maxHealth;
        playerUI.SetMaxHealth(maxHealth);
        playerUI.SetHealth(currentHealth);


        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabledOnStart[i];
        }


    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.gameSettings.respawnTimer);
        SetDefaults();
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();

        characterController.enabled = false;
        characterController.transform.position = spawnPoint.position;
        characterController.transform.rotation = spawnPoint.rotation;
        characterController.enabled = true;

    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(100);
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;
        playerUI.SetHealth(currentHealth);
        Debug.Log(transform.name + " a maintenant : " + currentHealth + " points de vies.");


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }


        Debug.Log(transform.name + " a été éliminé.");

        StartCoroutine(Respawn());
    }
}
