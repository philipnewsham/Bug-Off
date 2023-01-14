using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private Level[] _levels;
    private int _levelCount;
    private int _spiderCount;
    private int _spiderDefeatedCount = 0;
    [SerializeField] private float _levelResetDelay = 3.0f;
    [SerializeField] private GenerateLevelFromData _generateLevelFromData;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _backgroundTransform;

    private void OnEnable()
    {
        Spider.OnSpiderDeath += OnSpiderDeath;
        Fly.OnFlyDeath += OnFlyDeath;
    }

    private void Awake()
    {
        _levelCount = _levels.Length;
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("LevelIndex"))
        {
            PlayerPrefs.SetInt("LevelIndex", 0);
        }

        Level m_currentLevelData = _levels[PlayerPrefs.GetInt("LevelIndex")];
        _generateLevelFromData.GenerateLevel(m_currentLevelData);
        _spiderCount = m_currentLevelData.spiderIndexes.Length;
        _spiderDefeatedCount = 0;
        _camera.orthographicSize = m_currentLevelData.cameraSize;
        _backgroundTransform.localScale = Vector2.one * _camera.orthographicSize;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            IncreaseLevelIndex();
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DecreaseLevelIndex();
            SceneManager.LoadScene(0);
        }
    }

    private void OnSpiderDeath()
    {
        _spiderDefeatedCount++;
        if (_spiderDefeatedCount >= _spiderCount)
        {
            IncreaseLevelIndex();
            StartCoroutine(ResetLevel());
        }
    }

    void IncreaseLevelIndex()
    {
        int m_levelIndex = PlayerPrefs.GetInt("LevelIndex");
        m_levelIndex = (m_levelIndex + 1) % _levelCount;
        PlayerPrefs.SetInt("LevelIndex", m_levelIndex);
    }

    void DecreaseLevelIndex()
    {
        int m_levelIndex = PlayerPrefs.GetInt("LevelIndex");
        m_levelIndex = (m_levelIndex - 1);
        if(m_levelIndex == -1)
        {
            m_levelIndex = _levelCount - 1;
        }
        PlayerPrefs.SetInt("LevelIndex", m_levelIndex);
    }

    private void OnFlyDeath()
    {
        StartCoroutine(ResetLevel());
    }

    IEnumerator ResetLevel()
    {
        yield return new WaitForSeconds(_levelResetDelay);
        SceneManager.LoadScene(0);
    }

    private void OnDisable()
    {
        Spider.OnSpiderDeath -= OnSpiderDeath;
        Fly.OnFlyDeath -= OnFlyDeath;
    }
}
