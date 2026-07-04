using UnityEngine;

/// <summary>
/// Zona de dano genérica do laboratório.
/// </summary>
public class DamageZone : MonoBehaviour
{
    [Header("Dano")]
    [Tooltip("Quantidade de Polvinas removidas ao tocar na zona.")]
    [SerializeField] private int damageAmount = 1;

    [Tooltip("Nome opcional da zona para facilitar o debug.")]
    [SerializeField] private string zoneName = "DamageZone";

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health == null)
            return;

        if (health.TakeDamage(damageAmount))
            Debug.Log($"[DamageZone] {zoneName} causou dano no Player.");
    }
}
