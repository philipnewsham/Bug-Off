using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWebLines : MonoBehaviour
{
    [SerializeField] private float _lineDistance = 5.5f;
    [SerializeField] private WebPoint[] webPoints;
    [SerializeField] private WebLine _webLinePrefab;
    public Web web;

    private void Start()
    {
        GenerateLines();
    }

    [ContextMenu("GenerateWeb")]
    public void GenerateLines()
    {
        for (int i = 0; i < webPoints.Length - 1; i++)
        {
            for (int j = i + 1; j < webPoints.Length; j++)
            {
                if (Vector2.Distance(webPoints[i].transform.position, webPoints[j].transform.position) <= _lineDistance)
                {
                    webPoints[i].connectedWebPoints.Add(webPoints[j]);
                    webPoints[j].connectedWebPoints.Add(webPoints[i]);
                    Instantiate(_webLinePrefab, null).SetTargetTransforms(webPoints[i].transform, webPoints[j].transform);
                }
            }
        }

        web.webPoints = webPoints;
    }
}
