using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
   public float moveSpeed = 3f;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastMoveDir = Vector2.down;
    private static readonly int AnimMoveY = Animator.StringToHash("MoveX");
    private static readonly int AnimMoveX = Animator.StringToHash("MoveY");
    private static readonly int AnimSpeed = Animator.StringToHash("Speed");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            movement = direction * moveSpeed;


            UpdateAnimator();
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }

    private void UpdateAnimator()
    {
        if (movement.sqrMagnitude > 0.01f)
        {
            lastMoveDir = movement.normalized;
        }

        animator.SetFloat(AnimMoveX, lastMoveDir.x);
        animator.SetFloat(AnimMoveY, lastMoveDir.y);
        animator.SetFloat(AnimSpeed, movement.sqrMagnitude);
    }
}
