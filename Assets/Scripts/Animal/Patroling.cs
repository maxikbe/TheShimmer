using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Patroling : MonoBehaviour
{
    private NavMeshAgent agent;
    private enum State { Patrolling, Fleeing, Returning }
    private State currentState;
    private Vector3 currentFleeTarget;
    
    
    
    [SerializeField] private bool isFriendly = true;
    
    [SerializeField] private Transform playerPosition;
    [SerializeField] private Transform nestPosition;
    
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float minRunningDistance = 1f;
    [SerializeField] private float maxRunningDistance = 3f;
    
    
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float runningRadius = 10f;
    
    [SerializeField] private float visionRadius = 3f;
    [SerializeField] private LayerMask wallLayer;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;
    }

    void Start()
    {
        currentState = State.Patrolling;
        GoToNextPatrolPoint();
    }
    private void Update()
    {
        if (!isFriendly)
        {
            PatrolLogic();
            return;
        }
        bool canSeePlayer = CheckForPlayer();
        if (canSeePlayer)
        {
            currentState = State.Fleeing;
            RunAwayFromPlayer();
        }
        else
        {
            switch (currentState)
            {
                case State.Fleeing:
                    if(!agent.pathPending && agent.remainingDistance < 0.5f)
                    {
                        currentState = State.Patrolling;
                        PatrolLogic();
                    }
                    break;
                
                case State.Patrolling:
                    PatrolLogic();
                    break;
            }
        }
        
    }
    
    

    // Patroling
    Vector3 PatrolPosition()
    {
        Vector3 rand = Random.insideUnitCircle * patrolRadius;
        Vector3 point = nestPosition.position + new Vector3(rand.x, rand.y,0);
        return point;
    }

    private void GoToNextPatrolPoint()
    {
        agent.SetDestination(PatrolPosition());
    }

    private void PatrolLogic()
    {
        agent.speed = moveSpeed;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }
    
    
    // Fleeing
    private bool CheckForPlayer()
    {
        if (playerPosition == null) return false;
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition.position);
        
        if (distanceToPlayer > visionRadius) return false;
        
        Vector3 directionToPlayer = (playerPosition.position - transform.position).normalized;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, visionRadius, wallLayer | (1 << playerPosition.gameObject.layer));

        if (hit.collider != null)
        {
            if (hit.transform == playerPosition)
            {
                return true;
            }
        }

        return false;
    }

    private Vector3 GetFleePoint(Vector3 directionAwayFromPlayer)
    {
        float randomAngle = Random.Range(-60f, 60f);
        float randomDistance = Random.Range(minRunningDistance, maxRunningDistance);
        
        Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.up);
        Vector3 finalDirection = rotation * directionAwayFromPlayer;

        return transform.position + finalDirection * randomDistance;
    }

    private Vector3 GetFLeePointCloserToNest(Vector3 directionAwayFromPlayer)
    {
        
        Vector3 directionToNest = (nestPosition.position - transform.position).normalized;
        
        Vector3 leftDirection = Quaternion.AngleAxis(-60f, Vector3.forward) * directionAwayFromPlayer;
        Vector3 rightDirection = Quaternion.AngleAxis(60f, Vector3.forward) * directionAwayFromPlayer;
        
        float dotLeft = Vector3.Dot(leftDirection, directionToNest);
        float dotRight = Vector3.Dot(rightDirection, directionToNest);

        Vector3 bestDirection;

        if (dotLeft > dotRight)
        {
            bestDirection = leftDirection;
        }
        else
        {
            bestDirection = rightDirection;
        }
        
        float jitterAngle = Random.Range(-10f, 10f);
        float randomDistance = Random.Range(minRunningDistance, maxRunningDistance);
        
        
        bestDirection = Quaternion.AngleAxis(jitterAngle, Vector3.forward) * bestDirection;
        
        return transform.position + bestDirection * randomDistance;
    }

    private void RunAwayFromPlayer()
    {
        agent.speed = runSpeed;
        
        Vector3 fleeDestionation;
        float distanceToNest = Vector3.Distance(transform.position, nestPosition.position);
        
        Vector3 directionAwayFromPlayer = (transform.position - playerPosition.position).normalized;
        
        if (distanceToNest < runningRadius)
        {
            fleeDestionation = GetFLeePointCloserToNest(directionAwayFromPlayer);
        }
        else
        {
            fleeDestionation = GetFleePoint(directionAwayFromPlayer);
        }
        agent.SetDestination(fleeDestionation);
    }
    
    
    
    
}
