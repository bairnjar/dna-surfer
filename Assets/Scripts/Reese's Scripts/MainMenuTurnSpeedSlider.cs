using UnityEngine;
using UnityEngine.UI;

// Test Script - Reese
public class MainMenuTurnSpeedSlider : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private Text sliderText;

    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        if (sliderText == null)
            Debug.LogError("You forgot to drag the sliderText in.", this);
    }

    public void UpdateTurnSpeed()
    {
        sliderText.text = "Slider Value: " + slider.value.ToString();
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("turnSpeed", slider.value);
    }
}
