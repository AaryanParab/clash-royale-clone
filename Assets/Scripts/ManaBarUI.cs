using UnityEngine;
using UnityEngine.UI;

public class ManaBarUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider manaSlider;
    public Text manaText;

    public void UpdateMana(float current, float max)
    {
        if (manaSlider != null)
        {
            manaSlider.maxValue = max;
            manaSlider.value = current;
        }

        if (manaText != null)
        {
            manaText.text = $"{Mathf.Round(current)} / {max}";
        }
    }
}