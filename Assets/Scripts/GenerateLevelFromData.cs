using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevelFromData : MonoBehaviour
{
    [SerializeField] private WebPoint _webPointPrefab;
    [SerializeField] private WebLine _webLinePrefab;
    [SerializeField] private Fly _flyPrefab;
    [SerializeField] private Spider _spiderPrefab;
    private List<WebPoint> _spawnedWebPoints = new List<WebPoint>();
    private List<WebLine> _spawnedWebLines = new List<WebLine>();
    private Web _web;

    public void GenerateLevel(Level levelData)
    {
        GeneratePoints(levelData);
        GenerateLines(levelData);
        ConnectPoints(levelData);
        _web = new Web(_spawnedWebPoints.ToArray(), _spawnedWebLines.ToArray());
        Instantiate(_flyPrefab, _spawnedWebPoints[levelData.flyIndex].transform.position, Quaternion.identity);
        for (int i = 0; i < levelData.spiderIndexes.Length; i++)
        {
            Spider m_spider = Instantiate(_spiderPrefab, _spawnedWebPoints[levelData.spiderIndexes[i]].transform.position, Quaternion.identity);
            m_spider.SetSpawnDetails(_spawnedWebPoints[levelData.spiderIndexes[i]], _spawnedWebPoints[levelData.flyIndex], _web);
        }
    }

    void GeneratePoints(Level levelData)
    {
        for (int i = 0; i < levelData.webPoints.Length; i++)
        {
            _spawnedWebPoints.Add(Instantiate(_webPointPrefab, levelData.webPoints[i], Quaternion.identity));
        }
    }

    void GenerateLines(Level levelData)
    {
        for (int i = 0; i < levelData.webLinesA.Length; i++)
        {
            WebLine m_webLine = Instantiate(_webLinePrefab, null);
            m_webLine.SetTargetTransforms(_spawnedWebPoints[levelData.webLinesA[i]].transform, _spawnedWebPoints[levelData.webLinesB[i]].transform);
            _spawnedWebLines.Add(m_webLine);
        }
    }

    void ConnectPoints(Level levelData)
    {
        for (int i = 0; i < levelData.webLinesA.Length; i++)
        {
            _spawnedWebPoints[levelData.webLinesA[i]].connectedWebPoints.Add(_spawnedWebPoints[levelData.webLinesB[i]]);
            _spawnedWebPoints[levelData.webLinesB[i]].connectedWebPoints.Add(_spawnedWebPoints[levelData.webLinesA[i]]);
        }
    }
}
