using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Health_Segmented : MonoBehaviour {

    // Insta-death objects should be tagged "Death" and set as a trigger
    // Enemies (and other 1-damage obstacles) should be tagged "Enemy" and should NOT be set as a trigger

    [Header("-Score-")]
    [Tooltip("The score value of a coin or pickup.")]
    public int coinValue = 5;
    [Tooltip("The amount of points a player loses on death.")]
    public int deathPenalty = 20;

    [Tooltip("The text on the Canvas for displaying the score.")]
    public Text scoreText;

    [Tooltip("Optional prefix before the score text.")]
    public string scorePrefix;

    [Tooltip("The number of digits in the score number. 2 = 00, 4 = 0000, etc. Set to 0 to ignore.")]
    public int scoreDigits;

    [Header("-Health-")]
    // Feel free to add more! You'll need to edit the script in a few spots, though.
    public GameObject health3;
    public GameObject health2;
    public GameObject health1;

    [Tooltip("Optional sound effect that plays when a coin is picked up.")]
    public AudioClip Chirp;

    [Tooltip("Optional sound effect that plays when the player is hurt.")]
    public AudioClip hitSFX;

    private Transform respawn;
    private static int playerScore;

    void Start()
    {
        respawn = GameObject.FindGameObjectWithTag("Respawn").transform;
        UpdateScore();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Death"))
        {
            Respawn();
        }
        else if (collision.CompareTag("Coin"))
        {
            AddPoints(coinValue);
            Destroy(collision.gameObject);
            
            if (Chirp)
            {
                AudioSource.PlayClipAtPoint(Chirp, transform.position);
            }
        }
        else if (collision.CompareTag("Finish"))
        {
            Time.timeScale = 0;
        }
        else if (collision.CompareTag("Health"))
        {
            AddHealth();
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        // For more health, copy the if block for health3, change health3 to whatever yours is,
        // then change the if statement for health3 to else if

        if (hitSFX)
        {
            AudioSource.PlayClipAtPoint(hitSFX, transform.position);
        }

        if (health3.activeInHierarchy)
        {
            health3.SetActive(false);
        }
        else if (health2.activeInHierarchy)
        {
            health2.SetActive(false);
        }
        else
        {
            health1.SetActive(false);
            Respawn();
        }
    }
     
    private void AddHealth()
    {
        if (!health2.activeInHierarchy)
        {
            health2.SetActive(true);
        }
        else if (!health3.activeInHierarchy)
        {
            health3.SetActive(true);
        }
        // For more health, just copy the else if block for health3 and change the name.
    }

    public void Respawn()
    {
        // For more health, just add another similar line here.
        health3.SetActive(true);
        health2.SetActive(true);
        health1.SetActive(true);
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gameObject.transform.position = respawn.transform.position;
        AddPoints(-deathPenalty);
    }

    public int GetScore()
    {
        return playerScore;
    }

    public void AddPoints(int amount)
    {
        playerScore += amount;
        playerScore = (int)Mathf.Clamp(playerScore, 0, Mathf.Infinity);
        UpdateScore();
    }

    public void UpdateScore()
    {
        if (scoreDigits > 0)
        {
            scoreText.text = playerScore.ToString("D" + scoreDigits);
        }
        else
        {
            scoreText.text = playerScore.ToString();
        }
    }
}
