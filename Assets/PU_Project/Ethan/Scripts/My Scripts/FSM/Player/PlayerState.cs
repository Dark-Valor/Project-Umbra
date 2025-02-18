using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState 
{
    protected Player player;
    protected PlayerStateMachine stateMachine;

    public PlayerState(Player player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    public virtual void FrameUpdate() { }
}
