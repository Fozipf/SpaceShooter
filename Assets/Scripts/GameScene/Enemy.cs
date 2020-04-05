using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public enum MovementType
    {
        Normal, SideToSide, MoveAtAngle
    }

    [SerializeField]
    private MovementType _movementType = MovementType.Normal;

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

    private Vector3 _movementDirection;

    void Start()
    {

        _movementType = (MovementType)(Random.Range(0, Enum.GetValues(typeof(MovementType)).Length));

        if(_movementType == MovementType.MoveAtAngle)
        {
            transform.rotation = Quaternion.Euler(0, 0, 30);
        }else if(_movementType == MovementType.SideToSide)
        {
            int x = Random.Range(0, 2);
            _movementDirection = (x == 0 ? Vector3.left : Vector3.right);
        }

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

            GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position, transform.rotation);
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

        if(_movementType == MovementType.SideToSide)
        {
            

            transform.Translate(_movementDirection * _speed * Time.deltaTime);
        }

        //If moving out of playfield (left or right), appear on the other side
        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, transform.position.z);
        }

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
