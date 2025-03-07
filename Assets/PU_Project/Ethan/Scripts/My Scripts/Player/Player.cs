using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    public int playerUmbralJumpPower = 1500;

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

    [Tooltip("Length of time in seconds of melee attack animation")]
    public float meleeAnimLength = 0.5f;

    public GameObject meleeAttack;

    [HideInInspector]
    public event Action OnLerpFinished;


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

    public void StartMovementCoroutine(Vector3 targetPos, float speed)
    {
        StartCoroutine(MoveToPosition(targetPos, speed));
    }

    public void StartDeactivationCoroutine(GameObject obj, float time)
    {
        StartCoroutine(DeactivateAfterDelay(obj, time));
    }

    IEnumerator MoveToPosition(Vector3 targetPos, float speed)
    {
        //another fucking problem with tiles being one game object - this ends instantly
        //do a raycast and pass it in. if it touches, then you end the coroutine
        //flip to orientation handedled in umbral state code
        inControl = false;
        while (Vector3.Distance(transform.position, targetPos) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        Debug.Log("LERPING DONE");
        inControl = true;
        OnLerpFinished?.Invoke();
    }

    IEnumerator DeactivateAfterDelay(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }
}
