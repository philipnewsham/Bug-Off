using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WebPoint : MonoBehaviour
{
    private Vector2 _initialPosition;
    private bool _isDragging = false;
    [SerializeField] private float _pullDistance = 1.0f;
    [SerializeField] private float _resetDuration = 1.0f;
    [SerializeField] private AnimationCurve _resetCurve;
    [SerializeField] private CircleCollider2D _collider;
    public event Action<Vector2> OnFling;
    public static event Action OnWebFling;
    public List<WebPoint> connectedWebPoints = new List<WebPoint>();

    void Awake()
    {
        _initialPosition = transform.position;
    }

    void Update()
    {
        if (_isDragging && Input.GetMouseButtonUp(0))
        {
            StartCoroutine(FlingBackToInitialPosition());
            _isDragging = false;
        }

        Vector2 m_currentPosition = transform.position;
        SetPosition(m_currentPosition);
    }

    private void OnMouseDrag()
    {
        _isDragging = true;
        Vector2 m_mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SetPosition(m_mousePosition);
    }

    private void SetPosition(Vector2 targetPosition)
    {
        if (IsPositionOutOfPullRange(targetPosition))
        {
            transform.position = ReturnNormalisedPosition(targetPosition);
            return;
        }

        transform.position = targetPosition;
    }

    private bool IsPositionOutOfPullRange(Vector2 targetPosition)
    {
        return Vector2.Distance(targetPosition, _initialPosition) > _pullDistance;
    }

    private Vector2 ReturnNormalisedPosition(Vector2 targetPosition)
    {
        return _initialPosition + ((targetPosition - _initialPosition).normalized * _pullDistance);
    }

    IEnumerator FlingBackToInitialPosition()
    {
        _collider.enabled = false;
        Vector2 _releasePosition = transform.position;
        Vector2 _mirroredPosition = _initialPosition + (_initialPosition - _releasePosition);
        OnFling?.Invoke(_releasePosition - _initialPosition);
        OnWebFling?.Invoke();
        float m_time = 0.0f;
        while(m_time <= 1.0f)
        {
            transform.position = Vector2.Lerp(_releasePosition, _mirroredPosition, _resetCurve.Evaluate(m_time));
            m_time += Time.deltaTime / _resetDuration;
            yield return null;
        }

        transform.position = _initialPosition;
        _collider.enabled = true;
    }

    public WebPoint[] ReturnConnectedPoints()
    {
        return connectedWebPoints.ToArray();
    }
}
