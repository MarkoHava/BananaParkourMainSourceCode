using UnityEngine;

public class PausedMenu : MonoBehaviour {

    [SerializeField] Canvas OptionsMenu;
    private void Awake()
    {
        OptionsMenu.enabled = false;
    }
    public void UnpauseBtn()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        PublicConstants.Singleton.R_Player.GetComponent<AudioSource>().UnPause();
        GetComponent<Canvas>().enabled = false;
    }
    public void OptionsBtn()
    {
        OptionsMenu.enabled = true;
        GetComponent<Canvas>().enabled = false;
    }
    public void QuitBtn()
    {
        Application.Quit();
    }
}
