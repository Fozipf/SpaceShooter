using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Image _LivesImg;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private Slider _thrusterHUD;
    [SerializeField]
    private GameObject _sliderFill;
    [SerializeField]
    private Text _waveCountdown;

    [SerializeField]
    private Sprite[] _liveSprites;

    private GameManager _gameManager;
    private Player _player;

    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL.");
        }
        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateLives(int currentLives)
    {
        _LivesImg.sprite = _liveSprites[currentLives];
    }

    public void UpdateAmmoCount(int ammo, int maxAmmo)
    {
        _ammoText.text = "Ammo: " + ammo + "/"+maxAmmo;
    }

    public void UpdateThrusterHUD(int value)
    {
        _thrusterHUD.value = value;
        if (value == 0)
        {
            _sliderFill.SetActive(false);
        }
        else
        {
            _sliderFill.SetActive(true);
        }
    }

    public void GameOverSequence()
    {
        int savedScore = PlayerPrefs.GetInt("score", 0);
        int actualScore = _player.GetScore();

        if (actualScore > savedScore)
        {
            PlayerPrefs.SetInt("score", actualScore);
            PlayerPrefs.Save();
        }

        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());

        _gameManager.GameOver();
    }

    public void UpdateWaveCountdown(float remainingSeconds)
    {
        _waveCountdown.text = remainingSeconds+" sec";
    }

    IEnumerator GameOverFlickerRoutine()
    {
        float frequency = 0.5f;
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(frequency);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(frequency);
        }
    }
}
