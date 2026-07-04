using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controla o personagem usando os botões físicos do ESP32 via USB serial.
/// O teclado/gamepad continua funcionando como fallback para testes.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HealthSystem))]
public class PlayerController : MonoBehaviour
{
    // ----------------------------------------------------------------
    // Inspector
    // ----------------------------------------------------------------

    [Header("Movimento")]
    [Tooltip("Velocidade horizontal (unidades/segundo).")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Pulo")]
    [Tooltip("Força do pulo.")]
    [SerializeField] private float jumpForce = 10f;

    [Tooltip("Layer dos objetos de chão.")]
    [SerializeField] private LayerMask groundLayer;

    [Tooltip("Transform filho na base do sprite para checar chão.")]
    [SerializeField] private Transform groundCheck;

    [Tooltip("Raio da checagem de chão.")]
    [SerializeField] private float groundCheckRadius = 0.1f;

    [Header("Debug")]
    [Tooltip("Se marcado, ignora o ESP32 e usa apenas teclado/gamepad.")]
    [SerializeField] private bool forceKeyboardFallback = false;

    // ----------------------------------------------------------------
    // Privado
    // ----------------------------------------------------------------

    private Rigidbody2D _rb;
    private Animator    _animator;
    private bool        _isGrounded;

    // Fallback teclado/gamepad via New Input System
    private Vector2 _moveInput;
    private bool    _jumpRequested;

    // Buffer de pulo — capturado no Update, consumido no FixedUpdate
    // Evita perder o sinal quando FixedUpdate roda antes do Update no mesmo frame
    private bool _jumpBuffered;

    // Hashes do Animator
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

    // ----------------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------------

    private void Awake()
    {
        _rb       = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Checagem de chão
        Vector2 checkPos = groundCheck != null
            ? (Vector2)groundCheck.position
            : (Vector2)transform.position;
        _isGrounded = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);

        // Captura o JumpPressed do ESP32 aqui no Update e guarda no buffer.
        // Assim o FixedUpdate sempre encontra o sinal, independente da ordem de execução.
        ESP32SerialReader esp = ESP32SerialReader.Instance;
        if (esp != null && esp.IsConnected && !forceKeyboardFallback && esp.JumpPressed)
            _jumpBuffered = true;
    }

    private void FixedUpdate()
    {
        float horizontal;
        bool  jumpThisFrame;

        ESP32SerialReader esp = ESP32SerialReader.Instance;
        bool useESP = esp != null && esp.IsConnected && !forceKeyboardFallback;

        if (useESP)
        {
            // Estado direto dos botões — atualizado em tempo real pelo ESP32
            horizontal    = esp.ButtonRight ? 1f : esp.ButtonLeft ? -1f : 0f;
            jumpThisFrame = _jumpBuffered;  // usa o buffer, não esp.JumpPressed direto
            _jumpBuffered = false;
        }
        else
        {
            // Fallback teclado/gamepad via New Input System
            horizontal     = _moveInput.x;
            jumpThisFrame  = _jumpRequested;
            _jumpRequested = false;
        }

        // ── Movimento horizontal ──────────────────────────────────────
        _rb.linearVelocity = new Vector2(horizontal * moveSpeed, _rb.linearVelocity.y);

        // ── Pulo ─────────────────────────────────────────────────────
        if (jumpThisFrame)
            Debug.Log($"[Player] Tentando pular — isGrounded={_isGrounded}");

        if (jumpThisFrame && _isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // ── Animator ─────────────────────────────────────────────────
        _animator.SetBool(IsMovingHash, horizontal != 0f);

        // ── Flip do sprite ────────────────────────────────────────────
        if (horizontal > 0f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (horizontal < 0f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    // ----------------------------------------------------------------
    // Callbacks New Input System (fallback teclado/gamepad)
    // ----------------------------------------------------------------

    private void OnMove(InputValue value)  => _moveInput = value.Get<Vector2>();
    private void OnJump(InputValue value)  { if (value.isPressed) _jumpRequested = true; }

    // ----------------------------------------------------------------
    // API pública — dispara reações no OLED do ESP32
    // ----------------------------------------------------------------

    /// <summary>Chame para voltar à carinha neutra → (O _ O)</summary>
    public void TriggerIdleReaction()     => ESP32SerialReader.Instance?.SendIdle();

    public void SetKeyboardFallback(bool enabled) => forceKeyboardFallback = enabled;

    // ----------------------------------------------------------------
    // Gizmos
    // ----------------------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
