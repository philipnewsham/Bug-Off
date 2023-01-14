using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebLine : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;

    public void SetTargetTransforms(Transform pointA, Transform pointB)
    {
        _pointA = pointA;
        _pointB = pointB;
        SetPoints();
    }

    private void Update()
    {
        SetPoints();
    }

    void SetPoints()
    {
        _lineRenderer.SetPosition(0, _pointA.transform.position);
        _lineRenderer.SetPosition(1, _pointB.transform.position);
    }
}
