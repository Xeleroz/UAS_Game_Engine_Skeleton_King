using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad;
    public Vector2 playerPosition;
    UIManager uIManager;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            SceneManager.LoadScene(sceneToLoad);
            uIManager.gameOverUI.SetActive(false);
            uIManager.RestartGame();
        }
    }

}
