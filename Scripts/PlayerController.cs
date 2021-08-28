using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using TMPro;

public class PlayerController : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField, Header("Movement")] float moveSpeed;
    [SerializeField, Range(0,1)] float moveDampen;
    [SerializeField] SpeakerData speakerData;
    [SerializeField] TextMeshProUGUI nameinputfield;

    //[SerializeField] YarnProgram yarnDialog;
    //[SerializeField] string yarnStartNode;
#pragma warning restore 0649

    Vector2 moveVelocity, moveTarget, moveCurrent;
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    bool isInDialog = false;

    // healthbar stuff
    private int maxEnergy = 30;
    public HealthBar healthBar;
    public InventoryManager inventoryManager;

    private string yourname;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        DialogUI.Instance.AddSpeaker(speakerData);

        //Make sure that all RPG UI elements are set to correct values at game start
        healthBar.SetMaxHealth(maxEnergy);
        inventoryManager.LoadAllCards();
        inventoryManager.DisplayXPs();
    }

    public void SetMoveSpeed(float newspeed)
    {
        moveSpeed = newspeed;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.SetHealth(DialogUI.Instance.energyPoints);
        // If in dialog - early out
        if (isInDialog) return;

        //movement & animation
        //Handle all movement input here:
        moveTarget.x = Input.GetAxisRaw("Horizontal"); //-1 for left-key (e.g. A or left arrow), +1 for right-key, else 0
        moveTarget.y = Input.GetAxisRaw("Vertical"); // up & down
        animator.SetFloat("Horizontal", moveTarget.x);
        animator.SetFloat("Vertical", moveTarget.y);
        animator.SetFloat("Speed", moveTarget.sqrMagnitude);

        //GetMovement();
        Interact();
    }

    public void ChangeName()
    {
        yourname = nameinputfield.text;
        Debug.Log(yourname);
    }
    
    
    private void FixedUpdate()
    {
        Move();
    }
    
    /*
    void GetMovement()
    {
        // Target movement input
        moveTarget.x = Input.GetAxisRaw("Horizontal");
        moveTarget.y = Input.GetAxisRaw("Vertical");

        // Normalize and set magnitude
        moveTarget = moveTarget.normalized * moveSpeed;
    }
    */

    
    void Move()
    {
        /*
        if (FindObjectOfType<DialogueRunner>().IsDialogueRunning == true)
        {
            animator.SetInteger("DirectionX", 0);
            animator.SetInteger("DirectionY", 0);
            return;
        }
        */

        // Define movement on grid
        moveCurrent = Vector2.SmoothDamp(moveCurrent, moveTarget, ref moveVelocity, moveDampen);
        float speed = moveCurrent.sqrMagnitude;
        rb.MovePosition(rb.position + moveTarget * moveSpeed * Time.fixedDeltaTime);
        /*

        // Add velocity to rb
        Vector2 newPos = rb.position + moveCurrent * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
        
        // Set animator
        animator.SetFloat("Movement Speed", speed);

        // Flip sprite
        spriteRenderer.flipX = moveCurrent.x < 0; 
        */
    }
    
        public void OnDialogEnd()
    {
        isInDialog = false;
    }
    void Interact()
    {
        // Check input
        if (Input.GetButtonDown("Jump"))
        {
            // Check if NPC is active and not already talking
            if(NPC.ActiveNPC && !isInDialog)
            {
                // Start dialog
                isInDialog = true;
                DialogUI.Instance.dialogueRunner.StartDialogue(NPC.ActiveNPC.YarnStartNode);
            }
        }
    }
}
