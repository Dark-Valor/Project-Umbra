using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [Tooltip("Drag the hitbox object in here.")]
    [SerializeField] private Collider2D hitBox;

    [Tooltip("Drag the player's SpriteRenderer in here.")]
    [SerializeField] private SpriteRenderer playerSprite;

    [Tooltip("Optionally the player's animator in here.")]
    [SerializeField] private Animator animator;

    public AudioClip meleeSFX;

    public bool canMelee;
    public float hitBoxTime = 0.1f;
    public float cooldown = 0.5f;

    private bool facingRight = true;
    private float cooldownTimer = 0;
    private Coroutine meleeCoroutine;

    private void Start()
    {
        hitBox.enabled = false;
    }

    private void Update()
    {
        if (facingRight)
        {
            if (Input.GetAxis("Horizontal") < 0)
            {
                facingRight = false;

                transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            }
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            facingRight = true;

            transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        }

        if (Input.GetKeyDown(KeyCode.F) && canMelee && cooldownTimer <= 0.02f)
        {
            if (meleeCoroutine != null)
            {
                StopCoroutine(meleeCoroutine);
            }
            
            meleeCoroutine = StartCoroutine(MeleeCoroutine());
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private IEnumerator MeleeCoroutine()
    {
        if (animator != null)
        {
            animator.SetTrigger("Melee");
        }

        AudioSource.PlayClipAtPoint(meleeSFX, transform.position, 0.5f);

        cooldownTimer = cooldown;
        hitBox.enabled = true;
        yield return new WaitForSeconds(hitBoxTime);
        hitBox.enabled = false;
    }

    public void EnableMeleeAttack(bool _canMelee)
    {
        canMelee = _canMelee;
    }
}
