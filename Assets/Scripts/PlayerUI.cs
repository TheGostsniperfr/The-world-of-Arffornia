
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] Image fill;
    [SerializeField] private TextMeshProUGUI textMesh;

    [SerializeField] private float speedTransitionHealth;
    [SerializeField] private float timeActualTransitionHealth = 0;
    //private bool isHealthChanging;
    [SerializeField] private float currentHealth;
    [SerializeField] private float newHealth;


    //healthBar

    private void Update()
    {
        if (currentHealth != newHealth)
        {
            HealthTransition();
        }
    }
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
}
