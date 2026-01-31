using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct KeyValuePair
{
    public GuestBehavior Key;
    public float Value;
}

public class GuestBehavior : MonoBehaviour
{
    [SerializeField]
    public List<KeyValuePair> affinity;

    public float moveDistance = 0.5f;

    NavMeshAgent agent;

    public Transform spriteTransform;
    private SpriteRenderer spriteRenderer;
    public SpriteRenderer maskRenderer;

    [Header("Bob Settings")]
    public float bobAmplitude = 0.05f;
    public float bobFrequency = 8f;
    public float idleReturnSpeed = 12f;
    
    private Vector3 startLocalPos;
    private float bobTimer;

    private GameObject player;
    private PointAndClick pointAndClick;

    private Coroutine WalkCoroutine;

    private RandomTalk randomTalk;

    void Start()
    {
        randomTalk = gameObject.GetComponent<RandomTalk>();
        randomTalk.StartRandomTalking();

        player = GameObject.Find("MainCharacter");
        pointAndClick = player.GetComponent<PointAndClick>();

        startLocalPos = spriteTransform.localPosition;
        spriteRenderer = spriteTransform.gameObject.GetComponent<SpriteRenderer>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        WalkCoroutine = StartCoroutine(RandomlyWalk());
    }

    void Update()
    {
        // Use actual movement speed
        float speed = agent.velocity.magnitude;

        if (speed > 0.05f && (!pointAndClick.isTalking || pointAndClick.isTalkingTo != gameObject))
        {
            if (agent.velocity.x > 0)
            {
                spriteRenderer.flipX = true;
                maskRenderer.flipX = true;
            } else
            {
                spriteRenderer.flipX = false;
                maskRenderer.flipX = false;
            }

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
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (true)
        {
            Vector3 origin = transform.position;

            Vector3 randomOffset = new Vector3(
                Random.Range(-moveDistance, moveDistance),
                0,
                Random.Range(-moveDistance, moveDistance)
            );

            Vector3 socialBias = Vector3.zero;
            float influenceRadius = 6f;

            foreach (var pair in affinity)
            {
                GuestBehavior other = pair.Key;
                float affinityValue = pair.Value;

                if (other == null) continue;

                Vector3 toOther = other.transform.position - origin;
                float dist = toOther.magnitude;

                if (dist > influenceRadius || dist < 0.01f) continue;

                Vector3 dir = toOther / dist;
                float falloff = 1f - (dist / influenceRadius);

                socialBias += dir * affinityValue * falloff;
            }

            Vector3 desiredOffset =
                randomOffset +
                socialBias.normalized * moveDistance * 0.3f;

            Vector3 desiredPos = origin + desiredOffset;

            bool found = false;
            NavMeshHit hit;

            // Try original + rotated directions
            Vector3 direction = desiredOffset.normalized;
            float distance = desiredOffset.magnitude;

            float[] angles = { 0f, 180f, 90f, -90f, 45f, -45f, 135f, -135f };

            foreach (float angle in angles)
            {
                Vector3 rotatedDir = Quaternion.Euler(0, angle, 0) * direction;
                Vector3 testPos = origin + rotatedDir * distance;

                if (NavMesh.SamplePosition(testPos, out hit, moveDistance, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                    found = true;
                    break;
                }
            }

            // If literally nothing worked, don't move this cycle
            if (!found)
            {
                // optional: very small nudge inward
                Vector3 fallback = origin - direction * 0.5f;
                if (NavMesh.SamplePosition(fallback, out hit, 1f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }

            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }

    private void OnMouseDown()
    {
        randomTalk.StopRandomTalking();
        agent.SetDestination(transform.position);
        if (pointAndClick.isTalking)
        {
            return;
        }
        pointAndClick.isTalking = true;
        pointAndClick.isTalkingTo = this;
        StopCoroutine(WalkCoroutine);
        if (player.transform.position.x > transform.position.x)
        {
            pointAndClick.SetWalkPosition(transform.position + new Vector3(1f,0,0));
            spriteRenderer.flipX = true;
            maskRenderer.flipX = true;
            pointAndClick.spriteRenderer.flipX = false;
        }
        else
        {
            pointAndClick.SetWalkPosition(transform.position + new Vector3(-1f, 0, 0));
            spriteRenderer.flipX = false;
            maskRenderer.flipX = false;
            pointAndClick.spriteRenderer.flipX = true;
        }

        StartCoroutine(startTalk());
    }

    public IEnumerator startTalk()
    {
        while (Vector3.Distance(player.transform.position, transform.position) > 3)
        {
            yield return null;
        }
        var camera = GameObject.Find("DialoguePopup");
        camera.GetComponent<DialogueOptionsController>().fadeIn();
        var dialogueEntry = DialogueFetcher.Instance.FetchDialogue(GetComponent<Ghost>().Name);
        camera.GetComponent<DialogueOptionsController>().setupDialogue(dialogueEntry);
        camera.GetComponent<DialogueOptionsController>().dialogueFinished += ReturnFromTalking;
    }

    public void ReturnFromTalking()
    {
        pointAndClick.isTalking = false;
        WalkCoroutine = StartCoroutine(RandomlyWalk());
        randomTalk.StartRandomTalking();
    }
}
