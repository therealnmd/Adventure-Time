using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private BoxCollider2D _boxCollider;
    private float horizontalInput;

    //private bool grounded;
    private int maxJump = 2;
    private int jumpsRemaining;
    private float wallJumpCooldown;


    public float speed = 12.0f;
    public float jumpForce = 5.0f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;


    /*
    //Có thể sẽ k sử dụng đến
    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.03f);
    public LayerMask wallLayer;

    //Có thể sẽ k sử dụng đến
    [Header("WallMovement")]
    public float wallSlideSpeed = 2f;
    bool isWallSliding;

    //Có thể sẽ k sử dụng đến
    [Header("WallJumping")]
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpForce = new Vector2(5f, 5f);
    */
    // Start is called before the first frame update
    void Start()
    {
        //Lấy thuộc tính cho rigidbody và animator từ gameobject
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
        jumpsRemaining = 2; //đặt số lần có thể nhảy (double jump)
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        //Perform Walk
        


        //Flip the character when facing left and right
        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //Perform Jump
        

        //ProcessWallJump();

        _animator.SetBool("isRunning", horizontalInput != 0);
        _animator.SetBool("isGrounded", isGrounded());

        if (wallJumpCooldown > 0.2f)
        {
            _rigidbody.velocity = new Vector2(horizontalInput * speed, _rigidbody.velocity.y);

            if (onWall() && !isGrounded())
            {
                _rigidbody.gravityScale = 0;
                _rigidbody.velocity = Vector2.zero;
            }
            else
            {
                _rigidbody.gravityScale = 1;
            }

            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && jumpsRemaining > 0)
            {
                Jump();
            }
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }

    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    private void Jump()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce);
        _animator.SetTrigger("isJumping");
        jumpsRemaining--;
        
        if (onWall() && !isGrounded())
        {
            if (horizontalInput == 0)
            {
                _rigidbody.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                _rigidbody.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            }
            wallJumpCooldown = 0;
        }
        
        
        //grounded = false;

        /*if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            _rigidbody.velocity = new Vector2(wallJumpDirection * wallJumpForce.x, wallJumpForce.y); //Jump away from wall
            wallJumpTimer = 0;
            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }*/
    }


    /*private void ProcessWallSlide()
    {
        
    }

    private void ProcessWallJump()
    {
        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
        else
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }
    */

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            //grounded = true;
            jumpsRemaining = maxJump;
        }
    }
}
