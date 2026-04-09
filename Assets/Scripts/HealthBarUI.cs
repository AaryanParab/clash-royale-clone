using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider;

    public void UpdateHealth(float current, float max)
    {
        if (healthSlider != null)
            healthSlider.value = current / max;
    }
}