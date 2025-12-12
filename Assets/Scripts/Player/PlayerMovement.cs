using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed = 5f;
    private KeyCode moveUpKey = KeyCode.W;
    private KeyCode moveDownKey = KeyCode.S; 
    private KeyCode moveLeftKey = KeyCode.A;
    private KeyCode moveRightKey = KeyCode.D;
    private Rigidbody2D rb;
    private Vector2 movement;

    private Matrix4x4 isoMatrix;

    private bool moveSouth = false;
    private bool moveSouthWest = false;
    private bool moveSouthEast = false;
    private bool moveNorth = false;
    private bool moveNorthWest = false;
    private bool moveNorthEast = false;
    private bool moveWest = false;
    private bool moveEast = false;
    public bool isMoving = false;
    [SerializeField] private Animator animator;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -45));
    }


    void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(moveUpKey))vertical += 1f;
        if (Input.GetKey(moveDownKey))vertical -= 1f;
        if (Input.GetKey(moveLeftKey))horizontal -= 1f;
        if (Input.GetKey(moveRightKey))horizontal += 1f;

        UpdateDirectionBools(horizontal, vertical);
        UpdateAnimator();

        movement = new Vector2(horizontal, vertical).normalized;
        Vector2 input = new Vector2(horizontal, vertical).normalized;
        Vector3 isoMovement = isoMatrix.MultiplyVector(new Vector3(input.x, input.y, 0));
    }

    void FixedUpdate()
    {
       
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
      
        
    }

    void UpdateDirectionBools(float horizontal, float vertical)
    {
        
        ResetDirectionBools();
        moveSouth = vertical < 0 && horizontal == 0;
        moveNorth = vertical > 0 && horizontal == 0;
        moveWest = horizontal < 0 && vertical == 0;
        moveEast = horizontal > 0 && vertical == 0;
        moveSouthWest = vertical < 0 && horizontal < 0;
        moveSouthEast = vertical < 0 && horizontal > 0;
        moveNorthWest = vertical > 0 && horizontal < 0;
        moveNorthEast = vertical > 0 && horizontal > 0;
        //Debug.Log(moveSouth + " " + moveNorth + " " + moveEast + " " + moveWest + " " + moveSouthEast + " " + moveSouthWest + " " + moveNorthEast + " " + moveNorthWest);
    }

    void ResetDirectionBools()
    {
        moveSouth = false;
        moveNorth = false;
        moveWest = false;
        moveEast = false;
        moveSouthWest = false;
        moveSouthEast = false;
        moveNorthWest = false;
        moveNorthEast = false;
    }

    void UpdateAnimator()
    {
        if (animator == null) return;
        
        animator.SetBool("South", moveSouth);
        animator.SetBool("North", moveNorth);
        animator.SetBool("West", moveWest);
        animator.SetBool("East", moveEast);
        animator.SetBool("SouthWest", moveSouthWest);
        animator.SetBool("SouthEast", moveSouthEast);
        animator.SetBool("NorthWest", moveNorthWest);
        animator.SetBool("NorthEast", moveNorthEast);
    }
}
