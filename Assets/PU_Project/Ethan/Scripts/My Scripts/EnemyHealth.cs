using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    
    [Tooltip("How many times the enemy can get hit before being destroyed.")]
    [SerializeField] private int health = 3;

    [Tooltip("If true the sprite will flash red briefly when hit by a projectile.")]
    [SerializeField] private bool flashRedOnHit = true;

    [Tooltip("Drag in the enemy's SpriteRenderer component.")]
    [SerializeField] private SpriteRenderer sprite;

    [Tooltip("Optional sound effect when enemy is hit.")]
    [SerializeField] private AudioClip hitSoundEffect;

    [Tooltip("Optional sound effect when enemy is destroyed.")]
    [SerializeField] private AudioClip deathSoundEffect;

    [Tooltip("Optional: Drag in the enemy's animator for hit and death animations.")]
    [SerializeField] private Animator animator;

    [Tooltip("Delay (in seconds) before the enemy is destroyed after losing all health. Increase this if the enemy is being destroyed before their death animation fully plays.")]
    [SerializeField] private float delayBeforeDying = 0f;

    private Color originalColor;

    void Start()
    {
        if (flashRedOnHit)
        {
            if (sprite)
            {
                originalColor = sprite.color;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enemy collided with " + collision.gameObject.name);
        if (collision.CompareTag("PlayerMelee"))
        {
            Debug.Log("HIT!");
            collision.gameObject.SetActive(false);
            Hit();
        }
    }

    private void Hit()
    {
        health--;

        if (health <= 0)
        {
            if (deathSoundEffect)
            {
                AudioSource.PlayClipAtPoint(deathSoundEffect, transform.position);
            }

            if (animator)
            {
                animator.SetTrigger("Death");
            }

            StartCoroutine(DeathDelay(delayBeforeDying));
        }
        else
        {
            if (hitSoundEffect)
            {
                AudioSource.PlayClipAtPoint(hitSoundEffect, transform.position);
            }

            if (animator)
            {
                animator.SetTrigger("Hit");
            }
        }

        if (flashRedOnHit)
        {
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = originalColor;
    }

    private IEnumerator DeathDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
