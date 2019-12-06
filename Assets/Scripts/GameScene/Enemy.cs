using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1.8f;

    private Player _player;
    private Animator _animator;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    private bool _isEnemyDestroyed;

    [SerializeField]
    private GameObject _enemyLaserPrefab;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the Enemy is NULL.");
        }

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("_player is NULL.");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("_animator is NULL.");
        }
    }

    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire && !_isEnemyDestroyed)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;

            GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= -5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            _isEnemyDestroyed = true;

            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            _speed = 0;
            _animator.SetTrigger("OnEnemyDeath");
            _audioSource.Play();

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }

        if (other.tag.Equals("Laser"))
        {
            _isEnemyDestroyed = true;

            Laser laser = other.GetComponent<Laser>();
            if (!laser._isEnemyLaser)
            {

                Destroy(other.gameObject);
                if (_player != null)
                {
                    _player.AddScore(10);
                }

                _speed = 0;
                _animator.SetTrigger("OnEnemyDeath");
                _audioSource.Play();

                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 2.8f);
            }
        }
    }
}
