using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private List<Collider> Ragdoll = new List<Collider>();
    [SerializeField]
    private CapsuleCollider capsuleCollider;
    [SerializeField]
    private Rigidbody rb;
    private Animator animator;
    private NavMeshAgent agent;
    [SerializeField]
    private float Range;
    private CameraController cameraController;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float speed;
    void Awake()
    {
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        SetRagDollOff();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        agent.speed = speed;
    }
    void SetRagDollOff()
    {
        Collider[] ragdoll = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider c in ragdoll)
        {
            if (c.gameObject != gameObject)
            {
                c.isTrigger = true;
                Ragdoll.Add(c);
            }
        }
        capsuleCollider.isTrigger = false;
        rb.isKinematic = false;
    }
    public void TrunOnRagdoll()
    {
        animator.enabled = false;
        foreach (Collider c in Ragdoll)
        {
            c.isTrigger = false;
        }
        capsuleCollider.isTrigger = true;
        rb.isKinematic = true;
        agent.isStopped = true;
    }
    void Update()
    {
        float dis = Vector3.Distance(transform.position, cameraController.gameObject.transform.position);
        if (cameraController.HadFired == true && dis <= cameraController.Range)
        {
            agent.SetDestination(target.position);
            animator.SetBool("Run", true);
        }
    }
    /*private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.tag == "Bullet")
        {
            Debug.Log("Got Hit");
        }
    }*/
}
