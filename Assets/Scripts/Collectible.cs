using UnityEngine;

public enum CollectibleType
{
    PenDrive,
    DataCore,
    Polvina
}

/// <summary>
/// Coloca este script em qualquer item coletável da cena.
///
/// Setup:
///  1. O item precisa ter um Collider2D com "Is Trigger" marcado.
///  2. O Player precisa ter a tag "Player" (Edit → Project Settings → Tags).
///  3. Ajuste o que acontece nas regiões marcadas com "AÇÃO" abaixo.
/// </summary>
public class Collectible : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Define se o item conta para vitória ou se restaura vida.")]
    [SerializeField] private CollectibleType type = CollectibleType.DataCore;

    [Tooltip("Efeito visual ao coletar (opcional — arraste um prefab de partícula).")]
    [SerializeField] private GameObject collectEffect;

    [Tooltip("Som ao coletar (opcional).")]
    [SerializeField] private AudioClip collectSound;

    private bool _collected;

    public CollectibleType Type => type;
    public bool CountsForVictory => type != CollectibleType.Polvina;

    public void SetType(CollectibleType collectibleType)
    {
        type = collectibleType;
    }

    // ----------------------------------------------------------------
    // Trigger
    // ----------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Só reage ao Player
        if (!other.CompareTag("Player") || _collected) return;
        _collected = true;

        if (type == CollectibleType.Polvina)
        {
            other.GetComponent<HealthSystem>()?.AddLife();
            ESP32SerialReader.Instance?.SendPolvina();
        }
        else
        {
            GameManager.Instance?.OnItemCollected(type);
            ESP32SerialReader.Instance?.SendTreasure();
        }

        // ── AÇÃO: efeito visual ───────────────────────────────────────
        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        // ── AÇÃO: som ────────────────────────────────────────────────
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // ── Remove o item da cena ─────────────────────────────────────
        Destroy(gameObject);
    }
}
