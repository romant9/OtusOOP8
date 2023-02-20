using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//просто тест, в сцене не используется
public class CollisionTest : MonoBehaviour
{
    private bool collisionTop;
    private bool collisionBottom;
    private bool collisionLeft;
    private bool collisionRight;
    public float rayLength = 1;
    public LayerMask mask;

    public enum DamageType
    {
        Phisycal,
        Fire,
        Ice,
        Poison,
        None
    };
    public List<DamageType> damages = new List<DamageType>() { 
        DamageType.Phisycal,
        DamageType.Ice,
        DamageType.Poison};

    void Start()
    {
        bool damageIsContaine =  damages.Where(x => x == DamageType.Fire).Count() > 0;
        bool damageIsContaine2 =  damages.Contains(DamageType.Fire);
        
        Debug.Log(damageIsContaine + " " + damageIsContaine2);
    }

    private void FixedUpdate()
    {
        collisionTop = Physics2D.Raycast(transform.position, Vector2.up, rayLength,mask);
        collisionBottom = Physics2D.Raycast(transform.position, Vector2.down, rayLength,mask);
        collisionLeft = Physics2D.Raycast(transform.position, Vector2.left, rayLength, mask);
        collisionRight = Physics2D.Raycast(transform.position, Vector2.right, rayLength, mask);

        if (collisionTop)
        {
            Debug.DrawRay(transform.position, Vector2.up, Color.blue);
            Debug.Log($"top: {collisionTop}");
        }
        if (collisionBottom)
        {
            Debug.DrawRay(transform.position, Vector2.down, Color.blue);
            Debug.Log($"bottom: {collisionBottom}");
        }
        if (collisionLeft)
        {
            Debug.DrawRay(transform.position, Vector2.left, Color.blue);
            Debug.Log($"left: {collisionLeft}");
        }
        if (collisionRight)
        {
            Debug.DrawRay(transform.position, Vector2.right, Color.blue);
            Debug.Log($"right: {collisionRight}");
        }
    }
}
