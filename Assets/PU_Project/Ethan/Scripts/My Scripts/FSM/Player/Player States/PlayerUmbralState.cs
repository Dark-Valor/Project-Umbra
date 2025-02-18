using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUmbralState : PlayerState
{
    private float moveX;
    private float moveY;

    BoxCollider2D boxCollider;


    public PlayerUmbralState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.GetComponent<SpriteRenderer>().enabled = false;
        boxCollider = player.GetComponent<BoxCollider2D>();
        //boxCollider.offset -= new Vector2(0, boxCollider.size.y / 2);
    }

    public override void ExitState()
    {
        base.ExitState();
        player.GetComponent<SpriteRenderer>().enabled = true;

        //boxCollider.offset += new Vector2(0, boxCollider.size.y / 2);
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
    }

    void PlayerMove()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        player.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveX * player.umbralPlayerSpeed, player.gameObject.GetComponent<Rigidbody2D>().velocity.y);
    }
}
