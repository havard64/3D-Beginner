using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{

    public NavMeshAgent navMeshAgent;
    public Transform[] waypoints;
    int m_CurrentWaypointIndex;
    public Transform player;

    // Field of view setttings
    float fovDist = 20.0f;
    float focAngle = 45.0f;

    // FSM
    enum State { Patrol, Investigate, Chase };
    State curState = State.Patrol;

    // Patrol settings
    float patrolWait = 2.0f;
    float patrolTimePassed = 0;

    // Last place the player was seen
    Vector3 lastPlaceSeen;

    void Start()
    {
        navMeshAgent.SetDestination(waypoints[0].position);
        lastPlaceSeen = this.transform.position;
    }


    bool ICanSee(Transform player)
    {
        Vector3 direction = player.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, direction, out hit) && hit.collider.gameObject.tag == "Player" && direction.magnitude < fovDist && angle < focAngle)
        {
            return true;
        }
        return false;
    }

    void Patrol()
    {
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
            {
                m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            }
    }

    void Investigate()
    {
        // if the agent has arrived at the investigating goal, the agent should start to patrol
        if (Vector3.Distance(this.transform.position, lastPlaceSeen) < 0.04)
        {
            patrolTimePassed += Time.deltaTime;
            if (patrolTimePassed > patrolWait)
            {
                navMeshAgent.ResetPath();
                navMeshAgent.SetDestination(waypoints[0].position);
                curState = State.Patrol;
                patrolTimePassed = 0;
            }
        }
        else
        {
            navMeshAgent.SetDestination(lastPlaceSeen);
        }
    }

    void Chase()
    {
        navMeshAgent.ResetPath();
        navMeshAgent.SetDestination(player.position);
    }

    void Update()
    {
        State tmpState = curState; // Check if the state has changed


        if (ICanSee(player))
        {
            curState = State.Chase;
            lastPlaceSeen = player.position;
        }
        else
        {
            if (curState == State.Chase)
            {
                curState = State.Investigate;
            }
        }  

        // State check
        switch(curState)
        {
            case State.Patrol: // Start patrolling
                Patrol();
                break;
            case State.Investigate: // Start investigate
                Investigate();
                break;
            case State.Chase: // Start chasing
                Chase();
                break;
        }
    }
}
