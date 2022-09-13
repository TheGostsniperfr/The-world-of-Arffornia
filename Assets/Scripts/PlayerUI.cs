
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    //healthBar
    [Header("Health Bar")]

    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] Image fill;
    [SerializeField] private TextMeshProUGUI textMesh;

    [SerializeField] private float speedTransitionHealth;
    [SerializeField] private float timeActualTransitionHealth = 0;
    //private bool isHealthChanging;
    [SerializeField] private float currentHealth;
    [SerializeField] private float newHealth;


    //Energy bar
    [Header("Energy Bar")]
    [SerializeField] private Slider sliderEnergyBar;
    [SerializeField] private Gradient gradientEnergyBar;
    [SerializeField] private Image fillEnergyBar;
    [SerializeField] private TextMeshProUGUI textMeshEnergyBar;

    [SerializeField] private float speedTransitionEnergyBar;
    [SerializeField] private float timeActualTransitionEnergyBar = 0;

    [SerializeField] private float currentEnergy;
    [SerializeField] private float newEnergy;

    private void Update()
    {
        if (currentHealth != newHealth)
        {
            HealthTransition();
        }
        if(currentEnergy != newEnergy)
        {
            EnergyTransition();
        }
    }

    //Health bar 
    public void SetHealth(float _newHealth)
    {
        newHealth = _newHealth;        
    }

    public void SetMaxHealth(float currentHealth, float maxHealth)
    {
        slider.value = currentHealth;
        slider.maxValue = maxHealth;
        currentHealth = maxHealth;
        gradient.Evaluate(1f);
        Debug.Log("setMaxHealth : " + maxHealth);
    }
    private void HealthTransition()
    {
        timeActualTransitionHealth += Time.deltaTime * speedTransitionHealth;
        slider.value = Mathf.Lerp(currentHealth, newHealth, timeActualTransitionHealth);

        textMesh.text = Mathf.Round(slider.value) + " / " + slider.maxValue;

        fill.color = gradient.Evaluate(slider.value / slider.maxValue);

        if (slider.value == newHealth)
        {
            currentHealth = newHealth;
            timeActualTransitionHealth = 0;
        }
    }

    //Energy bar
    public void SetEnergy(float _newEnergy)
    {
        newEnergy = _newEnergy;
    }

    public void SetMaxEnergy(float currentEnergy, float maxEnergy)
    {
        sliderEnergyBar.value = currentEnergy;
        sliderEnergyBar.maxValue = maxEnergy;
        currentEnergy = maxEnergy;
        gradientEnergyBar.Evaluate(1f);
        Debug.Log("setMaxEnergy" + maxEnergy);
    }

    private void EnergyTransition()
    {
        timeActualTransitionEnergyBar += Time.deltaTime * speedTransitionEnergyBar;
        sliderEnergyBar.value = Mathf.Lerp(currentEnergy, newEnergy, timeActualTransitionEnergyBar);

        textMeshEnergyBar.text = Mathf.Round(sliderEnergyBar.value) + " / " + sliderEnergyBar.maxValue;

        fillEnergyBar.color = gradientEnergyBar.Evaluate(sliderEnergyBar.value / sliderEnergyBar.maxValue);

        if (sliderEnergyBar.value == newEnergy)
        {
            currentEnergy = newEnergy;
            timeActualTransitionEnergyBar = 0;
        }
    }
    }
