using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {
    [SerializeField] private Slider sensSlider;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Canvas PauseMenu;
    [SerializeField] private Toggle ToggleDisableFov;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sensSlider.value = MouseHandler.Singleton.sensitivityX / Screen.width;

        volumeSlider.value = 1;
        volumeSlider.onValueChanged.AddListener(s => SetVolume());
        sensSlider.onValueChanged.AddListener(s => SetSensitivity());
    }

    public void SetSensitivity()
    {
        MouseHandler.Singleton.sensitivityX = sensSlider.value * Screen.width;
        MouseHandler.Singleton.sensitivityY = sensSlider.value * Screen.height;
    }
    public void SetVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }
    public void CloseOptionsMenu()
    {
        PauseMenu.enabled = true;
        GetComponent<Canvas>().enabled = false;
    }
}
