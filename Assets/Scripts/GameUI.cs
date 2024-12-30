using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public void OnBackButton()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnResetButton()
    {
        SceneManager.LoadScene("GameScene");
    }
}
