using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Patroling : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform nestPosition;


    Vector3 PatrolPosition()
    {
        Vector3 rand = Random.insideUnitCircle * patrolRadius;
        Vector3 point = nestPosition.position + new Vector3(rand.x, rand.y,0);
        return point;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;
    }

    private void Update()
    {
        agent.speed = moveSpeed;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }

    private void GoToNextPatrolPoint()
    {
        agent.SetDestination(PatrolPosition());
    }

    void Start()
    {
        GoToNextPatrolPoint();
    }
}
