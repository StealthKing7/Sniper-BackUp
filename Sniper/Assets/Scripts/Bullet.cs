using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float bulletSpeed;
    private Vector3 force;
    private Rigidbody rb;
    public void Initialized(float speed)
    {
        bulletSpeed = speed;

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
        EnemyAI enemyAI = other.gameObject.GetComponent<EnemyAI>();
        if(enemyAI != null)
        {
            Debug.Log("found");
            enemyAI.TrunOnRagdoll();
        }
        else
        {
            Debug.Log("not found");
        }
        Destroy(gameObject);
    }
}
    

