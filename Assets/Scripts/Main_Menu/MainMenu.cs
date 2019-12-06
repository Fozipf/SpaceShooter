using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        Text scoreText = GameObject.Find("Score_Text").GetComponent<Text>();
        if (scoreText == null)
        {
            Debug.LogError("scoreText is NULL");
        }

        scoreText.text = "Highscore: " + PlayerPrefs.GetInt("score", 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }
}
