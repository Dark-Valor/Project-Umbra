using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Tooltip("The time between the enemy shooting.")]
    public float timeBetweenShots = 1f;

    [Tooltip("How long until the projectile destroys itself.")]
    public float projectileLifetime = 10f;

    [Tooltip("Optional sound effect played when the enemy shoots.")]
    [SerializeField] private AudioClip shootSound;

    private float breakTime;
    [SerializeField] private GameObject projectilePrefab;

    [Tooltip("If true, the enemy will flip to face the player.")]
    public bool facePlayer;

    [Tooltip("Check this if Face Player is on and the enemy is facing the wrong direction.")]
    public bool flipDirection;

    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        breakTime = timeBetweenShots;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (facePlayer)
        {
            if (player.position.x > transform.position.x)
            {
                if (flipDirection)
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                
            }
            else
            {
                if (flipDirection)
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }
            }
        }

        if (breakTime <= 0)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            if (shootSound)
            {
                AudioSource.PlayClipAtPoint(shootSound, transform.position);
            }

            Destroy(projectile, projectileLifetime);
            breakTime = timeBetweenShots;
        }
        else
        {
            breakTime -= Time.deltaTime;
        }
    }
}
