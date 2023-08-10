using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(PlatformerCharacterMovement2D))]
public class PlayerController : MonoBehaviour
{
    private PlatformerCharacterMovement2D m_characterMovement;
    
    void Awake()
    {
        m_characterMovement = GetComponent<PlatformerCharacterMovement2D>();
    }
    
    void FixedUpdate()
    {
        m_characterMovement.AddMovementInput(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

        if (Input.GetAxis("Jump") > 0.0f)
        {
            m_characterMovement.Jump();
        }
    }
}
