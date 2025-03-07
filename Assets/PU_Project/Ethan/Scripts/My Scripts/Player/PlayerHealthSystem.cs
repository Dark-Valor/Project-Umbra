using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSystem : MonoBehaviour {

    // Insta-death objects should be tagged "Death" and set as a trigger
    // Enemies (and other 1-damage obstacles) should be tagged "Enemy" and should NOT be set as a trigger

    [Header("-Health-")]
    // Feel free to add more! You'll need to edit the script in a few spots, though.
    public GameObject healthBar3;
    public GameObject healthBar2;
    public GameObject healthBar1;


    [Tooltip("Optional sound effect that plays when the player is hurt.")]
    public AudioClip hitSound;

    private Transform respawnPoint;
    private static int playerScore;

    float timer;

    void Start()
    {
        respawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Death"))
        {
            RespawnPlayer();
        }
        else if (collision.CompareTag("Finish"))
        {
            Time.timeScale = 0;
        }
        else if (collision.CompareTag("Health"))
        {
            AddPlayerHealth();
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            TakeDamagePlayer();
        }
    }

    private void TakeDamagePlayer()
    {
        // For more health, copy the if block for health3, change health3 to whatever yours is,
        // then change the if statement for health3 to else if
        StartCoroutine(WaitForTime(0.333f));


        if (hitSound)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        if (healthBar3.activeInHierarchy)
        {
            healthBar3.SetActive(false);
        }
        else if (healthBar2.activeInHierarchy)
        {
            healthBar2.SetActive(false);
        }
        else
        {
            healthBar1.SetActive(false);
            RespawnPlayer();
        }
    }
     
    private void AddPlayerHealth()
    {
        if (!healthBar2.activeInHierarchy)
        {
            healthBar2.SetActive(true);
        }
        else if (!healthBar3.activeInHierarchy)
        {
            healthBar3.SetActive(true);
        }
        // For more health, just copy the else if block for health3 and change the name.
    }

    public void RespawnPlayer()
    {
        // For more health, just add another similar line here.
        healthBar3.SetActive(true);
        healthBar2.SetActive(true);
        healthBar1.SetActive(true);
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gameObject.transform.position = respawnPoint.transform.position;
    }

    public IEnumerator WaitForTime(float time)
    {
        timer = 0f;
        GetComponent<Animator>().SetBool("IsHurt", true);
        while (timer < time)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        GetComponent<Animator>().SetBool("IsHurt", false);
        timer = 0f;
    }
}
