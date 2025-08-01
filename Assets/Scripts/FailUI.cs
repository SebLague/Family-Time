using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailUI : MonoBehaviour
{
    public TMPro.TMP_Text reason;
    public Button retryButton;
    public Button quitButton;
    
    void Start()
    {
        retryButton.onClick.AddListener(Retry);
        quitButton.onClick.AddListener(Quit);
    }

    void Retry()
    {
        FindFirstObjectByType<GameManager>().RestartTimeLoop();
    }

    void Quit()
    {
        FindFirstObjectByType<GameManager>().RestartGame();
    }
}
