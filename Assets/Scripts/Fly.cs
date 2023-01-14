using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fly : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _flingForceMultiplier;
    [SerializeField] private Camera _camera;
    [SerializeField] private WebPoint _currentWebPoint;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private float _deathForce = 10.0f;
    [SerializeField] private float _deathTorque = 10.0f;
    public event Action<WebPoint> OnLandedOnPoint;
    public static event Action OnFlyDeath;
    private bool _hitFirstPoint = false;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WebPoint"))
        {
            CollidedWithWebPoint(collision);
        }

        if (collision.CompareTag("Spider"))
        {
            CollidedWithSpider(collision);
        }
    }

    void CollidedWithWebPoint(Collider2D collision)
    {
        transform.parent = collision.transform;
        transform.localPosition = Vector3.forward;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.isKinematic = true;
        collision.gameObject.GetComponent<WebPoint>().OnFling += OnFling;
        _currentWebPoint = collision.gameObject.GetComponent<WebPoint>();
        if (!_hitFirstPoint)
        {
            _hitFirstPoint = true;
            return;
        }
        OnLandedOnPoint?.Invoke(_currentWebPoint);
    }

    void CollidedWithSpider(Collider2D collision)
    {
        transform.parent = null;
        _collider.enabled = false;
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.gravityScale = 1.0f;
        _rigidbody.AddForce(Vector2.up * _deathForce);
        _rigidbody.AddTorque(_deathTorque);
        OnFlyDeath?.Invoke();
    }

    private void OnFling(Vector2 direction)
    {
        _rigidbody.isKinematic = false;
        _rigidbody.simulated = true;
        transform.parent = null;
        transform.up = -direction;
        _rigidbody.AddForce(direction * -_flingForceMultiplier);
        _currentWebPoint.OnFling -= OnFling;
        _currentWebPoint = null;
    }

    private void Update()
    {
        Vector3 m_currentPosition = transform.position;
        if(m_currentPosition.y < -_camera.orthographicSize)
        {
            m_currentPosition.y += (_camera.orthographicSize * 2);
        }
        else if (m_currentPosition.y > _camera.orthographicSize)
        {
            m_currentPosition.y -= (_camera.orthographicSize * 2);
        }

        if(m_currentPosition.x < -_camera.orthographicSize * _camera.aspect)
        {
            m_currentPosition.x += (_camera.orthographicSize * _camera.aspect * 2);
        }
        else if(m_currentPosition.x > _camera.orthographicSize * _camera.aspect)
        {
            m_currentPosition.x -= (_camera.orthographicSize * _camera.aspect * 2);
        }

        transform.position = m_currentPosition;
    }

    public WebPoint ReturnCurrentWebPoint()
    {
        return _currentWebPoint;
    }
}
