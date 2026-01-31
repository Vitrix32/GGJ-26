using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PointAndClick : MonoBehaviour
{
    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            agent.SetDestination(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}
