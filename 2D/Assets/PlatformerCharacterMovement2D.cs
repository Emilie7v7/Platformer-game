using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMovementState
{
    Invalid = -1,
    Moving,
    Jumping,
    Climbing,
    Flying,
}

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerCharacterMovement2D : MonoBehaviour
{
    /* Properties */

    [Header("Movement")] 
    public float maxSpeed = 100;
    public AnimationCurve accelerationCurve;
    public AnimationCurve decelerationCurve;

    [Header("Jumping")] 
    // public float minJumpHeight;
    // public float maxJumpHeight;
    public AnimationCurve jumpHeight;
    public int jumpAmount = 1;

    [Header("Physics")] 
    public Vector2 gravity = new Vector2(0.0f, -9.8f);
    
    public bool drawDebug;
    
    /* Internal vars */
    private EMovementState movementState;
    private Vector2 inputVector;
    private Vector2 accelerationVector;
    
    private Vector2 acceleration;
    private Vector2 velocity;
    private bool isGrounded;
    private Bounds bounds;

    private float jumpHoldTime;
    private float decelerationTime = 0.0f;
    
    /* Components */
    private Rigidbody2D m_rigidbody2D;
    private Collider2D m_collider2D;
    
    void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_collider2D = GetComponent<Collider2D>();
        
        CalculateBounds();
        SetMovementState(EMovementState.Moving);
    }

    void Update()
    {
        CheckGrounded();
        
        switch (movementState)
        {
            case EMovementState.Moving:
                if (Mathf.Approximately(inputVector.x, 0.0f))
                {
                    decelerationTime = Mathf.Clamp(decelerationTime + Time.deltaTime, 0.0f,
                        decelerationCurve[decelerationCurve.length - 1].time);
                    
                }
                else
                {
                    decelerationTime = 0.0f;
                    accelerationVector = Vector2.ClampMagnitude(accelerationVector + inputVector, accelerationCurve[accelerationCurve.length - 1].time);
                }

                
                break;
            
            case EMovementState.Jumping:

                break;
            
            case EMovementState.Flying:

                break;
            
            case EMovementState.Climbing:

                break;
            
            case EMovementState.Invalid:
                // Just here so IDE's won't say it's not being used anywhere
                break;
        }

        
        ClearInputVector();
    }

    public void SetMovementState(EMovementState newMovementState)
    {
        movementState = newMovementState;
        switch (movementState)
        {
            case EMovementState.Moving:
                m_rigidbody2D.gravityScale = 1.0f;
                break;
            
            case EMovementState.Jumping:

                break;
            
            case EMovementState.Flying:
                m_rigidbody2D.gravityScale = 0.0f;
                break;
            
            case EMovementState.Climbing:
                m_rigidbody2D.gravityScale = 0.0f;
                break;
        }
    }

    public void Jump()
    {
        jumpHoldTime += Time.fixedDeltaTime;
    }

    public void AddMovementInput(Vector2 movementInput)
    {
        inputVector += movementInput;
    }

    private void CalculateBounds()
    {
        bounds = m_collider2D.bounds;
        bounds.center -= transform.position;
    }
    
    private void CheckGrounded()
    {
        isGrounded = true;
        // Physics2D.Raycast()
    }

    private void ClearInputVector()
    {
        inputVector = Vector3.zero;
    }

    private void OnDrawGizmos()
    {

        if (drawDebug)
        {
            Gizmos.DrawWireCube(transform.position + bounds.center, bounds.size);
        }
    }
}
