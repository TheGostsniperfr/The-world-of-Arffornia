
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] Image fill;

    public void SetHealth(int health)
    {
        slider.value = health;
        Debug.Log("HealthBar + health : " + health + slider.value);

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        gradient.Evaluate(1f);
    }
}
