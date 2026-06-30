using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controla movimento horizontal e pulo do personagem usando o New Input System.
///
/// Setup:
///  1. Attach este script no GameObject do Player.
///  2. O Player precisa de: Rigidbody2D, Animator, PlayerInput.
///  3. PlayerInput → Actions: InputSystem_Actions | Behavior: Send Messages.
///  4. Crie uma Layer chamada "Ground" e aplique nos objetos de chão.
///  5. No Inspector deste script, atribua a layer "Ground" ao campo Ground Layer.
///  6. No Animator, adicione:
///       - Bool  "IsMoving"
///       - Bool  "IsJumping"
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // ----------------------------------------------------------------
    // Inspector fields
    // ----------------------------------------------------------------

    [Header("Movimento")]
    [Tooltip("Velocidade horizontal do personagem (unidades por segundo).")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Pulo")]
    [Tooltip("Força aplicada ao pular.")]
    [SerializeField] private float jumpForce = 10f;

    [Tooltip("Layer dos objetos que servem como chão.")]
    [SerializeField] private LayerMask groundLayer;

    [Tooltip("Ponto na base do personagem usado para checar se está no chão.\n" +
             "Crie um GameObject filho vazio na base do sprite e arraste aqui.")]
    [SerializeField] private Transform groundCheck;

    [Tooltip("Raio da checagem de chão. Ajuste até cobrir toda a base do sprite.")]
    [SerializeField] private float groundCheckRadius = 0.1f;

    // ----------------------------------------------------------------
    // Referências privadas
    // ----------------------------------------------------------------

    private Rigidbody2D _rb;
    private Animator    _animator;

    // Input armazenado entre frames
    private Vector2 _moveInput;
    private bool    _jumpRequested;   // true por um frame quando Space é pressionado

    // Estado
    private bool _isGrounded;

    // Hashes dos parâmetros do Animator (mais rápido que passar string todo frame)
    private static readonly int IsMovingHash  = Animator.StringToHash("IsMoving");
    private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");

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
        // Checagem de chão feita no Update para não perder o contato entre
        // frames de física. Usa um pequeno círculo na base do personagem.
        _isGrounded = Physics2D.OverlapCircle(
            groundCheck != null ? groundCheck.position : transform.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void FixedUpdate()
    {
        // ---- Movimento horizontal ----
        float horizontal = _moveInput.x;
        _rb.linearVelocity = new Vector2(horizontal * moveSpeed, _rb.linearVelocity.y);

        // ---- Pulo ----
        // Só pula se houver input pendente E estiver no chão (sem pulo duplo).
        if (_jumpRequested && _isGrounded)
        {
            // Zeramos a velocidade vertical antes de aplicar a força
            // para que pulos feitos enquanto cai tenham força consistente.
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        // Consumimos o request independente de ter pulado ou não.
        _jumpRequested = false;

        // ---- Animator ----
        _animator.SetBool(IsMovingHash,  horizontal != 0f);
        _animator.SetBool(IsJumpingHash, !_isGrounded);

        // ---- Flip do sprite ----
        if (horizontal > 0f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (horizontal < 0f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    // ----------------------------------------------------------------
    // Callbacks do New Input System (Send Messages)
    // ----------------------------------------------------------------

    /// <summary>Chamado automaticamente pelo PlayerInput quando a ação Move dispara.</summary>
    private void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    /// <summary>Chamado automaticamente pelo PlayerInput quando a ação Jump dispara.</summary>
    private void OnJump(InputValue value)
    {
        // value.isPressed é true no momento em que o botão é pressionado.
        // Guardamos como flag para processar no próximo FixedUpdate.
        if (value.isPressed)
            _jumpRequested = true;
    }

    // ----------------------------------------------------------------
    // Gizmos – visualiza o groundCheck no Editor (útil para debug)
    // ----------------------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
