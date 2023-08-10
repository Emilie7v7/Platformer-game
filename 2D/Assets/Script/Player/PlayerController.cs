using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlatformerCharacterMovement2D))]
public class PlayerController : MonoBehaviour
{
    public InputActionAsset Actions;
    
    private PlatformerCharacterMovement2D m_characterMovement;
    
    void Awake()
    {
        m_characterMovement = GetComponent<PlatformerCharacterMovement2D>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        m_characterMovement.Dash();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            m_characterMovement.StartJumping();
        }
        else if (context.canceled)
        {
            m_characterMovement.StopJumping();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_characterMovement.AddMovementInput(context.ReadValue<Vector2>());
    }

    void FixedUpdate()
    {
        
    }
}
