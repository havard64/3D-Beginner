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

    void Start()
    {
        navMeshAgent.SetDestination(waypoints[0].position);
    }


    bool ICanSee(Transform player)
    {
        Vector3 direction = player.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, direction, out hit) && hit.collider.gameObject.tag == "Player" && direction.magnitude < fovDist && angle < focAngle)
        {
            Debug.Log("Can see player at position: " + player.position);
            return true;
        }
        Debug.Log("I can't see the player" + this.transform.position + "player position: " + player.position + "direction: " + direction + "angle: " + angle + "magnitude: " + direction.magnitude + "hit: " + Physics.Raycast(this.transform.position, direction, out hit) + "tag: " + hit.collider.gameObject.tag);
        return false;
    }


    void Update()
    {
        ICanSee(player);
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        }
    }
}
