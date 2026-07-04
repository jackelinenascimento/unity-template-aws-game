using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controla as vidas do Blink e aplica invencibilidade temporária após dano.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class HealthSystem : MonoBehaviour
{
    [Header("Vidas")]
    [Tooltip("Quantidade inicial de Polvinas.")]
    [SerializeField] private int startingLives = 2;

    [Tooltip("Máximo de Polvinas permitidas.")]
    [SerializeField] private int maxLives = 5;

    [Header("Invencibilidade")]
    [Tooltip("Tempo em segundos sem receber novo dano após ser atingido.")]
    [SerializeField] private float invincibilityDuration = 1.5f;

    [Tooltip("Intervalo do piscar do sprite durante a invencibilidade.")]
    [SerializeField] private float blinkInterval = 0.12f;

    private SpriteRenderer _spriteRenderer;
    private Coroutine _blinkRoutine;
    private Color _baseColor = Color.white;

    public event Action<int, int> LivesChanged;
    public event Action Died;

    public int CurrentLives { get; private set; }
    public int MaxLives => maxLives;
    public bool IsInvincible { get; private set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _baseColor = _spriteRenderer.color;
        CurrentLives = Mathf.Clamp(startingLives, 1, Mathf.Max(1, maxLives));
        NotifyLivesChanged();
    }

    public bool TakeDamage(int damageAmount = 1)
    {
        if (damageAmount <= 0 || IsInvincible || CurrentLives <= 0)
            return false;

        CurrentLives = Mathf.Max(0, CurrentLives - damageAmount);
        NotifyLivesChanged();
        ESP32SerialReader.Instance?.SendDamage();

        if (CurrentLives <= 0)
        {
            Died?.Invoke();
            GameManager.Instance?.OnPlayerDeath();
            return true;
        }

        if (invincibilityDuration > 0f)
        {
            if (_blinkRoutine != null)
                StopCoroutine(_blinkRoutine);

            _blinkRoutine = StartCoroutine(InvincibilityRoutine());
        }

        return true;
    }

    public bool AddLife(int amount = 1)
    {
        if (amount <= 0 || CurrentLives >= maxLives)
            return false;

        CurrentLives = Mathf.Clamp(CurrentLives + amount, 0, maxLives);
        NotifyLivesChanged();
        return true;
    }

    public int GetCurrentLives() => CurrentLives;

    private IEnumerator InvincibilityRoutine()
    {
        IsInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibilityDuration)
        {
            SetAlpha(0.35f);
            yield return new WaitForSeconds(blinkInterval);
            SetAlpha(1f);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval * 2f;
        }

        SetAlpha(1f);
        IsInvincible = false;
        _blinkRoutine = null;
    }

    private void NotifyLivesChanged()
    {
        LivesChanged?.Invoke(CurrentLives, maxLives);
    }

    private void SetAlpha(float alpha)
    {
        if (_spriteRenderer == null)
            return;

        Color next = _baseColor;
        next.a = alpha;
        _spriteRenderer.color = next;
    }
}
