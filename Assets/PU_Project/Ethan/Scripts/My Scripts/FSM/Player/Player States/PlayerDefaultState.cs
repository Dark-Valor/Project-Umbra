using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerDefaultState : PlayerState
{
    private float moveX;
    private float moveY;

    private float airBorneTimer = 0f;
    private bool coyoteJumpPerformed = false;
    private bool jumping = false;
    private bool falling = false;

    private float groundedTimer = 0f;
    private float requiredGroundTime = 0.2f;

    private bool grounded = true;


    public PlayerDefaultState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (player.inControl)
        {
            PlayerMove();
        }
        if (Input.GetKeyDown(KeyCode.E) && CanShadowPool())
        {
            Debug.Log("Shadow");
            player.StateMachine.ChangeState(player.UmbralState);
        }
    }

    void PlayerMove()
    {
        float yVelocity = player.GetComponent<Rigidbody2D>().velocity.y;

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
            airBorneTimer = 0f;
        }

        if (!grounded && falling && !jumping)
        {
            airBorneTimer += Time.deltaTime;
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
            else if (falling && airBorneTimer <= player.coyoteAmnestyPeriod && !coyoteJumpPerformed && !jumping)
            {
                coyoteJumpPerformed = true;
                Jump();
            }
        }

        //ANIMATIONS
        if (moveX != 0)
        {
           player.GetComponent<Animator>().SetBool("IsRunning", true);
        }
        else
        {
            player.GetComponent<Animator>().SetBool("IsRunning", false);
        }
        if (Input.GetButtonDown("Jump"))
        {
            player.GetComponent<Animator>().SetBool("IsJumping", true);
        }
        else
        {
            player.GetComponent<Animator>().SetBool("IsJumping", false);
        }

        //PLAYER DIRECTION
        if (moveX < 0.0f)
        {
            player.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (moveX > 0.0f)
        {
            player.GetComponent<SpriteRenderer>().flipX = false;
        }

        //PHYSICS
        player.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveX * player.playerSpeed, player.gameObject.GetComponent<Rigidbody2D>().velocity.y);
    }

    void Jump()
    {
        //JUMPING CODE
        jumping = true;
        falling = false;
        grounded = false;
        groundedTimer = 0f;

        player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * player.playerJumpPower);
    }

    public bool IsGrounded()
    {
        return grounded;
    }

    bool IsTouchingGround()
    {
        Vector2 rayOrigin = player.transform.position - new Vector3(0, player.GetComponent<Collider2D>().bounds.extents.y, 0);


        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, player.distToGround, player.GroundLayer);
        RaycastHit2D hitUmbral = Physics2D.Raycast(rayOrigin, Vector2.down, player.distToGround, player.UmbralLayer);

        return hit.collider != null || hitUmbral.collider != null;
    }

    bool CanShadowPool()
    {
        Vector2 rayOrigin = player.transform.position - new Vector3(0, player.GetComponent<Collider2D>().bounds.extents.y, 0);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, player.distToGround, player.UmbralLayer);

        return hit.collider != null;
    }

    public void SetControl(bool b)
    {
        player.inControl = b;
    }
}
