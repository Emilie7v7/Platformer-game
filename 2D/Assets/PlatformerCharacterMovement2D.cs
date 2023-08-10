using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum EMovementState
{
    Invalid = -1,
    Moving,
    Dashing,
    Climbing,
    Flying,
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerCharacterMovement2D : MonoBehaviour
{
    /* Properties */

    [Header("Movement")][Space]
    [SerializeField][Range(1.0f, 50.0f)][Tooltip("How fast can the character move")] 
    private float maxSpeed = 10;
    
    [SerializeField][Tooltip("How should the character accelerate")] 
    private AnimationCurve accelerationCurve;
    
    [SerializeField][Tooltip("Acceleration curve scale")] 
    private float accelerationScale = 1.0f;
    
    [SerializeField][Tooltip("How should the character decelerate")] 
    private AnimationCurve decelerationCurve;
    
    [SerializeField][Tooltip("Deceleration curve scale")] 
    private float decelerationScale = 1.0f;
    
    [SerializeField][Range(1.0f, 100.0f)][Tooltip("How fast should the character turn around")] 
    private float turnSpeed = 40.0f;

    
    [Header("Jumping")][Space]
    [SerializeField][Range(0.0f, 50.0f)][Tooltip("How high can the character jump")] 
    private float jumpHeight = 50.0f;
    
    [SerializeField][Range(0.01f, 5.0f)][Tooltip("How fast should the character reach the apex (highest point) of the jump")] 
    private float timeToJumpApex = 1.0f;
    
    [SerializeField][Range(0.0f, 5.0f)][Tooltip("How fast should the character stop jumping when button is released")] 
    private float jumpCutOff = 1.0f;
    
    [SerializeField][Range(0, 3)][Tooltip("How many times can the character jump in the air")] 
    private int airJumpAmount = 0;
    
    [SerializeField][Tooltip("Can the character stick to the walls")] 
    private bool canWallStick = true;
    
    [SerializeField][Tooltip("Can the character wall jump")]
    private bool canWallJump = true;

    
    [Header("Dash")][Space]
    [SerializeField][Range(2.0f, 40.0f)][Tooltip("How fast should the player dash")] 
    private float dashSpeed = 50.0f;
    
    [SerializeField][Range(0.01f, 2.0f)][Tooltip("Duration of dash in seconds")] 
    private float dashDuration = 1.0f;
    
    [SerializeField][Range(0.01f, 10.0f)][Tooltip("Minimum time between two dashes in seconds")] 
    private float dashCooldown = 1.0f;

    
    [Header("Physics")][Space]
    [SerializeField][Range(0.0f, 5.0f)][Tooltip("Default gravity scale")] 
    private float defaultGravityScale = 1.0f;
    
    [SerializeField][Range(0.0f, 5.0f)][Tooltip("Gravity scale when jumping (lower = faster jump)")] 
    private float jumpMultiplier = 1.0f;
    
    [SerializeField][Range(0.0f, 5.0f)][Tooltip("Gravity scale when falling (higher = faster fall)")] 
    private float fallMultiplier = 1.0f;
    
    [SerializeField][Range(0.0f, 5.0f)][Tooltip("Gravity scale when sticking to walls (higher = faster wall slide down)")] 
    private float wallStickMultiplier = 1.0f;

    
    [Header("Collider Settings")][Space]
    [SerializeField][Tooltip("Length of the ground-checking collider")] 
    private float groundCheckLength = 0.95f;
    
    [SerializeField][Tooltip("Distance between the ground-checking colliders")] 
    private Vector3 groundColliderOffset;
    
    [SerializeField][Tooltip("Length of the wall-checking collider")] 
    private float wallCheckLength = 0.95f;
    
    [SerializeField][Tooltip("Distance between the wall-checking colliders")] 
    private Vector3 wallColliderOffset;

    
    [Header("Layer Masks")]
    [SerializeField][Tooltip("Which layers are read as the ground")] 
    private LayerMask groundLayer;
 
    
    [Header("Debug")][Space]
    public bool drawVelocities;
    public bool drawGroundCheck;
    public bool drawWallCheck;

    /* Internal vars */
    private EMovementState movementState;
    private EMovementState previousMovementState;

    private Vector2 inputVector;
    private Vector2 lastInputVector;
    private Vector2 lastDirection;
    
    private bool isGrounded;
    private bool isWallSticking;

    // Jumping
    private float jumpHoldTime;
    private float gravMultiplier = 1.0f;
    private bool desiredJump;
    private bool holdingJump;
    private bool currentlyJumping;

    // Movement
    private Vector2 desiredVelocity;
    private float accelerationTime;
    private float maxAccelerationTime;
    private float decelerationTime;
    private float maxDecelerationTime;
    
    // Dashing
    private Vector2 dashDirection;
    private float dashTimeRemaining;
    private float currentDashCooldown;
    private bool canDash;

    /* Components */
    private Rigidbody2D m_Rigidbody2D;

    void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        
        accelerationTime = 0.0f;
        decelerationTime = 0.0f;
        maxAccelerationTime = accelerationCurve[accelerationCurve.length - 1].time;
        maxDecelerationTime = decelerationCurve[decelerationCurve.length - 1].time;
        
        SetMovementState(EMovementState.Moving);
    }

    void Update()
    {
        CheckGrounded();
        CalculateAccelerationAndDeceleration();

        currentDashCooldown -= Time.deltaTime;
        
        switch (movementState)
        {
            case EMovementState.Moving:
                lastDirection = new Vector2(inputVector.x, 0.0f).normalized;
                desiredVelocity = new Vector2(inputVector.x, 0.0f) * maxSpeed;
                float newGravity = -2 * jumpHeight / (timeToJumpApex * timeToJumpApex);
                m_Rigidbody2D.gravityScale = newGravity / Physics2D.gravity.y * gravMultiplier;
                
                if (isGrounded)
                {
                    canDash = true;
                }
                break;
            
            case EMovementState.Dashing:
                dashTimeRemaining -= Time.deltaTime;
                if (dashTimeRemaining <= 0.0f)
                {
                    SetMovementState(previousMovementState);
                }
                break;
            
            case EMovementState.Flying:

                break;
            
            case EMovementState.Climbing:

                break;
            
            case EMovementState.Invalid:
                // Just here so IDE's won't say it's not being used anywhere
                break;
        }
    }

    private void CalculateAccelerationAndDeceleration()
    {
        if (Mathf.Abs(inputVector.x) > 0.0f)
        {
            accelerationTime = Mathf.Min(accelerationTime + Time.deltaTime, maxAccelerationTime);
            decelerationTime = Mathf.Max(decelerationTime - Time.deltaTime, 0.0f);
        }
        else
        {
            decelerationTime = Mathf.Min(decelerationTime + Time.deltaTime, maxDecelerationTime);
            accelerationTime = Mathf.Max(accelerationTime - Time.deltaTime, 0.0f);
        }
    }

    private void FixedUpdate()
    {
        switch (movementState)
        {
            case EMovementState.Moving:
                HandleMovement();
                HandleJumping();
                break;
            
            case EMovementState.Dashing:
                m_Rigidbody2D.velocity = dashDirection * dashSpeed;
                break;
            
            case EMovementState.Flying:

                break;
            
            case EMovementState.Climbing:

                break;
            
            case EMovementState.Invalid:
                // Just here so IDE's won't say it's not being used anywhere
                break;
        }
    }

    private void HandleJumping()
    {
        Vector2 velocity = m_Rigidbody2D.velocity;
        if (desiredJump)
        {
            if (isGrounded || (canWallJump && isWallSticking))
            {
                float jumpSpeed = Mathf.Sqrt(-2.0f * Physics2D.gravity.y * m_Rigidbody2D.gravityScale * jumpHeight);

                if (velocity.y > 0.0f)
                {
                    jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0.0f);
                }
                else if (velocity.y < 0.0f)
                {
                    jumpSpeed += Mathf.Abs(m_Rigidbody2D.velocity.y);
                }

                if (canWallJump && isWallSticking)
                {
                    velocity = new Vector2(-Mathf.Sign(inputVector.x), 1.0f).normalized * jumpSpeed;
                } 
                else if (isGrounded)
                {
                    velocity.y += jumpSpeed;
                }
                
                currentlyJumping = true;
            }

            desiredJump = false;
            m_Rigidbody2D.velocity = velocity;
            return;
        }

        if (velocity.y > 0.01f)
        {
            if (isGrounded)
            {
                // Don't do anything if character is standing on something (moving platform etc.)
                gravMultiplier = defaultGravityScale;
            }
            else
            {
                if (holdingJump && currentlyJumping)
                {
                    gravMultiplier = jumpMultiplier;
                }
                else
                {
                    gravMultiplier = jumpCutOff;
                }
            }
        }
        else if (velocity.y < -0.01f)
        {
            if (isGrounded)
            {
                gravMultiplier = defaultGravityScale;     
            }
            else if (isWallSticking)
            {
                gravMultiplier = wallStickMultiplier;
            }
            else
            {
                gravMultiplier = fallMultiplier;     
            }
        }
        else
        {
            if (isGrounded)
            {
                currentlyJumping = false;
            }
            
            gravMultiplier = defaultGravityScale;
        }
    }

    private void HandleMovement()
    {
        float acceleration = accelerationCurve.Evaluate(accelerationTime) * accelerationScale;
        float deceleration = decelerationCurve.Evaluate(decelerationTime) * decelerationScale;

        // Debug.Log("Acc (time): " + acceleration + " (" + accelerationTime + "s)" + "\n\rDec (time): " + deceleration + " (" + decelerationTime + "s)");
        float maxSpeedChange = 0.0f;
        Vector2 velocity = m_Rigidbody2D.velocity;

        if (Mathf.Approximately(inputVector.x, 0.0f))
        {
            maxSpeedChange = deceleration * Time.deltaTime;
        }
        else
        {
            if (Mathf.Sign(inputVector.x) != Mathf.Sign(m_Rigidbody2D.velocity.x))
            {
                maxSpeedChange = turnSpeed * Time.deltaTime;
            }
            else
            {
                maxSpeedChange = acceleration * Time.deltaTime;
            }
        }

        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        m_Rigidbody2D.velocity = velocity;
    }

    public void SetMovementState(EMovementState newMovementState)
    {
        if (movementState == newMovementState)
        {
            return;
        }
        
        previousMovementState = movementState;
        movementState = newMovementState;
        
        switch (movementState)
        {
            case EMovementState.Moving:
                
                break;
            
            case EMovementState.Dashing:
                m_Rigidbody2D.gravityScale = 0.0f;
                dashTimeRemaining = dashDuration;
                currentDashCooldown = dashCooldown;
                break;
            
            case EMovementState.Flying:
                m_Rigidbody2D.gravityScale = 0.0f;
                break;
            
            case EMovementState.Climbing:
                m_Rigidbody2D.gravityScale = 0.0f;
                break;
        }
    }

    public void StartJumping()
    {
        desiredJump = true;
        holdingJump = true;
    }

    public void StopJumping()
    {
        holdingJump = false;
    }

    public void Dash()
    {
        if (!canDash || currentDashCooldown > 0.0f)
        {
            return;
        }
        
        canDash = false;
        dashDirection = inputVector == Vector2.zero ? lastInputVector : inputVector;
        SetMovementState(EMovementState.Dashing);
    }

    public void AddMovementInput(Vector2 movementInput)
    {
        if (inputVector != Vector2.zero)
        {
            lastInputVector = inputVector;
        }

        inputVector = movementInput;
    }

    private void CheckGrounded()
    {
        var position = transform.position;
        
        isGrounded = Physics2D.Raycast(position + groundColliderOffset, Vector2.down, groundCheckLength, groundLayer) ||
                     Physics2D.Raycast(position - groundColliderOffset, Vector2.down, groundCheckLength, groundLayer);

        if (canWallStick)
        {
            isWallSticking =
                Physics2D.Raycast(position + wallColliderOffset, lastDirection, wallCheckLength, groundLayer) ||
                Physics2D.Raycast(position - wallColliderOffset, lastDirection, wallCheckLength, groundLayer);
        }
    }

    private void OnDrawGizmos()
    {
        var position = transform.position;
        
        //Draw the ground colliders on screen for debug purposes
        if (drawGroundCheck)
        {
            if (isGrounded)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawLine(position + groundColliderOffset,
                position + groundColliderOffset + Vector3.down * wallCheckLength);
            Gizmos.DrawLine(position - groundColliderOffset,
                position - groundColliderOffset + Vector3.down * wallCheckLength);
        }
        
        if (drawWallCheck)
        {
            if (isWallSticking)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Vector3 directionVector = new Vector3(lastDirection.x, lastDirection.y) * wallCheckLength;
            Gizmos.DrawLine(position + wallColliderOffset,
                position + wallColliderOffset + directionVector);
            Gizmos.DrawLine(position - wallColliderOffset,
                position - wallColliderOffset + directionVector);
        }
        
        
        if (drawVelocities && m_Rigidbody2D)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, position + new Vector3(desiredVelocity.x, desiredVelocity.y) / 10.0f);
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(position, position + new Vector3(m_Rigidbody2D.velocity.x, m_Rigidbody2D.velocity.y) / 10.0f);
        }
    }
}
