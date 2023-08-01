using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyBehavior : MonoBehaviour
{
    public float health;
    public float runSpeed = 20f;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    public Animator bunnyAnimator;
    public SpriteRenderer bunnySprite;
    private Vector3 _currentTarget;
    public float closeEnough = 10f;


    void BunnyMovement()
    {
        
        float distanceA = Vector3.Distance(transform.position, pointA.position);
        float distanceB = Vector3.Distance(transform.position, pointB.position);

        if (distanceA <= closeEnough)
        {
          
            _currentTarget = pointB.position;
            bunnySprite.flipX = false;
        }
        else if (distanceB <= closeEnough)
        {
          
            _currentTarget = pointA.position;
            bunnySprite.flipX = true;
        }
        transform.position = Vector3.MoveTowards(transform.position, _currentTarget, runSpeed * Time.deltaTime);
    }
   



    
    public void Update()
    {
        BunnyMovement();
    }
}
