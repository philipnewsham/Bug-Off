using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GenerateLevelSystem : MonoBehaviour
{
#if UNITY_EDITOR
    public enum GenerationMode
    {
        SettingPoints,
        LinkingPoints,
        SetFly,
        SetSpider
    }

    private GenerationMode _currentMode = GenerationMode.SettingPoints;
    [SerializeField] private WebPointLevelGeneration _webPointPrefab;
    [SerializeField] private WebLine _webLinePrefab;
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _layerMask;
    private List<WebPointLevelGeneration> _webPoints = new List<WebPointLevelGeneration>();
    private List<WebLine> _webLines = new List<WebLine>();

    private WebPointLevelGeneration _webPointA = null;
    private WebPointLevelGeneration _webPointB = null;

    private WebPointLevelGeneration _flyWebPoint = null;
    private List<WebPointLevelGeneration> _spiderWebPoints = new List<WebPointLevelGeneration>();
    [SerializeField] private InputField _levelNameInputField;
    private List<(WebPointLevelGeneration, WebPointLevelGeneration)> _connectedPoints = new List<(WebPointLevelGeneration, WebPointLevelGeneration)>();

    private void OnEnable()
    {
        WebPointLevelGeneration.OnWebPointClicked += OnWebPointClicked;
    }

    private void OnWebPointClicked(WebPointLevelGeneration webPoint)
    {
        if (_currentMode == GenerationMode.SettingPoints)
        {
            webPoint.gameObject.SetActive(false);
            return;
        }

        if(_currentMode == GenerationMode.LinkingPoints)
        {
            UpdateConnections(webPoint);
            return;
        }

        if(_currentMode == GenerationMode.SetFly)
        {
            SetFly(webPoint);
            return;
        }

        if(_currentMode == GenerationMode.SetSpider)
        {
            SetSpider(webPoint);
            return;
        }
    }

    void UpdateConnections(WebPointLevelGeneration webPoint)
    {
        if (_webPointA == null)
        {
            _webPointA = webPoint;
            webPoint.Highlight(true);
            return;
        }

        if (_webPointA == webPoint)
        {
            _webPointA = null;
            webPoint.Highlight(false);
            return;
        }

        if (_webPointB == null)
        {
            _webPointB = webPoint;
            _webPointB.Highlight(true);
            LinkPoints();
            _webPointA.Highlight(false);
            _webPointB.Highlight(false);
            _webPointA = null;
            _webPointB = null;
            return;
        }
    }

    void SetFly(WebPointLevelGeneration webPoint)
    {
        if (_spiderWebPoints.Contains(webPoint))
        {
            return;
        }

        if (_flyWebPoint != null)
        {
            _flyWebPoint.SetFly(false);

            if (_flyWebPoint == webPoint)
            {
                return;
            }
        }

        _flyWebPoint = webPoint;
        webPoint.SetFly(true);
    }

    void SetSpider(WebPointLevelGeneration webPoint)
    {
        if(_flyWebPoint == webPoint)
        {
            return;
        }

        if (_spiderWebPoints.Contains(webPoint))
        {
            _spiderWebPoints.Remove(webPoint);
            webPoint.SetSpider(false);
            return;
        }

        _spiderWebPoints.Add(webPoint);
        webPoint.SetSpider(true);
    }

    public void SetMode(int mode)
    {
        _currentMode = (GenerationMode)mode;
    }

    void LinkPoints()
    {
        WebLine m_webLine = Instantiate(_webLinePrefab);
        _webLines.Add(m_webLine);
        _connectedPoints.Add((_webPointA, _webPointB));
        m_webLine.SetTargetTransforms(_webPointA.transform, _webPointB.transform);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _currentMode == GenerationMode.SettingPoints)
        {
            Vector3 m_spawnPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            m_spawnPosition.z = 0.0f;

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (Physics2D.Raycast(m_spawnPosition, Vector3.forward, 100.0f, _layerMask.value))
            {
                return;
            }

            GenerateWebPoint(m_spawnPosition);
        }

        _camera.orthographicSize = Mathf.Max(1, _camera.orthographicSize + Input.mouseScrollDelta.y);
        if (Input.GetKeyDown(KeyCode.Home))
        {
            _camera.orthographicSize = 5;
        }
    }

    void GenerateWebPoint(Vector3 spawnPosition)
    {
        for (int i = 0; i < _webPoints.Count; i++)
        {
            if (!_webPoints[i].gameObject.activeSelf)
            {
                _webPoints[i].transform.position = spawnPosition;
                _webPoints[i].gameObject.SetActive(true);
                return;
            }
        }

        _webPoints.Add(Instantiate(_webPointPrefab, spawnPosition, Quaternion.identity));
    } 

    public void SaveLevel()
    {
        Level m_level = ScriptableObject.CreateInstance<Level>();
        List<WebPointLevelGeneration> m_activeWebPoints = new List<WebPointLevelGeneration>();
        List<Vector2> m_webPositions = new List<Vector2>();
        for (int i = 0; i < _webPoints.Count; i++)
        {
            if (_webPoints[i].gameObject.activeSelf)
            {
                m_activeWebPoints.Add(_webPoints[i]);
            }
        }

        for (int i = 0; i < m_activeWebPoints.Count; i++)
        {
            m_webPositions.Add(m_activeWebPoints[i].transform.position);
        }

        m_level.webPoints = m_webPositions.ToArray();

        m_level.flyIndex = -1;
        for (int i = 0; i < m_activeWebPoints.Count; i++)
        {
            if(m_activeWebPoints[i] == _flyWebPoint)
            {
                m_level.flyIndex = i;
                break;
            }
        }

        m_level.spiderIndexes = new int[_spiderWebPoints.Count];
        for (int i = 0; i < _spiderWebPoints.Count; i++)
        {
            for (int j = 0; j < m_activeWebPoints.Count; j++)
            {
                if(m_activeWebPoints[j] == _spiderWebPoints[i])
                {
                    m_level.spiderIndexes[i] = j;
                }
            }
        }

        m_level.webLinesA = new int[_webLines.Count];
        m_level.webLinesB = new int[_webLines.Count];
        for (int i = 0; i < _connectedPoints.Count; i++)
        {
            int pointA = -1;
            for (int j = 0; j < m_activeWebPoints.Count; j++)
            {
                if(_connectedPoints[i].Item1 == m_activeWebPoints[j])
                {
                    pointA = j;
                }
            }

            int pointB = -1;
            for (int j = 0; j < m_activeWebPoints.Count; j++)
            {
                if (_connectedPoints[i].Item2 == m_activeWebPoints[j])
                {
                    pointB = j;
                }
            }

            m_level.webLinesA[i] = pointA;
            m_level.webLinesB[i] = pointB;
        }

        m_level.cameraSize = _camera.orthographicSize;
        // path has to start at "Assets"
        string path = string.Format("Assets/Levels/{0}.asset", _levelNameInputField.text);
        AssetDatabase.CreateAsset(m_level, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = m_level;
    }

    private void OnDisable()
    {
        WebPointLevelGeneration.OnWebPointClicked -= OnWebPointClicked;
    }
#endif
}
