using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float bulletSpeed;
    private Vector3 force;
    private Rigidbody rb;
    public void Initialized(float speed, Vector3 _force)
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
        Debug.Log("vfds");
        Debug.Log(other.gameObject.name,other.gameObject);
        EnemyAI enemyAI = other.GetComponentInParent<EnemyAI>();
        if (enemyAI != null)
        {
            Debug.Log("Hit");
            enemyAI.TrunOnRagdoll();
            Rigidbody[] rbs = enemyAI.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                rb.AddForce(force);
            }
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
    

