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
    [SerializeField] private Sprite penDriveSprite;
    [SerializeField] private Sprite dataCoreSprite;
    [SerializeField] private Sprite polvinaSprite;
    [SerializeField] private Sprite laserSprite;
    [SerializeField] private Sprite droneSprite;
    [SerializeField] private Sprite acidSprite;

    [Header("Cenario")]
    [SerializeField] private GameObject laboratoryBackdropPrefab;
    [SerializeField] private GameObject groundLayoutPrefab;
    [SerializeField] private Vector3 backdropPosition = new Vector3(36f, 7f, 0f);
    [SerializeField] private Vector3 backdropScale = new Vector3(3.2f, 3.2f, 1f);
    [SerializeField] private Vector3 groundLayoutPosition = new Vector3(18f, -2.5f, 0f);
    [SerializeField] private Vector3 groundLayoutScale = new Vector3(4.8f, 1.8f, 1f);

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
        ResolveDefaultSprites();
        EnsureEnvironmentArt();
        EnsurePlayableDefaults();
        TMP_Text scoreText = EnsureHud();
        TMP_Text timerText = EnsureTimerText();
        TMP_Text statusText = EnsureStatusText();
        HealthSystem healthSystem = EnsureHealthSystem();
        EnsureCollectibles();
        EnsureObstacles();
        EnsureGameManager(scoreText);
        EnsureHudManager(scoreText, timerText, statusText, healthSystem);
    }

    private void ResolveDefaultSprites()
    {
        if (penDriveSprite == null)
            penDriveSprite = LoadSpriteFromResources("Sprites/drive");

        if (dataCoreSprite == null)
            dataCoreSprite = LoadSpriteFromResources("Sprites/chip");

        if (collectibleSprite == null)
            collectibleSprite = dataCoreSprite != null ? dataCoreSprite : LoadSpriteFromResources("Sprites/chipdrive");
    }

    private void EnsureEnvironmentArt()
    {
        if (laboratoryBackdropPrefab == null)
            return;

        if (GameObject.Find("CenarioFuturistaAuto") != null ||
            GameObject.Find("Cenário Futurista") != null ||
            GameObject.Find("Cenario Futurista") != null)
        {
            EnsureGroundLayout();
            return;
        }

        GameObject backdrop = Instantiate(laboratoryBackdropPrefab, backdropPosition, Quaternion.identity);
        backdrop.name = "CenarioFuturistaAuto";
        backdrop.transform.localScale = backdropScale;

        foreach (Collider2D collider in backdrop.GetComponentsInChildren<Collider2D>(true))
            collider.enabled = false;

        foreach (TilemapRenderer renderer in backdrop.GetComponentsInChildren<TilemapRenderer>(true))
            renderer.sortingOrder = -20;

        EnsureGroundLayout();
    }

    private void EnsureGroundLayout()
    {
        if (groundLayoutPrefab == null)
            return;

        if (HasPlayableGround())
            return;

        GameObject groundLayout = Instantiate(groundLayoutPrefab, groundLayoutPosition, Quaternion.identity);
        groundLayout.name = "ChaoAuto";
        groundLayout.transform.localScale = groundLayoutScale;

        int groundLayer = LayerMask.NameToLayer("Ground");
        foreach (Tilemap tilemap in groundLayout.GetComponentsInChildren<Tilemap>(true))
        {
            if (groundLayer >= 0)
                tilemap.gameObject.layer = groundLayer;

            TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
            if (renderer != null)
                renderer.sortingOrder = -5;

            TilemapCollider2D tilemapCollider = tilemap.GetComponent<TilemapCollider2D>();
            if (tilemapCollider == null)
                tilemapCollider = tilemap.gameObject.AddComponent<TilemapCollider2D>();

            tilemapCollider.usedByComposite = false;
        }
    }

    private static bool HasPlayableGround()
    {
        Tilemap[] tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap == null)
                continue;

            if (tilemap.GetComponent<Collider2D>() != null)
                return true;
        }

        GameObject[] roots = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject root in roots)
        {
            if (root == null)
                continue;

            string name = root.name.ToLowerInvariant();
            if (!name.Contains("ground") && !name.Contains("chao") && !name.Contains("tilemap"))
                continue;

            if (root.GetComponent<Collider2D>() != null)
                return true;
        }

        return false;
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

    private void EnsureHudManager(TMP_Text scoreText, TMP_Text timerText, TMP_Text statusText, HealthSystem healthSystem)
    {
        HUDManager hudManager = FindFirstObjectByType<HUDManager>();
        if (hudManager == null)
        {
            GameObject hudObject = new GameObject("HUDManager");
            hudManager = hudObject.AddComponent<HUDManager>();
        }

        hudManager.Configure(polvinaSprite, scoreText, EnsureHeartsContainer(), timerText, statusText);
        hudManager.Bind(healthSystem, GameManager.Instance);
    }

    private TMP_Text EnsureTimerText()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return null;

        Transform existing = canvas.transform.Find("TimerText");
        if (existing != null)
            return existing.GetComponent<TMP_Text>();

        TMP_Text template = FindFirstObjectByType<TMP_Text>(FindObjectsInactive.Include);
        GameObject timerObject = new GameObject("TimerText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        RectTransform rect = timerObject.GetComponent<RectTransform>();
        rect.SetParent(canvas.transform, false);
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -20f);
        rect.sizeDelta = new Vector2(260f, 44f);

        TMP_Text timerText = timerObject.GetComponent<TextMeshProUGUI>();
        timerText.text = "Limpeza: 02:00";
        timerText.fontSize = 26f;
        timerText.color = Color.white;

        if (template != null)
        {
            timerText.font = template.font;
            timerText.fontSharedMaterial = template.fontSharedMaterial;
            timerText.alignment = TextAlignmentOptions.Top;
        }

        return timerText;
    }

    private TMP_Text EnsureStatusText()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return null;

        Transform existing = canvas.transform.Find("StatusText");
        if (existing != null)
            return existing.GetComponent<TMP_Text>();

        TMP_Text template = FindFirstObjectByType<TMP_Text>(FindObjectsInactive.Include);
        GameObject statusObject = new GameObject("StatusText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        RectTransform rect = statusObject.GetComponent<RectTransform>();
        rect.SetParent(canvas.transform, false);
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -56f);
        rect.sizeDelta = new Vector2(420f, 36f);

        TMP_Text statusText = statusObject.GetComponent<TextMeshProUGUI>();
        statusText.text = "Mainframe bloqueado";
        statusText.fontSize = 20f;
        statusText.color = new Color(0.36f, 0.93f, 1f);

        if (template != null)
        {
            statusText.font = template.font;
            statusText.fontSharedMaterial = template.fontSharedMaterial;
            statusText.alignment = TextAlignmentOptions.Top;
        }

        return statusText;
    }

    private void CreateCollectible(int index, Vector3 position)
    {
        CollectibleType type = CollectibleTypes[index];
        GameObject item = new GameObject(GetCollectibleName(index, type), typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Collectible));
        item.transform.position = position;
        item.transform.localScale = type == CollectibleType.Polvina ? new Vector3(0.9f, 0.9f, 1f) : new Vector3(0.72f, 0.72f, 1f);

        SpriteRenderer renderer = item.GetComponent<SpriteRenderer>();
        renderer.sprite = GetCollectibleSprite(type);
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

    private Sprite GetCollectibleSprite(CollectibleType type)
    {
        return type switch
        {
            CollectibleType.PenDrive => penDriveSprite != null ? penDriveSprite : collectibleSprite,
            CollectibleType.Polvina => polvinaSprite,
            _ => dataCoreSprite != null ? dataCoreSprite : collectibleSprite
        };
    }

    private static Sprite LoadSpriteFromResources(string resourcePath)
    {
        Sprite sprite = Resources.Load<Sprite>(resourcePath);
        if (sprite != null)
            return sprite;

        Texture2D texture = Resources.Load<Texture2D>(resourcePath);
        if (texture == null)
            return null;

        return Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f);
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
