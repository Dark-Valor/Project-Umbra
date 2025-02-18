using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour {

    [Tooltip("Drag the projectile prefab in here.")]
    [SerializeField] private GameObject projectile;

    [Tooltip("Leave true for the projectile to flip with the player.")]
    [SerializeField] private bool flip = true;

    [Tooltip("If true then the movement speed of the player affects the speed of the projectile.")]
    [SerializeField] private bool projectileAffectedByPlayerSpeed = true;

    private bool facingRight = true;
    private Rigidbody2D rbdPlayer;
    
    void Start()
    {
        rbdPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (facingRight)
        {
            if (Input.GetAxis("Horizontal") < 0)
            {
                transform.RotateAround(rbdPlayer.transform.position, Vector3.up, 180f);
                facingRight = false;
            }
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            transform.RotateAround(rbdPlayer.transform.position, Vector3.up, 180f);
            facingRight = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            GameObject shot = Instantiate(projectile);

            // If player is facing left, flip the projectile, if flip is checked
            if (!facingRight && flip)
            {
                Vector3 rot = shot.transform.rotation.eulerAngles;
                rot = new Vector3(rot.x, rot.y + 180, rot.z);
                shot.transform.rotation = Quaternion.Euler(rot);
            }

            shot.transform.position = transform.position;
            shot.SetActive(true);
            float playerSpeed = rbdPlayer.velocity.x;

            if (projectileAffectedByPlayerSpeed)
            {
                shot.GetComponent<Projectile>().Launch(playerSpeed, facingRight);
            }
            else
            {
                shot.GetComponent<Projectile>().Launch(0f, facingRight);
            }
        }
    }
}
