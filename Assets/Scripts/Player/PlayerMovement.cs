using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float walkSpeed = 5f;
    private float runSpeed = 8f;
    private KeyCode keyUp = KeyCode.W;
    private KeyCode keyDown = KeyCode.S;
    private KeyCode keyLeft = KeyCode.A;
    private KeyCode keyRight = KeyCode.D;
    private KeyCode keyRun = KeyCode.LeftShift;

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
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(keyLeft))  horizontal -= 1f;
        if (Input.GetKey(keyRight)) horizontal += 1f;
        if (Input.GetKey(keyDown))  vertical  -= 1f;
        if (Input.GetKey(keyUp))    vertical  += 1f;

        bool isRunning = Input.GetKey(keyRun);
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

        if (movement.sqrMagnitude > 0.01f)
            lastMoveDir = movement.normalized;

        float animSpeed = 0f;
        if (movement.sqrMagnitude > 0.01f)
            animSpeed = isRunning ? 2f : 1f;

        animator.SetFloat(AnimMoveX, lastMoveDir.x);
        animator.SetFloat(AnimMoveY, lastMoveDir.y);
        animator.SetFloat(AnimSpeed, animSpeed);
    }
}