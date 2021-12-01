using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCharacterController : MonoBehaviour
{
    private float gravity = -50f;
    private CharacterController characterController;
    private Vector3 velocity;
    private Animator animator;
    private bool isGrounded;
    private float horizontalInput;
    private bool jumpPressed;
    private float jumpTimer;
    private float jumpGracePeriod = 0.2f;

    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private Transform[] groundChecks;
    [SerializeField] private Transform[] wallChecks;

    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private AudioClip jumpSoundEffect;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = 1;

        //CHARACTER ORIENTATION
        transform.forward = new Vector3(horizontalInput, 0, Mathf.Abs(horizontalInput) - 1);

        //CHARACTER GRAVITY

        //GroundCheck - checks if character is touching ground
        foreach (var groundCheck in groundChecks){
            if (Physics.CheckSphere(groundCheck.position, 0.1f, groundLayers, QueryTriggerInteraction.Ignore)) {
                isGrounded = true;
                break;
            }
        }

        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundLayers, QueryTriggerInteraction.Ignore);

        if (isGrounded && velocity.y < 0) {
            velocity.y = 0;
        }
        else{
            velocity.y += gravity * Time.deltaTime;
        }

        //jump Logic
        jumpPressed = Input.GetButtonDown("Jump");

        if (jumpPressed) {
            jumpTimer = Time.time;
        }
        if (isGrounded && (jumpPressed || (jumpTimer > 0 && Time.time < jumpTimer + jumpGracePeriod))){
            velocity.y += Mathf.Sqrt(jumpHeight * -2 * gravity);
            if (jumpSoundEffect != null){
                AudioSource.PlayClipAtPoint(jumpSoundEffect, transform.position, 0.5f);
            }
            jumpTimer = -1;
        }
        characterController.Move(velocity * Time.deltaTime);

        //CHARACTER FORWARD MOVEMENT

        //WallCheck - checks if character is touching wall
        var blocked = false;
        foreach (var wallCheck in wallChecks){
            if (Physics.CheckSphere(wallCheck.position, 0.01f, groundLayers, QueryTriggerInteraction.Ignore)) {
                blocked = true;
                break;
            }
        }
        if (!blocked){
            characterController.Move(new Vector3(horizontalInput * runSpeed, 0 ,0) * Time.deltaTime);
        }
        
        //Run animation variables
        animator.SetFloat("Speed", horizontalInput);

        animator.SetBool("IsGrounded", isGrounded);

        animator.SetFloat("VerticalSpeed", velocity.y);

    }
}
