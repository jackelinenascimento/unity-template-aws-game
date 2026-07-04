using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// Consolida a cena em runtime quando o template ainda não foi completamente montado no Editor.
/// Cria HUD, GameManager, coletáveis e obstáculos mínimos necessários para a demo.
/// </summary>
[DefaultExecutionOrder(-1000)]
public class SceneAutoSetup : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite collectibleSprite;
    [SerializeField] private Sprite polvinaSprite;
    [SerializeField] private Sprite laserSprite;
    [SerializeField] private Sprite droneSprite;
    [SerializeField] private Sprite acidSprite;

    [Header("Cenario")]
    [SerializeField] private GameObject laboratoryBackdropPrefab;
    [SerializeField] private Vector3 backdropPosition = new Vector3(36f, 7f, 0f);
    [SerializeField] private Vector3 backdropScale = new Vector3(3.2f, 3.2f, 1f);

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;

    private static readonly Vector3[] CollectiblePositions =
    {
        new Vector3(12f, 1.2f, 0f),
        new Vector3(34f, 1.6f, 0f),
        new Vector3(60f, 2.1f, 0f),
        new Vector3(48f, 3.2f, 0f)
    };

    private static readonly CollectibleType[] CollectibleTypes =
    {
        CollectibleType.PenDrive,
        CollectibleType.DataCore,
        CollectibleType.DataCore,
        CollectibleType.Polvina
    };

    private static readonly Vector3[] ObstaclePositions =
    {
        new Vector3(20f, -0.3f, 0f),
        new Vector3(43f, 0.2f, 0f),
        new Vector3(68f, -0.5f, 0f)
    };

    private void Awake()
    {
        EnsureEnvironmentArt();
        EnsurePlayableDefaults();
        TMP_Text scoreText = EnsureHud();
        HealthSystem healthSystem = EnsureHealthSystem();
        EnsureCollectibles();
        EnsureObstacles();
        EnsureGameManager(scoreText);
        EnsureHudManager(scoreText, healthSystem);
    }

    private void EnsureEnvironmentArt()
    {
        if (laboratoryBackdropPrefab == null)
            return;

        if (GameObject.Find("CenarioFuturistaAuto") != null ||
            GameObject.Find("Cenário Futurista") != null ||
            GameObject.Find("Cenario Futurista") != null)
        {
            return;
        }

        GameObject backdrop = Instantiate(laboratoryBackdropPrefab, backdropPosition, Quaternion.identity);
        backdrop.name = "CenarioFuturistaAuto";
        backdrop.transform.localScale = backdropScale;

        foreach (Collider2D collider in backdrop.GetComponentsInChildren<Collider2D>(true))
            collider.enabled = false;

        foreach (TilemapRenderer renderer in backdrop.GetComponentsInChildren<TilemapRenderer>(true))
            renderer.sortingOrder = -20;
    }

    private void EnsurePlayableDefaults()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null)
            return;

        // A cena de demonstração deve abrir jogável mesmo sem o hardware conectado.
        player.SetKeyboardFallback(true);

        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private TMP_Text EnsureHud()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return null;

        foreach (TMP_Text text in canvas.GetComponentsInChildren<TMP_Text>(true))
        {
            if (text.gameObject.name == "ScoreText")
                return text;
        }

        TMP_Text template = FindFirstObjectByType<TMP_Text>(FindObjectsInactive.Include);
        GameObject scoreObject = new GameObject("ScoreText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        RectTransform rect = scoreObject.GetComponent<RectTransform>();
        rect.SetParent(canvas.transform, false);
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(20f, -20f);
        rect.sizeDelta = new Vector2(320f, 50f);

        TMP_Text scoreText = scoreObject.GetComponent<TextMeshProUGUI>();
        scoreText.text = "Data Cores: 0/0";
        scoreText.fontSize = 28f;
        scoreText.color = Color.white;

        if (template != null)
        {
            scoreText.font = template.font;
            scoreText.fontSharedMaterial = template.fontSharedMaterial;
            scoreText.alignment = TextAlignmentOptions.TopLeft;
        }

        return scoreText;
    }

    private void EnsureCollectibles()
    {
        Collectible[] existing = FindObjectsByType<Collectible>(FindObjectsSortMode.None);
        for (int i = 0; i < existing.Length && i < CollectibleTypes.Length; i++)
            existing[i].SetType(CollectibleTypes[i]);

        for (int i = existing.Length; i < CollectiblePositions.Length; i++)
            CreateCollectible(i, CollectiblePositions[i]);
    }

    private void EnsureObstacles()
    {
        Obstacle[] existing = FindObjectsByType<Obstacle>(FindObjectsSortMode.None);
        for (int i = existing.Length; i < ObstaclePositions.Length; i++)
            CreateObstacle(i);
    }

    private void EnsureGameManager(TMP_Text scoreText)
    {
        GameManager manager = FindFirstObjectByType<GameManager>();
        if (manager == null)
        {
            GameObject managerObject = new GameObject("GameManager");
            manager = managerObject.AddComponent<GameManager>();
        }

        TMP_Text titleText = null;
        if (gameOverPanel != null)
            titleText = gameOverPanel.GetComponentInChildren<TMP_Text>(true);

        manager.Configure(gameOverPanel, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, scoreText, titleText, null);
        manager.RefreshSceneState();
    }

    private void EnsureHudManager(TMP_Text scoreText, HealthSystem healthSystem)
    {
        HUDManager hudManager = FindFirstObjectByType<HUDManager>();
        if (hudManager == null)
        {
            GameObject hudObject = new GameObject("HUDManager");
            hudManager = hudObject.AddComponent<HUDManager>();
        }

        hudManager.Configure(polvinaSprite, scoreText, EnsureHeartsContainer());
        hudManager.Bind(healthSystem, GameManager.Instance);
    }

    private void CreateCollectible(int index, Vector3 position)
    {
        CollectibleType type = CollectibleTypes[index];
        GameObject item = new GameObject(GetCollectibleName(index, type), typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Collectible));
        item.transform.position = position;
        item.transform.localScale = type == CollectibleType.Polvina ? new Vector3(0.9f, 0.9f, 1f) : new Vector3(0.72f, 0.72f, 1f);

        SpriteRenderer renderer = item.GetComponent<SpriteRenderer>();
        renderer.sprite = type == CollectibleType.Polvina ? polvinaSprite : collectibleSprite;
        renderer.sortingOrder = 8;

        CircleCollider2D collider = item.GetComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.45f;

        Collectible collectible = item.GetComponent<Collectible>();
        collectible.SetType(type);
    }

    private void CreateObstacle(int index)
    {
        string objectName;
        Sprite sprite;
        Vector2 size;

        switch (index)
        {
            case 0:
                objectName = "LaserSeguranca";
                sprite = laserSprite;
                size = new Vector2(1.2f, 2.4f);
                break;
            case 1:
                objectName = "DronePatrulha";
                sprite = droneSprite;
                size = new Vector2(1.6f, 1.2f);
                break;
            default:
                objectName = "AcidoFluorescente";
                sprite = acidSprite;
                size = new Vector2(2f, 0.8f);
                break;
        }

        GameObject obstacle = new GameObject(objectName, typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Obstacle));
        obstacle.transform.position = ObstaclePositions[index];

        SpriteRenderer renderer = obstacle.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 7;

        BoxCollider2D collider = obstacle.GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = size;
    }

    private static string GetCollectibleName(int index, CollectibleType type)
    {
        return type switch
        {
            CollectibleType.PenDrive => $"PenDrive_{index + 1:00}",
            CollectibleType.Polvina => $"Polvina_{index + 1:00}",
            _ => $"DataCore_{index + 1:00}"
        };
    }

    private HealthSystem EnsureHealthSystem()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null)
            return null;

        HealthSystem health = player.GetComponent<HealthSystem>();
        if (health == null)
            health = player.gameObject.AddComponent<HealthSystem>();

        return health;
    }

    private Transform EnsureHeartsContainer()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return null;

        Transform existing = canvas.transform.Find("HeartsContainer");
        if (existing != null)
            return existing;

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

        return rect;
    }
}
