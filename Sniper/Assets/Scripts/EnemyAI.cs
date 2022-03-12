using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private List<Rigidbody> Ragdoll = new List<Rigidbody>();
    private NavMeshAgent agent;
    private Animator animator;
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
        Rigidbody[] ragdoll = gameObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in ragdoll)
        {
            if (r.gameObject != gameObject)
            {
                r.isKinematic = true;
                Ragdoll.Add(r);
            }
        }
    }
    public void TrunOnRagdoll()
    {
        foreach (Rigidbody r in Ragdoll)
        {
            r.isKinematic = false;
        }
        animator.enabled = false;
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, Range);
    }
}
