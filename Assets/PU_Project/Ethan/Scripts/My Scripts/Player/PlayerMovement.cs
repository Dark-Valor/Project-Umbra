using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public int playerSpeed = 10;
    public int playerJumpPower = 1250;
    private float moveX;
    private float moveY;

    [Tooltip("Maximum distance the player can be from the ground to still be considered \"grounded\"")]
    public float distToGround = 0.01f;
    public bool inControl = true;

    [Tooltip("The ground layer / the layer that all platforms belong to.")]
    public LayerMask GroundLayer;

    private float airBorneTimer = 0f;
    private bool coyoteJumpPerformed = false;
    private bool jumping = false;
    private bool falling = false;

    [Tooltip("The amount of time a player can be in the air and still be allowed to perform a coyote jump.")]
    public float coyoteAmnestyPeriod = 0.5f;

    private float groundedTimer = 0f;
    private float requiredGroundTime = 0.2f;

    public bool grounded = true;


    void Update()
    {
        if (inControl)
        {
            PlayerMove();
        }
    }

    void PlayerMove()
    {
        float yVelocity = GetComponent<Rigidbody2D>().velocity.y;

        bool touchingGround = IsTouchingGround();

        if (touchingGround)
        {
            groundedTimer += Time.deltaTime;
        }
        else
        {
            groundedTimer = 0f;
        }

        grounded = groundedTimer >= requiredGroundTime;

        if (grounded)
        {
            falling = false;
            coyoteJumpPerformed = false;
            airBorneTimer = 0f;
            jumping = false;
        }

        if (!grounded && !falling && yVelocity < 0 && !jumping)
        {
            falling = true;
            airBorneTimer =0f;
        }

        if (!grounded && falling && !jumping)
        {
            airBorneTimer +=Time.deltaTime;
        }

        //CONTROLS
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            if (grounded && !jumping)
            {
                Jump();
            }
            else if (falling && airBorneTimer <= coyoteAmnestyPeriod && !coyoteJumpPerformed && !jumping)
            {
                coyoteJumpPerformed = true;
                Jump();
            }
        }

        //ANIMATIONS
        if (moveX != 0)
        {
            GetComponent<Animator>().SetBool("IsRunning", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("IsRunning", false);
        }
        if (Input.GetButtonDown("Jump"))
        {
            GetComponent<Animator>().SetBool("IsJumping", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("IsJumping", false);
        }

        //PLAYER DIRECTION
        if (moveX < 0.0f)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (moveX > 0.0f)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }

        //PHYSICS
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveX * playerSpeed, gameObject.GetComponent<Rigidbody2D>().velocity.y);
    }

    void Jump()
    {
        //JUMPING CODE
        jumping = true;
        falling = false;
        grounded = false;
        groundedTimer = 0f;

        GetComponent<Rigidbody2D>().AddForce(Vector2.up * playerJumpPower);
    }

    public bool IsGrounded()
    {
        return grounded;
    }

    bool IsTouchingGround()
    {
        Vector2 rayOrigin = transform.position - new Vector3(0, GetComponent<Collider2D>().bounds.extents.y, 0);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, distToGround, GroundLayer);
        return hit.collider != null ;
    }

    public void SetControl(bool b)
    {
        inControl = b;
    }
}