using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityStandardAssets.CrossPlatformInput;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public bool canJump = true;

    [SerializeField] private AudioClip hazardSound;

    [SerializeField] private AudioClip[] damageSound;

    [SerializeField] private float runSpeed = 5f;

    [SerializeField] private float jumpSpeed = 5f;

    [SerializeField] private float climbSpeed = 5f;

    [SerializeField] private float deathKickHeight = 4f;
    // Vector2 deathKick = new Vector2(0, 4f);

    [SerializeField] private float bigDeathKickHeight = 20f;

    private AudioMixer audioMixer;

    private float gravityScaleAtStart;

    private float initialJumpSpeed;

    private float initialRunSpeed;

    // Config
    private bool inputEnabled = true;


    // [SerializeField] List<Sprite> damageSpeeches = new List<Sprite>();

    // State
    private bool isAlive = true;
    // Vector2 bigDeathKick = new Vector2(0, 20f);

    private float kickDirection;

    private float masterVolume;
    private Animator myAnimator;
    private CapsuleCollider2D myBodyCollider;
    private BoxCollider2D myFeet;

    // Cached component references
    private Rigidbody2D myRigidBody;
    private SpriteRenderer mySprite;


    // Message then methods
    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        mySprite = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeet = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;

        getSpeedValues();

        // Get master volume
        audioMixer = Resources.Load<AudioMixer>("MainAudioMixer");
        audioMixer.GetFloat("masterVolume", out masterVolume);
    }

    private void Update()
    {
        if (!isAlive) return;

        Run();
        ClimbLadder();
        Jump();
        FlipSprite();
        LooseHealth();
        Sick();
        DeathKickDirection();
    }

    private void getSpeedValues()
    {
        initialRunSpeed = runSpeed;
        initialJumpSpeed = jumpSpeed;
    }

    private void Run()
    {
        if (!inputEnabled) return;

        var controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // value is between -1 to +1
        var playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        var playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("Running", playerHasHorizontalSpeed);
    }

    private void DeathKickDirection()
    {
        if (CrossPlatformInputManager.GetAxis("Horizontal") > 0)
            kickDirection = -4f;
        else if (CrossPlatformInputManager.GetAxis("Horizontal") < 0) kickDirection = 4f;
    }

    private void ClimbLadder()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myAnimator.SetBool("Climbing", false);
            myRigidBody.gravityScale = gravityScaleAtStart;
            return;
        }

        var controlThrow = 0f;

        // Disable controls if not is disabled
        if (inputEnabled) controlThrow = CrossPlatformInputManager.GetAxis("Vertical");

        var climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;


        var playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);
    }

    private void Jump()
    {
        // Check if player has input enabled
        if (!inputEnabled || !canJump) return;

        // Check if player is touching ground
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;

        if (CrossPlatformInputManager.GetButtonDown("Jump")) StartCoroutine("Jumping");
    }

    private IEnumerator Jumping()
    {
        var jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
        myRigidBody.velocity += jumpVelocityToAdd;
        canJump = false;
        // Pause jumping ability
        yield return new WaitForSeconds(0.6f);
        canJump = true;
    }

    private void handleDeathKick()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground Hazards")) ||
            myFeet.IsTouchingLayers(LayerMask.GetMask("Ground Hazards")))
            GetComponent<Rigidbody2D>().velocity = new Vector2(kickDirection, deathKickHeight);
        else
            GetComponent<Rigidbody2D>().velocity = new Vector2(kickDirection, bigDeathKickHeight);
    }


    private void LooseHealth()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards", "Ground Hazards")) ||
            myFeet.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards", "Ground Hazards")))
        {
            isAlive = false;
            StartCoroutine("Dying");
            // mySprite.enabled = false;
            handleDeathKick();
            FindObjectOfType<GameSession>().ProcessPlayerDeath();

            // Show a random damage Speech Bubble when taking damage
            // DamageSpeechBubble();

            // If there's an Audio Clip in the first array position play the damage sound
            if (damageSound[0])
            {
                // Assign a random Audio Clip
                var rand = Random.Range(0, damageSound.Length);

                AudioSource.PlayClipAtPoint(damageSound[rand], transform.position, (masterVolume + 80f) * 0.01f);
            }
        }
    }

    // private void DamageSpeechBubble()
    // {
    //     // Choose a random Damage Speech Bubble
    //     Sprite randomSpeech = damageSpeeches[Random.Range(0, damageSpeeches.Count)];

    //     // Call Speech Bubble method
    //     FindObjectOfType<SpeechBubble>().ChangeBubble(randomSpeech);
    // }

    private void Sick()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("5G")))
        {
            GetComponent<SpriteRenderer>().color = Color.green;

            runSpeed = initialRunSpeed / 2;
            jumpSpeed = initialJumpSpeed / 2;

            AudioSource.PlayClipAtPoint(hazardSound, transform.position, (masterVolume + 40f) * 0.01f);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            runSpeed = initialRunSpeed;
            jumpSpeed = initialJumpSpeed;
        }
    }

    public void StartTime()
    {
        Time.timeScale = 1f;
    }


    private IEnumerator Dying()
    {
        myAnimator.SetTrigger("Dying");
        yield return new WaitForSeconds(0.6f);
        isAlive = true;
    }

    private void FlipSprite()
    {
        var playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed) transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);

        // Flip speech bubble if player is facing left so text is legible
        if (transform.localScale.x == -1)
            FindObjectOfType<SpeechBubble>().FlipSprite();
        else
            FindObjectOfType<SpeechBubble>().DefaultSprite();
    }

    public void DisableInput(float time)
    {
        inputEnabled = false;
        StartCoroutine(EnableInput(time));
    }

    private IEnumerator EnableInput(float time)
    {
        yield return new WaitForSeconds(time);
        inputEnabled = true;
    }
}