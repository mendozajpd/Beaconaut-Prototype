using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction PlayerControls;
    public InputAction JumpControls;
    private Rigidbody2D _rb;
    private Vector2 _moveDirection;
    private Vector2 _currentVelocity;
    [SerializeField] private float _moveSpeed = 5.0f;

    // JUMP RELATED
    private bool _isJumping;
    private bool _shouldJump;
    private float _jumpPressedTime;
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float jumpGracePeriod = 0.2f; // Grace period duration

    // GRAVITY
    [SerializeField] private float gravityScaleIncreaseRate = 0.1f;
    private float _initialGravityScale;

    // GROUND CHECK
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 boxSize = new Vector2(0.5f, 0.5f);
    [SerializeField] private float castDistance = 0.1f;
    private bool _isGrounded;


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _initialGravityScale = _rb.gravityScale;
    }

    void OnEnable()
    {
        PlayerControls.Enable();
        JumpControls.Enable();
        JumpControls.performed += onJumpPerformed;
    }

    void OnDisable()
    {
        PlayerControls.Disable();
        JumpControls.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Update()
    {
        _moveDirection = PlayerControls.ReadValue<Vector2>();

        _isGrounded = isGrounded();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _currentVelocity = _rb.linearVelocity;
        Vector2 movement = new Vector2(_moveDirection.x, _moveDirection.y) * _moveSpeed;
        _rb.linearVelocity = new Vector2(movement.x, _currentVelocity.y);

        if (_isGrounded)
        {
            _rb.gravityScale = _initialGravityScale; // Reset gravity scale when grounded
            
            if (_jumpPressedTime + jumpGracePeriod > Time.time)
            {
                _jump();
            }
        }
        else
        {
            _rb.gravityScale += gravityScaleIncreaseRate; // Increase gravity scale when not grounded
        }

        if (_isJumping)
        {
            _jump();
        }
    }


    private void onJumpPerformed(InputAction.CallbackContext context)
    {
        _jumpPressedTime = Time.time;

        if (_isGrounded)
        {
            _isJumping = true;
        }
    }

    private void _jump()
    {
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        _isJumping = false;
    }

    private bool isGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0f, -transform.up, castDistance, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }

}
