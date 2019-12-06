using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed = 7f;
    [SerializeField]
    private float _speedMultiplier = 1.7f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;

    [SerializeField]
    private float _firerate = 0.15f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _score;

    private bool _isTripleShotEnabled;
    private bool _isShieldEnabled;
    private bool _isSpeedBoostEnabled;

    private UIManager _uiManager;
    private SpawnManager _spawnManager;

    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;

    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The SpawnManager is NULL.");
        }
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the Player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }

        _shieldVisualizer = this.gameObject.transform.GetChild(0).gameObject;
        _shieldVisualizer.SetActive(false);

        transform.position = new Vector3(0, 0, 0);
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(movementDirection * _speed * Time.deltaTime);

        //The following could be replaced with "transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);"
        if (transform.position.y > 2f)
        {
            transform.position = new Vector3(transform.position.x, 2f, transform.position.z);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, transform.position.z);
        }



        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, transform.position.z);
        }

    }

    void FireLaser()
    {
        _canFire = Time.time + _firerate;

        if (_isTripleShotEnabled)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();
    }

    public void Damage()
    {
        if (_isShieldEnabled)
        {
            _isShieldEnabled = false;
            _shieldVisualizer.SetActive(false);
            return;
        }

        _lives--;

        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _uiManager.GameOverSequence();
            this.gameObject.SetActive(false);

        }
    }

    public void TripleShotActive()
    {
        _isTripleShotEnabled = true;
        Invoke("TripleShotPowerDown", 5);
    }

    private void TripleShotPowerDown()
    {
        _isTripleShotEnabled = false;
    }

    public void SpeedBoostActive()
    {
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    private IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitWhile(() => _isSpeedBoostEnabled);
        _speed *= _speedMultiplier;
        _isSpeedBoostEnabled = true;

        yield return new WaitForSeconds(5);
        _speed /= _speedMultiplier;
        _isSpeedBoostEnabled = false;
    }

    public void ShieldActive()
    {
        _isShieldEnabled = true;
        _shieldVisualizer.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("EnemyLaser"))
        {
            Damage();
            Destroy(other.gameObject);
        }
    }

    public int GetScore()
    {
        return _score;
    }
}
