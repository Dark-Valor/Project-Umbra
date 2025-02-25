using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerUmbralState : PlayerState
{
    private float moveX;
    private float moveY;
    private Rigidbody2D rb;
    private Vector2 originalGravity;
    bool isVertical = false;


    public PlayerUmbralState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.GetComponent<SpriteRenderer>().enabled = false;
        SetInvincible(true);
        rb = player.GetComponent<Rigidbody2D>();
        originalGravity = Physics2D.gravity;
    }

    public override void ExitState()
    {
        base.ExitState();
        player.GetComponent<SpriteRenderer>().enabled = true;
        SetInvincible(false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (player.inControl)
        {
            PlayerMove();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            player.StateMachine.ChangeState(player.DefaultState);
        }
        
        if (IsNotUmbral())
        {
            player.StateMachine.ChangeState(player.DefaultState);
        }
        

        if (HitWall() && !isVertical)
        {
            StartWallClimb();
        } 
        else if (HitWall() && isVertical)
        {
            Debug.Log("true");
            StopWallClimb();
        }
    }

    void PlayerMove()
    {
        if (!isVertical)
        {
            moveX = Input.GetAxis("Horizontal");
            player.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveX * player.umbralPlayerSpeed, player.gameObject.GetComponent<Rigidbody2D>().velocity.y);
        }
        else
        {
            moveY = Input.GetAxis("Horizontal");
            player.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveX * player.umbralPlayerSpeed, moveY * player.umbralPlayerSpeed);
        }
    }

    //bool IsNotUmbral()
    //{
    //    Vector2 rayOrigin = player.transform.position - new Vector3(0, player.GetComponent<Collider2D>().bounds.extents.y, 0);
    //    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, player.distToGround, player.UmbralLayer);
    //    return hit.collider == null;
    //}

    bool IsNotUmbral()
    {
        Vector2 rayOrigin = player.transform.position - new Vector3(0, player.GetComponent<Collider2D>().bounds.extents.y, 0);

        Vector2 rayDirection = isVertical ? Vector2.left : Vector2.down;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, player.distToGround, player.UmbralLayer);

        return hit.collider == null; 
    }


    void SetInvincible(bool invincible)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                Physics2D.IgnoreCollision(enemyCollider, player.GetComponent<Collider2D>(), invincible);
            }
        }
    }

    bool HitWall()
    {
        Vector2 rayOrigin = player.transform.position;

        RaycastHit2D hitRight = Physics2D.Raycast(rayOrigin, Vector2.right, 2.5f, player.UmbralLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(rayOrigin, Vector2.left, 2.5f, player.UmbralLayer);
        
        bool validRightHit = hitRight.collider && hitRight.collider.CompareTag("VerticalWall");
        bool validLeftHit = hitLeft.collider && hitLeft.collider.CompareTag("VerticalWall");

        return validRightHit || validLeftHit;

    }

    void StartWallClimb()
    {
        Debug.Log("Starting wall climb");
        isVertical = true;

        player.transform.rotation = Quaternion.Euler(0, 0, 90);

        Physics2D.gravity = new Vector2(9.81f, 0);

        rb.velocity = Vector2.zero;
    }

    void StopWallClimb()
    {
        Debug.Log("Stopping wall climb");
        isVertical = false;

        player.transform.rotation = Quaternion.identity;
        Physics2D.gravity = originalGravity;
    }
}
