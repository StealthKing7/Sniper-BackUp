using System.Collections;
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
    void Awake()
    {
        SetRagDollOff();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
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

        if (CameraController.Instance.HadFired)
        {
            agent.SetDestination(new Vector3(100f, 100f, 100f));
            animator.SetBool("Run", true);
        }

    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, Range);
    }
}
