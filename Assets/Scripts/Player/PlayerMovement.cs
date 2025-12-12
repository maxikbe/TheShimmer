using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastMoveDir = Vector2.down;
    private float currentSpeed;

    private static readonly int AnimMoveX = Animator.StringToHash("MoveX");
    private static readonly int AnimMoveY = Animator.StringToHash("MoveY");
    private static readonly int AnimSpeed = Animator.StringToHash("Speed");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        movement = new Vector2(horizontal, vertical);
        if (movement.sqrMagnitude > 1f)
            movement.Normalize();

        UpdateAnimator(isRunning);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimator(bool isRunning)
    {
        if (animator == null) return;

        // Zapamatuj poslední směr pohybu (pro idle animaci)
        if (movement.sqrMagnitude > 0.01f)
            lastMoveDir = movement.normalized;

        // Speed:  0 = idle, 1 = walk, 2 = run
        float animSpeed = 0f;
        if (movement.sqrMagnitude > 0.01f)
            animSpeed = isRunning ? 2f : 1f;

        animator.SetFloat(AnimMoveX, lastMoveDir.x);
        animator.SetFloat(AnimMoveY, lastMoveDir.y);
        animator.SetFloat(AnimSpeed, animSpeed);
    }
}