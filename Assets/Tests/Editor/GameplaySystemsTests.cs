using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySystemsTests
{
    private readonly List<GameObject> _roots = new List<GameObject>();

    [SetUp]
    public void SetUp()
    {
        Time.timeScale = 1f;
        _roots.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        for (int i = _roots.Count - 1; i >= 0; i--)
        {
            if (_roots[i] != null)
                Object.DestroyImmediate(_roots[i]);
        }

        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject gameObject in allObjects)
        {
            if (gameObject != null && gameObject.transform.parent == null)
                Object.DestroyImmediate(gameObject);
        }

        _roots.Clear();
        Time.timeScale = 1f;
    }

    [Test]
    public void GameManager_CountsOnlyRequiredCollectibles()
    {
        GameObject managerObject = CreateRoot("GameManager");
        GameManager manager = managerObject.AddComponent<GameManager>();

        CreateCollectible("Pendrive", CollectibleType.PenDrive);
        CreateCollectible("DataCore", CollectibleType.DataCore);
        CreateCollectible("Polvina", CollectibleType.Polvina);

        InvokePrivate(manager, "Awake");
        manager.RefreshSceneState();

        Assert.AreEqual(2, manager.TotalRequiredCollectibles);
        Assert.AreEqual(0, manager.CollectedRequiredCount);

        manager.OnItemCollected(CollectibleType.Polvina);
        Assert.AreEqual(0, manager.CollectedRequiredCount);

        manager.OnItemCollected(CollectibleType.PenDrive);
        Assert.AreEqual(1, manager.CollectedRequiredCount);
        Assert.IsFalse(manager.AllRequiredCollected);
    }

    [Test]
    public void HealthSystem_ClampsLivesAndTriggersDeath()
    {
        GameObject player = CreateRoot("Player");
        player.tag = "Player";
        player.AddComponent<SpriteRenderer>();

        HealthSystem health = player.AddComponent<HealthSystem>();
        SetPrivateField(health, "startingLives", 2);
        SetPrivateField(health, "maxLives", 3);
        SetPrivateField(health, "invincibilityDuration", 0f);
        InvokePrivate(health, "Awake");

        bool died = false;
        health.Died += () => died = true;

        Assert.AreEqual(2, health.GetCurrentLives());
        Assert.IsTrue(health.AddLife());
        Assert.AreEqual(3, health.GetCurrentLives());
        Assert.IsFalse(health.AddLife());
        Assert.AreEqual(3, health.GetCurrentLives());

        Assert.IsTrue(health.TakeDamage());
        Assert.AreEqual(2, health.GetCurrentLives());
        Assert.IsFalse(health.IsInvincible);

        Assert.IsTrue(health.TakeDamage(2));
        Assert.AreEqual(0, health.GetCurrentLives());
        Assert.IsTrue(died);
    }

    [Test]
    public void SceneAutoSetup_CreatesMinimumGameplayObjects()
    {
        GameObject canvasObject = CreateRoot("Canvas");
        canvasObject.AddComponent<Canvas>();
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject panel = new GameObject("GameOverPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(canvasObject.transform, false);
        panel.SetActive(false);
        GameObject title = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        title.transform.SetParent(panel.transform, false);

        GameObject player = CreateRoot("Player");
        player.tag = "Player";
        player.AddComponent<SpriteRenderer>();
        player.AddComponent<Rigidbody2D>();
        player.AddComponent<Animator>();
        player.AddComponent<HealthSystem>();
        player.AddComponent<PlayerController>();

        GameObject setupObject = CreateRoot("SceneSetup");
        SceneAutoSetup setup = setupObject.AddComponent<SceneAutoSetup>();
        SetPrivateField(setup, "collectibleSprite", CreateSprite(Color.cyan));
        SetPrivateField(setup, "polvinaSprite", CreateSprite(Color.magenta));
        SetPrivateField(setup, "laserSprite", CreateSprite(Color.red));
        SetPrivateField(setup, "droneSprite", CreateSprite(Color.green));
        SetPrivateField(setup, "acidSprite", CreateSprite(Color.yellow));
        SetPrivateField(setup, "gameOverPanel", panel);

        InvokePrivate(setup, "Awake");

        Assert.GreaterOrEqual(Object.FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length, 4);
        Assert.GreaterOrEqual(Object.FindObjectsByType<Obstacle>(FindObjectsSortMode.None).Length, 3);
        Assert.IsNotNull(Object.FindFirstObjectByType<GameManager>());
        Assert.IsNotNull(Object.FindFirstObjectByType<HUDManager>());
        Assert.IsNotNull(GameObject.Find("ScoreText"));
        Assert.IsNotNull(player.GetComponent<HealthSystem>());
    }

    private GameObject CreateRoot(string name)
    {
        GameObject root = new GameObject(name);
        _roots.Add(root);
        return root;
    }

    private Collectible CreateCollectible(string name, CollectibleType type)
    {
        GameObject collectibleObject = CreateRoot(name);
        Collectible collectible = collectibleObject.AddComponent<Collectible>();
        collectible.SetType(type);
        return collectible;
    }

    private static void InvokePrivate(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        method?.Invoke(target, null);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        field?.SetValue(target, value);
    }

    private static Sprite CreateSprite(Color color)
    {
        Texture2D texture = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 100f);
    }
}
