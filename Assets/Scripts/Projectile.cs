using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10.0f;
    new private Rigidbody rigidbody;
    private Vector3 direction;

    void SelfDestruct()
    {
        Destroy(this.gameObject);
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection;
        
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = direction * speed;
    }
    
    void Start()
    {
        Invoke(nameof(SelfDestruct), 1f);
    }
}
