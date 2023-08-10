using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LadderMovement : MonoBehaviour
{
    [SerializeField] private float ClimbingSpeed = 3.0f; 
    
    private Rigidbody2D rigidBody;

    private bool isClimbing;
    private int ladderCount = 0;
    private float verticalInput;
    private float prevGravityScale;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        prevGravityScale = rigidBody.gravityScale;
    }
    
    void Update() 
    {
        verticalInput = Input.GetAxis("Vertical");
        if (!isClimbing && ladderCount > 0 && Mathf.Abs(verticalInput) > 0f)
        {
            isClimbing = true;
        }
    }
    
    private void FixedUpdate() 
    { 
        if(isClimbing)
        {
            rigidBody.gravityScale = 0f; 
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, verticalInput * ClimbingSpeed);
        }
        else
        {
            rigidBody.gravityScale = prevGravityScale;
        }
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            ++ladderCount;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            if (--ladderCount <= 0)
            {
                isClimbing = false;
            }
        }
    }
}