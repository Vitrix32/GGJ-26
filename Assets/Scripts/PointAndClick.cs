using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PointAndClick : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] private Animator animator;

    public bool isTalking = false;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer maskSpriteRenderer;

    public GuestBehavior isTalkingTo;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        float speed = agent.velocity.magnitude;
        if (speed > 0.05f && !isTalking)
        {
            if (agent.velocity.x > 0)
            {
                spriteRenderer.flipX = true;
                maskSpriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
                maskSpriteRenderer.flipX = false;
            }
        }

            if (Input.GetMouseButtonDown(0) && !isTalking)
        {
            Vector3 clickLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickLoc.z = 0;
            agent.SetDestination(clickLoc);
        }

        if (agent.remainingDistance > 0)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    public void SetWalkPosition(Vector3 loc)
    {
        agent.SetDestination(loc);
    }
}
