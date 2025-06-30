using UnityEngine;

public class Enable_PausedMenu : MonoBehaviour {
    [SerializeField] Canvas pausedMenu;
    [SerializeField] Canvas optionsMenu;

    private void Awake()
    {
        pausedMenu.enabled = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!pausedMenu.enabled && optionsMenu.enabled) optionsMenu.enabled = false;
            else pausedMenu.enabled = !pausedMenu.enabled;

            if (pausedMenu.enabled) GetComponent<AudioSource>().Pause();
            else GetComponent<AudioSource>().UnPause();
            Time.timeScale = pausedMenu.enabled ? 0 : 1;
            Cursor.lockState = pausedMenu.enabled ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
