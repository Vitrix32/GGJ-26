using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuestBehavior : MonoBehaviour
{
    public float moveDistance = 0.5f;

    NavMeshAgent agent;

    public Transform spriteTransform;

    [Header("Bob Settings")]
    public float bobAmplitude = 0.05f;
    public float bobFrequency = 8f;
    public float idleReturnSpeed = 12f;

    private Vector3 startLocalPos;
    private float bobTimer;

    void Start()
    {
        startLocalPos = spriteTransform.localPosition;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        StartCoroutine(RandomlyWalk());
    }

    void Update()
    {
        // Use actual movement speed
        float speed = agent.velocity.magnitude;

        if (speed > 0.05f)
        {
            bobTimer += Time.deltaTime * speed;
            float bobOffset =
                Mathf.Sin(bobTimer * bobFrequency) *
                bobAmplitude *
                Mathf.Clamp01(speed / agent.speed);
            spriteTransform.localPosition = startLocalPos + Vector3.up * bobOffset;
        }
        else
        {
            // Smoothly return to idle position
            bobTimer = 0f;
            spriteTransform.localPosition = Vector3.Lerp(
                spriteTransform.localPosition,
                startLocalPos,
                Time.deltaTime * idleReturnSpeed
            );
        }
    }

    private IEnumerator RandomlyWalk()
    {
        while (true)
        {
            Vector2 newPos = transform.position;
            newPos += new Vector2(Random.Range(-moveDistance, moveDistance), Random.Range(-moveDistance, moveDistance));
            agent.SetDestination(newPos);
            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }

        yield return null;
    }
}
