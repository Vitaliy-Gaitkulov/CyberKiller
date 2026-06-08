using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Toggle();
    }

    public void Toggle()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
    }

    public void ExitToMenu()
    {
        PhotonNetwork.LeaveRoom();
    }

    void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }
}
