using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _speedMultiplier = 1.5f;
    [SerializeField]
    private float _speedMultiplierPowerup = 1.7f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _multiDirectionShotPrefab;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;
    [SerializeField]
    private GameObject _mainCamera;

    [SerializeField]
    private float _firerate = 0.15f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _score;
    [SerializeField]
    private int _shieldStrength = 2;
    [SerializeField]
    private int _ammoCount;
    private int _maxAmmo = 15;
    [SerializeField]
    private int _thrustersCharge = 100;

    private bool _isTripleShotEnabled;
    private bool _isShieldEnabled;
    private bool _isSpeedBoostEnabled;
    private bool _isMultiDirectionShotEnabled;
    private bool _thrustersActive;

    [SerializeField]
    private UIManager _uiManager;
    private SpawnManager _spawnManager;
    private SpriteRenderer _shieldSprite;

    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;

    void Start()
    {
        _ammoCount = _maxAmmo;

        StartCoroutine(ThrustersRoutine());

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The SpawnManager is NULL.");
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

        _shieldSprite = _shieldVisualizer.GetComponent<SpriteRenderer>();

        transform.position = new Vector3(0, 0, 0);
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammoCount > 0)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, verticalInput, 0);

        if (Input.GetKey(KeyCode.LeftShift) && _thrustersCharge > 0)
        {
            movementDirection *= _speedMultiplier;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _thrustersActive = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _thrustersActive = false;
        }

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

    IEnumerator ThrustersRoutine()
    {
        while (true)
        {
            if (_thrustersActive && _thrustersCharge > 0)
            {
                _thrustersCharge--;
            }

            _uiManager.UpdateThrusterHUD(_thrustersCharge);

            if (_thrustersCharge == 0)
            {
                yield return new WaitForSeconds(7);
                _thrustersCharge = 100;
            }

            yield return new WaitForSeconds(0.025f);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _firerate;

        if (_isMultiDirectionShotEnabled)
        {
            Instantiate(_multiDirectionShotPrefab, transform.position, Quaternion.identity);
        }
        else if (_isTripleShotEnabled)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _ammoCount--;
        _uiManager.UpdateAmmoCount(_ammoCount, _maxAmmo);
        _audioSource.Play();
    }

    public void Damage()
    {
        CameraShake cam = _mainCamera.GetComponent<CameraShake>();
        cam.Shake(0.1f, 0.1f, 0.5f, 0.2f);

        if (_isShieldEnabled)
        {

            Color shieldColor = _shieldSprite.color;

            switch (_shieldStrength)
            {
                case 2:
                    shieldColor.a = 0.6f;
                    break;
                case 1:
                    shieldColor.a = 0.3f;
                    break;
                case 0:
                    shieldColor.a = 1f;
                    _isShieldEnabled = false;
                    _shieldVisualizer.SetActive(false);
                    break;
            }
            _shieldSprite.color = shieldColor;
            _shieldStrength--;

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

    public void HealPlayer()
    {
        if (_lives < 3)
        {
            _lives++;

            if (_lives == 2)
            {
                _rightEngine.SetActive(false);
            }
            else if (_lives == 3)
            {
                _leftEngine.SetActive(false);
            }

            _uiManager.UpdateLives(_lives);
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

    public void EnableMultiDirectionShot()
    {
        _isMultiDirectionShotEnabled = true;
        Invoke("DisableMultiDirectionShot", 5);
    }

    private void DisableMultiDirectionShot()
    {
        _isMultiDirectionShotEnabled = false;
    }

    public void SpeedBoostActive()
    {
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    private IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitWhile(() => _isSpeedBoostEnabled);
        _speed *= _speedMultiplierPowerup;
        _isSpeedBoostEnabled = true;

        yield return new WaitForSeconds(5);
        _speed /= _speedMultiplierPowerup;
        _isSpeedBoostEnabled = false;
    }

    public void ShieldActive()
    {
        if (!_isShieldEnabled)
        {
            _shieldStrength = 2;
            _isShieldEnabled = true;
            _shieldVisualizer.SetActive(true);
        }

    }

    public void RefillAmmo()
    {
        _ammoCount = _maxAmmo;
        _uiManager.UpdateAmmoCount(_ammoCount, _maxAmmo);
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
