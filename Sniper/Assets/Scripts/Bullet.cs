using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float bulletSpeed;
    private Vector3 force;
    private Rigidbody rb;
    public void Initialized(float speed,Vector3 _force)
    {
        bulletSpeed = speed;
        force = _force;
    }
    private void Awake()
    { 
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        rb.velocity = transform.forward * bulletSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {

        Destroy(gameObject);
    }
}
