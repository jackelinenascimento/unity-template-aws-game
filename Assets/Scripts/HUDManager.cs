using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Centraliza a HUD de pontuação e vidas.
/// </summary>
public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Polvinas")]
    [SerializeField] private Transform heartsContainer;
    [SerializeField] private Sprite polvinaIcon;
    [SerializeField] private Vector2 heartSize = new Vector2(36f, 36f);

    private readonly List<Image> _hearts = new List<Image>();
    private HealthSystem _healthSystem;
    private GameManager _gameManager;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Configure(Sprite icon, TMP_Text scoreOverride = null, Transform heartsParent = null)
    {
        if (icon != null)
            polvinaIcon = icon;

        if (scoreOverride != null)
            scoreText = scoreOverride;

        if (heartsParent != null)
            heartsContainer = heartsParent;
    }

    public void Bind(HealthSystem healthSystem, GameManager gameManager)
    {
        if (_healthSystem != null)
            _healthSystem.LivesChanged -= OnLivesChanged;

        if (_gameManager != null)
            _gameManager.RequiredCollectiblesChanged -= OnScoreChanged;

        _healthSystem = healthSystem;
        _gameManager = gameManager;

        if (_healthSystem != null)
        {
            _healthSystem.LivesChanged += OnLivesChanged;
            EnsureHeartsContainer();
            RebuildHearts(_healthSystem.MaxLives);
            PaintHearts(_healthSystem.CurrentLives);
        }

        if (_gameManager != null)
        {
            _gameManager.RequiredCollectiblesChanged += OnScoreChanged;
            EnsureScoreText();
            OnScoreChanged(_gameManager.CollectedRequiredCount, _gameManager.TotalRequiredCollectibles);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        if (_healthSystem != null)
            _healthSystem.LivesChanged -= OnLivesChanged;

        if (_gameManager != null)
            _gameManager.RequiredCollectiblesChanged -= OnScoreChanged;
    }

    private void OnLivesChanged(int currentLives, int maxLives)
    {
        EnsureHeartsContainer();
        if (_hearts.Count != maxLives)
            RebuildHearts(maxLives);

        PaintHearts(currentLives);
    }

    private void OnScoreChanged(int current, int total)
    {
        EnsureScoreText();
        if (scoreText != null)
            scoreText.text = $"Data Cores: {current}/{total}";
    }

    private void EnsureScoreText()
    {
        if (scoreText != null)
            return;

        scoreText = FindFirstObjectByType<TMP_Text>();
    }

    private void EnsureHeartsContainer()
    {
        if (heartsContainer != null)
            return;

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return;

        GameObject container = new GameObject("HeartsContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        RectTransform rect = container.GetComponent<RectTransform>();
        rect.SetParent(canvas.transform, false);
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-20f, -20f);
        rect.sizeDelta = new Vector2(220f, 48f);

        HorizontalLayoutGroup layout = container.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = 8f;
        layout.childAlignment = TextAnchor.MiddleRight;
        layout.childControlHeight = false;
        layout.childControlWidth = false;

        heartsContainer = rect;
    }

    private void RebuildHearts(int maxLives)
    {
        if (heartsContainer == null)
            return;

        for (int i = heartsContainer.childCount - 1; i >= 0; i--)
        {
            GameObject child = heartsContainer.GetChild(i).gameObject;
            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }

        _hearts.Clear();

        for (int i = 0; i < maxLives; i++)
        {
            GameObject heart = new GameObject($"Polvina_{i + 1}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform rect = heart.GetComponent<RectTransform>();
            rect.SetParent(heartsContainer, false);
            rect.sizeDelta = heartSize;

            Image image = heart.GetComponent<Image>();
            image.sprite = polvinaIcon;
            image.preserveAspect = true;
            _hearts.Add(image);
        }
    }

    private void PaintHearts(int currentLives)
    {
        for (int i = 0; i < _hearts.Count; i++)
        {
            if (_hearts[i] == null)
                continue;

            _hearts[i].color = i < currentLives
                ? Color.white
                : new Color(1f, 1f, 1f, 0.18f);
        }
    }
}
