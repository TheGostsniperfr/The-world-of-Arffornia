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

    //Health
    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private float currentHealth;
    private float tempoHealth;
    private PlayerUI playerUI;

    //energy bar 
    [Header("Energy bar")]
    [SerializeField] private float curentEnergy = 100f;
    [SerializeField] private float maxxEnergyBar = 100f;
    [SerializeField] private float regenEnergyBar = 1f;
    [SerializeField] private float speedRegenEnergyBar = 1f;
    [SerializeField] public bool isRegen = true;


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
        isDead = false;
        currentHealth = maxHealth;
        tempoHealth = currentHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabledOnStart[i];
        }

        if (isLocalPlayer)
        {
            playerUI = playerSetup.playerUIInstance.GetComponent<PlayerUI>();
            if (playerUI == null)
            {
                Debug.LogError("pas de Player UI");
            }

            playerUI.SetMaxHealth(currentHealth, maxHealth);
            playerUI.SetHealth(currentHealth);
            playerUI.SetMaxEnergy(curentEnergy, maxxEnergyBar);
            playerUI.SetEnergy(currentHealth);
        }

    }

    public bool isEnergySufficient(float attackEnergyCost)
    {
        if(curentEnergy - attackEnergyCost >= 0)
        {
            curentEnergy -= attackEnergyCost;
            playerUI.SetEnergy(curentEnergy);
            return true;
        }
        else
        {
            return false;
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
        else
        {
            if(currentHealth != tempoHealth)
            {
                playerUI.SetHealth(currentHealth);
            }

            if (isRegen)
            {
                if (curentEnergy + regenEnergyBar <= maxxEnergyBar)
                {
                    curentEnergy += regenEnergyBar;
                    playerUI.SetEnergy(curentEnergy);
                }
                else
                {
                    curentEnergy = maxxEnergyBar;
                }
            }
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

        Debug.Log(transform.name + " a ?t? ?limin?.");
        StartCoroutine(Respawn());
    }
}
