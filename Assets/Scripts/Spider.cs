using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spider : MonoBehaviour
{
    [SerializeField] private WebPoint _currentWebPoint;
    [SerializeField] private WebPoint _targetWebPoint;
    [SerializeField] private Web _web;
    [SerializeField] private Fly _fly;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _deathForce = 100.0f;
    [SerializeField] private float _deathTorque = 200.0f;
    public static event Action OnSpiderDeath;

    public void SetSpawnDetails(WebPoint currentWebPoint, WebPoint flyWebPoint, Web web)
    {
        _fly = FindObjectOfType<Fly>();
        _currentWebPoint = currentWebPoint;
        _web = web;
        _targetWebPoint = _web.ReturnNextClosestWebPoint(currentWebPoint, flyWebPoint);
        StartCoroutine(LookAtTransform(_targetWebPoint.transform));
        _fly.OnLandedOnPoint += OnLandedOnPoint;
    }

    private void OnLandedOnPoint(WebPoint flyPoint)
    {
        StartCoroutine(MoveToNextPoint());
    }

    WebPoint ReturnTargetWebPoint()
    {
        return _web.ReturnNextClosestWebPoint(_currentWebPoint, _fly.ReturnCurrentWebPoint());
    }

    IEnumerator MoveToNextPoint()
    {
        yield return StartCoroutine(MoveBetweenTransforms(_currentWebPoint.transform, _targetWebPoint.transform));
        transform.parent = _targetWebPoint.transform;
        _currentWebPoint = _targetWebPoint;

        _targetWebPoint = ReturnTargetWebPoint();
        yield return StartCoroutine(LookAtTransform(_targetWebPoint.transform));
    }

    IEnumerator MoveBetweenTransforms(Transform pointA, Transform pointB)
    {
        float m_time = 0.0f;
        while (m_time < 1.0f)
        {
            transform.position = Vector2.Lerp(pointA.position, pointB.position, m_time);
            m_time += Time.deltaTime;
            yield return null;
        }
        transform.position = pointB.position;
    }

    IEnumerator LookAtTransform(Transform target)
    {
        Vector2 m_initialUp = transform.up;
        Vector2 m_targetUp = target.position - transform.position;
        float m_time = 0.0f;
        while(m_time < 1.0f)
        {
            transform.up = Vector2.Lerp(m_initialUp, m_targetUp, m_time);
            m_time += Time.deltaTime;
            yield return null;
        }
        transform.up = m_targetUp;
    }

    public void OnTargetHit()
    {
        _fly.OnLandedOnPoint -= OnLandedOnPoint;
        _collider.enabled = false;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.gravityScale = 1.0f;
        _rigidbody.AddForce(Vector2.up * _deathForce);
        _rigidbody.AddTorque(_deathTorque);
        OnSpiderDeath?.Invoke();
    }

    private void OnDisable()
    {
        _fly.OnLandedOnPoint -= OnLandedOnPoint;
    }
}
