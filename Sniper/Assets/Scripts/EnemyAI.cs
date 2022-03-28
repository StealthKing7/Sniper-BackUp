using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private List<Collider> Ragdoll = new List<Collider>();
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
        Collider[] ragdoll = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider c in ragdoll)
        {
            if (c.gameObject != gameObject)
            {
                c.isTrigger = true;
                Ragdoll.Add(c);
            }
        }
    }
    public void TrunOnRagdoll()
    {
        foreach (Collider c in Ragdoll)
        {
            c.isTrigger = false;
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
}
