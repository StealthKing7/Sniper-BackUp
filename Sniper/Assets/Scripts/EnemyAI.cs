using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private List<Rigidbody> Ragdoll = new List<Rigidbody>();
    private NavMeshAgent agent;

    void Awake()
    {
        SetRagDollOff();
        agent = GetComponent<NavMeshAgent>();
    }
    void SetRagDollOff()
    {
        Rigidbody[] ragdoll = gameObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in ragdoll)
        {
            if (r.gameObject != this.gameObject)
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
    }
    // Update is called once per frame
    void Update()
    {
     //agent.SetDestination(Vector3.zero);
    }
}
