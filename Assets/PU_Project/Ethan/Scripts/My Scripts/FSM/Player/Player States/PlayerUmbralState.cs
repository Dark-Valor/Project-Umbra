using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerUmbralState : PlayerState
{
    private float moveX;
    private float moveY;
    private Rigidbody2D rb;
    private Vector2 originalGravity;

    private bool facingRight = false;

    private Vector2 defaultGravity = new Vector2(0f, -9.81f);
    bool isVertical = false; bool hitCeiling = false;

    bool rightWall; bool leftWall; bool ceiling;

    Vector2 rayOrigin;

    private float notUmbralTimer = 0f;

    private bool lerping = false;

    private BoxCollider2D playerCollider;

    private Vector2 playerColliderInitialSize;

    private RaycastHit2D currentRayCastHit;

    private bool isRotating;
    private float rotationTimer = 0f;

    private enum Orientation
    {
        RightSideUp,
        UpsideDown,
        LeftVertical,
        RightVertical
    };

    private Orientation playerOrientation;
    private Orientation previousOrientation;

    private TileData.Type currentSurfaceType;

    public PlayerUmbralState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        playerCollider = player.GetComponent<BoxCollider2D>();
        playerColliderInitialSize = player.GetComponent<BoxCollider2D>().size;
        Vector2 newSize = playerCollider.size;
        newSize.y -= 3f;
        playerCollider.size = newSize;
        playerOrientation = Orientation.RightSideUp;
        player.GetComponent<SpriteRenderer>().enabled = false;
        SetInvincible(true);
        rb = player.GetComponent<Rigidbody2D>();
        originalGravity = defaultGravity;
        player.OnLerpFinished += LerpFinished;

        Debug.Log("physics when entering shadow: " + Physics2D.gravity);
    }

    public override void ExitState()
    {
        Reset();
        Vector2 newSize = playerCollider.size;
        newSize.y += 3f;
        playerCollider.size = newSize;
        player.GetComponent<SpriteRenderer>().enabled = true;
        SetInvincible(false);
        player.OnLerpFinished -= LerpFinished;
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (isRotating)
        {
            player.inControl = false;
            rotationTimer += Time.deltaTime;
            if (rotationTimer >= 0.75f)
            {
                isRotating = false;
                player.inControl = true;
                rotationTimer = 0;
            }
        }

        Vector2 rayOrigin = player.transform.position;
        if (Input.GetKeyDown(KeyCode.E))
        {
            player.StateMachine.ChangeState(player.DefaultState);
        }

        if (!player.inControl)
        {
            return;
        } 
        else
        {
            PlayerMove();
        }
        if (!isRotating)
        {
            CheckSurface();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            DartJump();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SuperJump();
            player.StateMachine.ChangeState(player.DefaultState);
        }
    }

    void PlayerMove()
    {

        if (playerOrientation == Orientation.RightSideUp || playerOrientation == Orientation.UpsideDown)
        {
            moveX = Input.GetAxis("Horizontal");
            player.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveX * player.umbralPlayerSpeed, player.gameObject.GetComponent<Rigidbody2D>().velocity.y);
        }
        else
        {
            moveY = Input.GetAxis("Vertical");
            player.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, moveY * player.umbralPlayerSpeed);
        }
        if (playerOrientation == Orientation.RightSideUp || playerOrientation == Orientation.UpsideDown)
        {
            if (moveX < 0.0f)
            {
                player.GetComponent<SpriteRenderer>().flipX = true;
                facingRight = false;
            }
            else if (moveX > 0.0f)
            {
                player.GetComponent<SpriteRenderer>().flipX = false;
                facingRight = true;
            }
        } 
        else
        {
            if (moveY < 0.0f)
            {
                player.GetComponent<SpriteRenderer>().flipX = true;
                facingRight = false;
            }
            else if (moveY > 0.0f)
            {
                player.GetComponent<SpriteRenderer>().flipX = false;
                facingRight = true;
            }
        }
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

    void Reset()
    {
        isVertical = false;
        rb.freezeRotation = false;
        player.transform.rotation = Quaternion.identity;
        playerOrientation = Orientation.RightSideUp;

        Physics2D.gravity = defaultGravity;
        Debug.Log("reset");
        rb.freezeRotation = true;
    }

    void SuperJump()
    {
        player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * player.playerUmbralJumpPower);
    }

    bool CheckEdgeRotation()
    {
        if (isRotating)
            return false;

        Vector2 rightRayOrigin = new Vector2();
        Vector2 leftRayOrigin = new Vector2();
        RaycastHit2D rightRaycast = new RaycastHit2D();
        RaycastHit2D leftRaycast = new RaycastHit2D();

        Orientation initialOrientation = playerOrientation;
        if (playerOrientation == Orientation.RightSideUp)
        {
            rightRayOrigin = new Vector2(player.transform.position.x + player.GetComponent<Collider2D>().bounds.extents.x - 0.19f, player.transform.position.y);
            leftRayOrigin = new Vector2(player.transform.position.x - player.GetComponent<Collider2D>().bounds.extents.x - 0.19f, player.transform.position.y);
            rightRaycast = Physics2D.Raycast(rightRayOrigin, Vector2.down, player.distToGround * 2f, player.GroundLayer);
            leftRaycast = Physics2D.Raycast(leftRayOrigin, Vector2.down, player.distToGround * 2f, player.GroundLayer);

            Debug.DrawRay(rightRayOrigin, Vector2.down * player.distToGround * 2f, Color.red, 0.1f);
            Debug.DrawRay(leftRayOrigin, Vector2.down * player.distToGround * 2f, Color.green, 0.1f);
        } 
        else if (playerOrientation == Orientation.UpsideDown)
        {
            rightRayOrigin = new Vector2(player.transform.position.x + player.GetComponent<Collider2D>().bounds.extents.x + 0.19f, player.transform.position.y);
            leftRayOrigin = new Vector2(player.transform.position.x - player.GetComponent<Collider2D>().bounds.extents.x + 0.19f, player.transform.position.y);
            rightRaycast = Physics2D.Raycast(rightRayOrigin, Vector2.up, player.distToGround * 2f, player.GroundLayer);
            leftRaycast = Physics2D.Raycast(leftRayOrigin, Vector2.up, player.distToGround * 2f, player.GroundLayer);

            Debug.DrawRay(rightRayOrigin, Vector2.up * player.distToGround * 2f, Color.red, 0.1f);
            Debug.DrawRay(leftRayOrigin, Vector2.up * player.distToGround * 2f, Color.green, 0.1f);
        }
        else if (playerOrientation == Orientation.LeftVertical)
        {
            rightRayOrigin = new Vector2(player.transform.position.x, player.transform.position.y + player.GetComponent<Collider2D>().bounds.extents.y + 0.19f);
            leftRayOrigin = new Vector2(player.transform.position.x, player.transform.position.y - player.GetComponent<Collider2D>().bounds.extents.y + 0.19f);
            rightRaycast = Physics2D.Raycast(rightRayOrigin, Vector2.left, player.distToGround * 2f, player.GroundLayer);
            leftRaycast = Physics2D.Raycast(leftRayOrigin, Vector2.left, player.distToGround * 2f, player.GroundLayer);

            Debug.DrawRay(rightRayOrigin, Vector2.left * player.distToGround * 2f, Color.red, 0.1f);
            Debug.DrawRay(leftRayOrigin, Vector2.left * player.distToGround * 2f, Color.green, 0.1f);
        } 
        else
        {
            rightRayOrigin = new Vector2(player.transform.position.x, player.transform.position.y + player.GetComponent<Collider2D>().bounds.extents.y - 0.19f);
            leftRayOrigin = new Vector2(player.transform.position.x, player.transform.position.y - player.GetComponent<Collider2D>().bounds.extents.y - 0.19f);
            rightRaycast = Physics2D.Raycast(rightRayOrigin, Vector2.right, player.distToGround * 2f, player.GroundLayer);
            leftRaycast = Physics2D.Raycast(leftRayOrigin, Vector2.right, player.distToGround * 2f, player.GroundLayer);

            Debug.DrawRay(rightRayOrigin, Vector2.right * player.distToGround * 2f, Color.red, 0.1f);
            Debug.DrawRay(leftRayOrigin, Vector2.right * player.distToGround * 2f, Color.green, 0.1f);
        }

        if (rightRaycast.collider == null && leftRaycast.collider != null) //going over right edge
        {
            isRotating = true;
            //rotate right 90 degrees
            Debug.Log("[Wallride] Detected right edge! Rotating...");
            player.inControl = true;
            if (initialOrientation == Orientation.RightSideUp)
            {
                playerOrientation = Orientation.LeftVertical;
            }
            if (initialOrientation == Orientation.RightVertical)
            {
                playerOrientation = Orientation.RightSideUp;
            }
            if (initialOrientation == Orientation.LeftVertical)
            {
                playerOrientation = Orientation.UpsideDown;
            }
            if (initialOrientation == Orientation.UpsideDown)
            {
                playerOrientation = Orientation.RightVertical;
            }

            ChangePlayerOrientationEdge();
            AlignPlayerToNewSurface(true);
            player.inControl = true;
            return true;
        }
        else if (rightRaycast.collider != null && leftRaycast.collider == null) //going over left edge
        {
            //rotate left 90 degrees
            isRotating = true;
            Debug.Log("[Wallride] Detected left edge! Rotating...");
            player.inControl = false;
            if (initialOrientation == Orientation.RightSideUp)
            {
                playerOrientation = Orientation.RightVertical;
            }
            else if (initialOrientation == Orientation.RightVertical)
            {
                playerOrientation = Orientation.UpsideDown;
            }
            else if (initialOrientation == Orientation.UpsideDown)
            {
                playerOrientation = Orientation.LeftVertical;
            }
            else if (initialOrientation == Orientation.LeftVertical)
            {
                playerOrientation = Orientation.RightSideUp;
            }

            ChangePlayerOrientationEdge();
            AlignPlayerToNewSurface(false);
            player.inControl = true;
            return true;
        }
        return false;
    }

    void AlignPlayerToNewSurface(bool fromRightCollider)
    {
        Vector3Int tilePosition = TileManager.Instance.tilemap.WorldToCell(player.transform.position);

        Vector3 tileWorldPos = TileManager.Instance.tilemap.GetCellCenterWorld(tilePosition);
        Debug.DrawRay(tileWorldPos, Vector3.up * 0.2f, Color.red, 2f);
        if (fromRightCollider)
        {
            switch (playerOrientation)
            {
                case Orientation.RightSideUp:
                    Debug.Log("rightsideup");
                    player.transform.position = new Vector3(tileWorldPos.x + player.GetComponent<Collider2D>().bounds.extents.x + 2.5f, tileWorldPos.y + player.GetComponent<Collider2D>().bounds.extents.y + 2f, player.transform.position.z);
                    break;
                case Orientation.UpsideDown:
                    Debug.Log("upsidedown");
                    player.transform.position = new Vector3(tileWorldPos.x - player.GetComponent<Collider2D>().bounds.extents.x, tileWorldPos.y - player.GetComponent<Collider2D>().bounds.extents.y, player.transform.position.z);
                    break;
                case Orientation.LeftVertical:
                    Debug.Log("leftvertical");
                    player.transform.position = new Vector3(tileWorldPos.x + player.GetComponent<Collider2D>().bounds.extents.x + 2.5f, tileWorldPos.y - player.GetComponent<Collider2D>().bounds.extents.y - 3.5f, player.transform.position.z);
                    break;
                case Orientation.RightVertical:
                    Debug.Log("rightsideup");
                    player.transform.position = new Vector3(tileWorldPos.x - player.GetComponent<Collider2D>().bounds.extents.x, tileWorldPos.y + player.GetComponent<Collider2D>().bounds.extents.y, player.transform.position.z);
                    break;
            }
        }
        else
        {
            switch (playerOrientation)
            {
                case Orientation.RightSideUp:
                    player.transform.position = new Vector3(tileWorldPos.x - player.GetComponent<Collider2D>().bounds.extents.x, tileWorldPos.y + player.GetComponent<Collider2D>().bounds.extents.y, player.transform.position.z);
                    break;
                case Orientation.UpsideDown:
                    player.transform.position = new Vector3(tileWorldPos.x + player.GetComponent<Collider2D>().bounds.extents.x, tileWorldPos.y - player.GetComponent<Collider2D>().bounds.extents.y, player.transform.position.z);
                    break;
                case Orientation.LeftVertical:
                    player.transform.position = new Vector3(tileWorldPos.x - player.GetComponent<Collider2D>().bounds.extents.x, tileWorldPos.y + player.GetComponent<Collider2D>().bounds.extents.y, player.transform.position.z);
                    break;
                case Orientation.RightVertical:
                    player.transform.position = new Vector3(tileWorldPos.x + player.GetComponent<Collider2D>().bounds.extents.x, tileWorldPos.y + player.GetComponent<Collider2D>().bounds.extents.y, player.transform.position.z);
                    break;
            }
        }

        Debug.Log($"[Wallride] Aligned Player to New Surface at {player.transform.position}");
    }

    void CheckSurface()
    {
        if (isRotating)
            return;

        previousOrientation = playerOrientation;
        Vector2 rayOrigin = player.transform.position;
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        float rayDistance = facingRight ? player.GetComponent<Collider2D>().bounds.extents.x + 0.6f : player.GetComponent<Collider2D>().bounds.extents.x + 0.2f;

        if (playerOrientation == Orientation.RightSideUp)
        {
            direction = facingRight ? Vector2.right : Vector2.left;
            rayDistance = facingRight ? player.GetComponent<Collider2D>().bounds.extents.y + 0.2f : player.GetComponent<Collider2D>().bounds.extents.y + 0.6f;
        }
        else if (playerOrientation == Orientation.UpsideDown)
        {
            direction = facingRight ? Vector2.right : Vector2.left;
            rayDistance = player.GetComponent<Collider2D>().bounds.extents.y + 0.2f;
        }
        else if (playerOrientation == Orientation.LeftVertical)
        {
            direction = facingRight ? Vector2.up : Vector2.down;
        }
        else if (playerOrientation == Orientation.RightVertical)
        {
            direction = facingRight ? Vector2.up : Vector2.down;
        }
        RaycastHit2D forwardRaycast = Physics2D.Raycast(rayOrigin, direction, rayDistance, player.GroundLayer);
        Debug.DrawRay(rayOrigin, direction * rayDistance, Color.red, 0.1f);

        if (forwardRaycast.collider != null)
        {
            TileData tileData = TileManager.Instance.GetTileData(TileManager.Instance.tilemap.WorldToCell(forwardRaycast.point));
            if (tileData != null)
            {
                currentSurfaceType = tileData.type;
            }
            isRotating = true;
            currentRayCastHit = forwardRaycast;
            SetPlayerOrientation(forwardRaycast.normal);
        } else
        {
            if (!isRotating)
            {
                CheckEdgeRotation();
            }
        }
    }

    void SetPlayerOrientation(Vector2 surfaceNormal)
    {
        if (playerOrientation == Orientation.RightSideUp && surfaceNormal == Vector2.up) return;
        if (playerOrientation == Orientation.UpsideDown && surfaceNormal == Vector2.down) return;
        if (playerOrientation == Orientation.LeftVertical && surfaceNormal == Vector2.right) return;
        if (playerOrientation == Orientation.RightVertical && surfaceNormal == Vector2.left) return;

        if (surfaceNormal == Vector2.up)
        {
            Debug.Log("Standing on Ground");
            playerOrientation = Orientation.RightSideUp;
        }
        else if (surfaceNormal == Vector2.down)
        {
            Debug.Log("Hanging from Ceiling");
            playerOrientation = Orientation.UpsideDown;
        }
        else if (surfaceNormal == Vector2.right)
        {
            Debug.Log("Running up a Left Wall");
            playerOrientation = Orientation.LeftVertical;
        }
        else if (surfaceNormal == Vector2.left)
        {
            Debug.Log("Running up a Right Wall");
            playerOrientation = Orientation.RightVertical;
        }
        ChangePlayerOrientation();
    }

    void ChangePlayerOrientationEdge()
    {
        rb.freezeRotation = false;
        if (playerOrientation == Orientation.RightSideUp)
        {
            player.transform.rotation = Quaternion.Euler(0, 0, 0);
            Physics2D.gravity = new Vector2(0, -9.81f);
        }
        else if (playerOrientation == Orientation.UpsideDown)
        {
            player.transform.rotation = Quaternion.Euler(0, 0, 180);
            Physics2D.gravity = new Vector2(0, 9.81f);
        }
        else if (playerOrientation == Orientation.LeftVertical)
        {
            player.transform.rotation = Quaternion.Euler(0, 0, -90);
            Physics2D.gravity = new Vector2(-9.81f, 0);
        }
        else if (playerOrientation == Orientation.RightVertical)
        { 
            player.transform.rotation = Quaternion.Euler(0, 0, 90);
            Physics2D.gravity = new Vector2(9.81f, 0);
        }
        rb.freezeRotation = true;
    }

    void ChangePlayerOrientation()
    {
        rb.freezeRotation = false;
        if (playerOrientation == Orientation.RightSideUp)
        {
            if (previousOrientation == Orientation.LeftVertical)
            {
                player.transform.position = new Vector2(player.transform.position.x + 1f, player.transform.position.y);
            }
            if (previousOrientation == Orientation.RightVertical)
            {
                player.transform.position = new Vector2(player.transform.position.x - 1f, player.transform.position.y);
            }
            player.transform.rotation = Quaternion.Euler(0, 0, 0);
            Physics2D.gravity = new Vector2(0, -9.81f);
        }
        else if (playerOrientation == Orientation.UpsideDown)
        {
            if (previousOrientation == Orientation.LeftVertical)
            {
                player.transform.position = new Vector2(player.transform.position.x + 1f, player.transform.position.y);
            }
            if (previousOrientation == Orientation.RightVertical)
            {
                player.transform.position = new Vector2(player.transform.position.x - 1f, player.transform.position.y);
            }
            player.transform.rotation = Quaternion.Euler(0, 0, 180);
            Physics2D.gravity = new Vector2(0, 9.81f);
        }
        else if (playerOrientation == Orientation.LeftVertical)
        {
            if (previousOrientation == Orientation.RightSideUp)
            {
                player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 1f);
            }
            if (previousOrientation == Orientation.UpsideDown)
            {
                player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y - 1f);
            }
            player.transform.rotation = Quaternion.Euler(0, 0, -90);
            Physics2D.gravity = new Vector2(-9.81f, 0);
        }
        else if (playerOrientation == Orientation.RightVertical)
        {
            if (previousOrientation == Orientation.RightSideUp)
            {
                player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 1f);
            }
            if (previousOrientation == Orientation.UpsideDown)
            {
                player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y - 1f);
            }
            player.transform.rotation = Quaternion.Euler(0, 0, 90);
            Physics2D.gravity = new Vector2(9.81f, 0);
        }
        rb.freezeRotation = true;
    }



    void DartJump()
    {
        float positionOffset = 0f;
        bool xOrY = false; //true for x, false for y
        Vector2 rayOrigin = player.transform.position;
        RaycastHit2D hit = new RaycastHit2D();

        Tilemap tilemap = TileManager.Instance.tilemap;

        Vector2 newGravity = new Vector2(0f, 0f);

        //right vertical wall
        if (Physics2D.gravity == new Vector2(9.81f, 0))
        {
            Debug.Log("TRIED FROM RIGHT WALL");
            positionOffset = -player.GetComponent<Collider2D>().bounds.extents.x + 1f;
            xOrY = true;
            hit = Physics2D.Raycast(rayOrigin, Vector2.left, 150f, player.GroundLayer);
            newGravity = new Vector2(-9.81f, 0f);

            rb.freezeRotation = false;
            player.transform.rotation = Quaternion.Euler(0, 0, -90);
            rb.freezeRotation = true;

            playerOrientation = Orientation.LeftVertical;
            
            Debug.DrawRay(rayOrigin, Vector2.left * 150f, Color.red, 0.1f);
        }
        //left vertical wall
        else if (Physics2D.gravity == new Vector2(-9.81f, 0))
        {
            Debug.Log("TRIED FROM LEFT WALL");
            positionOffset = player.GetComponent<Collider2D>().bounds.extents.x + 3f;
            xOrY = true;
            hit = Physics2D.Raycast(rayOrigin, Vector2.right, 150f, player.GroundLayer);
            newGravity = new Vector2(9.81f, 0f);

            rb.freezeRotation = false;
            player.transform.rotation = Quaternion.Euler(0, 0, 90);
            rb.freezeRotation = true;

            playerOrientation = Orientation.RightVertical;

            Debug.DrawRay(rayOrigin, Vector2.right * 150f, Color.red, 0.1f);
        }
        //on the ground
        else if (Physics2D.gravity == defaultGravity)
        {
            positionOffset = player.GetComponent<Collider2D>().bounds.extents.y + 4f;
            xOrY = false;
            hit = Physics2D.Raycast(rayOrigin, Vector2.up, 150f, player.GroundLayer);
            newGravity = -defaultGravity;

            rb.freezeRotation = false;
            player.transform.rotation = Quaternion.Euler(0, 0, 180);
            rb.freezeRotation = true;
            isVertical = false;

            playerOrientation = Orientation.UpsideDown;

            Debug.DrawRay(rayOrigin, Vector2.up * 150f, Color.red, 0.1f);
        }
        //on the ceiling
        else if (Physics2D.gravity == -defaultGravity)
        {
            positionOffset = -player.GetComponent<Collider2D>().bounds.extents.y - 2f;
            xOrY = false;
            hit = Physics2D.Raycast(rayOrigin, Vector2.down, 150f, player.GroundLayer);
            newGravity = defaultGravity;

            rb.freezeRotation = false;
            player.transform.rotation = Quaternion.identity;
            rb.freezeRotation = true;

            playerOrientation = Orientation.RightSideUp;

            Debug.DrawRay(rayOrigin, Vector2.down * 150f, Color.red, 0.1f);
        }

        Vector3 positionVector;
        if (xOrY) { positionVector = new Vector3(positionOffset, 0f, 0f); }
        else { positionVector = new Vector3(0f, positionOffset, 0f); }

        if (hit.collider != null)
        {
            Vector3Int tilePosition = tilemap.WorldToCell(hit.point);

            Vector3 tileWorldPos = tilemap.GetCellCenterWorld(tilePosition);

            Debug.Log($"[DartJump] Moving to Tile: {tilePosition} at {tileWorldPos}");

            Vector3 target;
            if (xOrY)
            {
                target = new Vector3(tileWorldPos.x - positionOffset, player.transform.position.y, player.transform.position.z);
            }
            else
            {
                target = new Vector3(player.transform.position.x, tileWorldPos.y - positionOffset, player.transform.position.z);
            }

            lerping = true;
            player.StartMovementCoroutine(target, 100f);

            Physics2D.gravity = newGravity;

        }
        else
        {
            return;
        }
    }

    void LerpFinished()
    {
        lerping = false;
    }
}