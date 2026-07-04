using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

/// <summary>
/// Gerencia o estado do jogo e exibe o painel de fim de jogo.
///
/// Setup:
///  1. Crie um GameObject vazio chamado "GameManager" na cena.
///  2. Adicione este script nele.
///  3. Crie o painel de UI conforme as instruções abaixo.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ----------------------------------------------------------------
    // Singleton
    // ----------------------------------------------------------------
    public static GameManager Instance { get; private set; }

    // ----------------------------------------------------------------
    // Inspector
    // ----------------------------------------------------------------

    [Header("UI - Painel de Fim de Jogo")]
    [Tooltip("O painel 'ACABOU' — arraste o objeto Canvas/Panel aqui.")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("Nome exato da cena para reiniciar (confira em File → Build Settings).")]
    [SerializeField] private string sceneName = "SampleScene";

    [Header("UI - HUD")]
    [Tooltip("Texto da HUD com a contagem de Data Cores. Se vazio, o script tenta encontrar sozinho.")]
    [SerializeField] private TMP_Text scoreText;

    [Tooltip("Título opcional do painel final. Se existir, alterna entre vitória e aviso.")]
    [SerializeField] private TMP_Text resultTitleText;

    [Tooltip("Subtexto opcional do painel final.")]
    [SerializeField] private TMP_Text resultBodyText;

    // ----------------------------------------------------------------
    // Privado
    // ----------------------------------------------------------------
    private int _totalCollectibles;
    private int _collectedCount;
    private bool _configuredAtRuntime;
    private bool _hasGameEnded;

    public event Action<int, int> RequiredCollectiblesChanged;
    public event Action<bool> GameEnded;

    public int TotalRequiredCollectibles => _totalCollectibles;
    public int CollectedRequiredCount => _collectedCount;
    public bool AllRequiredCollected => _totalCollectibles <= 0 || _collectedCount >= _totalCollectibles;
    public bool HasGameEnded => _hasGameEnded;

    // ----------------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        RebuildSceneState();
    }

    private void Start()
    {
        // Garante que configurações aplicadas por bootstrap em runtime também sejam refletidas.
        if (_configuredAtRuntime)
            RebuildSceneState();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // ----------------------------------------------------------------
    // API pública
    // ----------------------------------------------------------------

    /// <summary>
    /// Chamado pelo Collectible quando um item é coletado.
    /// Quando o último for coletado, exibe o painel de fim de jogo.
    /// </summary>
    public void OnItemCollected()
    {
        OnItemCollected(CollectibleType.DataCore);
    }

    public void OnItemCollected(CollectibleType type)
    {
        if (_hasGameEnded || type == CollectibleType.Polvina)
        {
            RequiredCollectiblesChanged?.Invoke(_collectedCount, _totalCollectibles);
            return;
        }

        _collectedCount = Mathf.Min(_collectedCount + 1, _totalCollectibles);
        RefreshHud();
        RequiredCollectiblesChanged?.Invoke(_collectedCount, _totalCollectibles);

        if (_totalCollectibles > 0 && _collectedCount >= _totalCollectibles)
            ShowEndState(true, "ESCAPOU!", "Todos os Data Cores foram coletados.");
    }

    /// <summary>Reinicia a cena atual.</summary>
    public void RestartGame()
    {
        Time.timeScale = 1f; // garante que o tempo não ficou pausado
        string targetScene = string.IsNullOrWhiteSpace(sceneName)
            ? SceneManager.GetActiveScene().name
            : sceneName;
        SceneManager.LoadScene(targetScene);
    }

    // ----------------------------------------------------------------
    // Privado
    // ----------------------------------------------------------------

    public void Configure(
        GameObject panel,
        string targetSceneName,
        TMP_Text hudText = null,
        TMP_Text titleText = null,
        TMP_Text bodyText = null)
    {
        gameOverPanel = panel;
        sceneName = targetSceneName;
        scoreText = hudText;
        resultTitleText = titleText;
        resultBodyText = bodyText;
        _configuredAtRuntime = true;
    }

    public void RefreshSceneState()
    {
        RebuildSceneState();
    }

    public bool TryCompleteLevelFromPortal()
    {
        if (_hasGameEnded)
            return true;

        if (!AllRequiredCollected)
        {
            Debug.Log("[GameManager] Portal bloqueado — faltam itens obrigatórios.");
            return false;
        }

        ShowEndState(true, "ESCAPOU!", "Blink alcançou o portal de fuga.");
        return true;
    }

    public void OnPlayerDeath()
    {
        if (_hasGameEnded)
            return;

        ShowEndState(false, "SINAL PERDIDO", "Blink foi deletado antes de escapar.");
    }

    private void ShowEndState(bool victory, string title, string body)
    {
        _hasGameEnded = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (resultTitleText != null)
            resultTitleText.text = title;

        if (resultBodyText != null)
            resultBodyText.text = body;

        // Pausa o jogo enquanto o painel está visível
        Time.timeScale = 0f;

        if (victory)
            ESP32SerialReader.Instance?.SendTreasure();
        else
            ESP32SerialReader.Instance?.SendDamage();

        GameEnded?.Invoke(victory);
        Debug.Log(victory
            ? "[GameManager] Vitória concluída."
            : "[GameManager] Game over por perda de vidas.");
    }

    private void RefreshHud()
    {
        if (scoreText == null)
            return;

        scoreText.text = $"Data Cores: {_collectedCount}/{_totalCollectibles}";
    }

    private TMP_Text FindHudText()
    {
        TMP_Text[] texts = FindObjectsByType<TMP_Text>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (TMP_Text text in texts)
        {
            if (text == null || !text.gameObject.activeInHierarchy)
                continue;

            string objectName = text.gameObject.name.ToLowerInvariant();
            string currentText = text.text.ToLowerInvariant();

            if (objectName.Contains("score") || currentText.Contains("data core") || currentText.Contains("/"))
                return text;
        }

        return null;
    }

    private void RebuildSceneState()
    {
        Collectible[] collectibles = FindObjectsByType<Collectible>(FindObjectsSortMode.None);
        _totalCollectibles = 0;
        foreach (Collectible collectible in collectibles)
        {
            if (collectible != null && collectible.CountsForVictory)
                _totalCollectibles++;
        }

        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (scoreText == null)
            scoreText = FindHudText();

        if (resultTitleText == null && gameOverPanel != null)
            resultTitleText = gameOverPanel.GetComponentInChildren<TMP_Text>(true);

        _hasGameEnded = false;
        RefreshHud();
        RequiredCollectiblesChanged?.Invoke(_collectedCount, _totalCollectibles);
    }
}
