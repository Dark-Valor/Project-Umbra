using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [Tooltip("How long in seconds until the projectile disappears.")]
    [SerializeField] private float lifetime = 2.0f;
    private int lifetimeFrames = 0;
    private int timeLived = 0;

    [Tooltip("Speed of the projectile relative to the speed of the player.")]
    [SerializeField] private float relativeVelocity = 1f;
    private float velocity = 0f;

	void Start () {

        lifetimeFrames = (int)(lifetime / Time.fixedDeltaTime);
	}
	
    public void Launch(float emitVelocity, bool goingRight)
    {
        if (goingRight)
        {
            velocity = emitVelocity + relativeVelocity;
        }
        else
        {
            velocity = emitVelocity - relativeVelocity;
        }
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.right * velocity;
    }

	void FixedUpdate () {
		if (timeLived < lifetimeFrames)
        {
            timeLived++;
        }
        else
        {
            Destroy(gameObject);
        }
	}
}
