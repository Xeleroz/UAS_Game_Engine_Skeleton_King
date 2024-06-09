using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverUI;
    GameObject player;

    void Start()
    {
        HideGameOver();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            SceneManager.LoadScene("MainMenu");
        }

    }
    public void ShowGameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    public void HideGameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
}
