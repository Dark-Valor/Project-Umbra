using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    #region State Machine
    public PlayerStateMachine StateMachine { get; set; }
    public PlayerDefaultState DefaultState { get; set; }
    public PlayerUmbralState UmbralState { get; set; }
    #endregion

    public int playerSpeed = 10;
    public int playerJumpPower = 1250;

    public int umbralPlayerSpeed = 15;

    [Tooltip("Maximum distance the player can be from the ground to still be considered \"grounded\"")]
    public float distToGround = 0.01f;
    public bool inControl = true;

    [Tooltip("The ground layer / the layer that all platforms belong to.")]
    public LayerMask GroundLayer;

    [Tooltip("The umbral layer / the layer that the player can use shadow pool")]
    public LayerMask UmbralLayer;

    public string umbralTag = "Umbral";
    public string groundTag = "Ground";

    [Tooltip("The amount of time a player can be in the air and still be allowed to perform a coyote jump.")]
    public float coyoteAmnestyPeriod = 0.5f;


    private void Awake()
    {
        StateMachine = new PlayerStateMachine();

        DefaultState = new PlayerDefaultState(this, StateMachine);
        UmbralState = new PlayerUmbralState(this, StateMachine);
    }

    private void Start()
    {
        StateMachine.Initialize(DefaultState);
    }

    private void Update()
    {
        StateMachine.CurrentPlayerState.FrameUpdate();
    }
}
