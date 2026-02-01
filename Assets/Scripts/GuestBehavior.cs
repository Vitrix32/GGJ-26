using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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

    public Coroutine WalkCoroutine;

    private RandomTalk randomTalk;

    public GameObject dialoguePopup;

    private Material mat;

    private void Awake()
    {
        dialoguePopup = GameObject.Find("DialoguePopup");
    }

    void Start()
    {
        //dialoguePopup.SetActive(false);
        randomTalk = gameObject.GetComponent<RandomTalk>();
        if (SceneManager.GetActiveScene().name != "WinScene")
        {
            randomTalk.StartRandomTalking();
        }

        player = GameObject.Find("MainCharacter");
        pointAndClick = player.GetComponent<PointAndClick>();

        startLocalPos = spriteTransform.localPosition;
        spriteRenderer = spriteTransform.gameObject.GetComponent<SpriteRenderer>();
        mat = spriteRenderer.material;

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
        if (SceneManager.GetActiveScene().name != "WinScene")
        {
            WalkCoroutine = StartCoroutine(RandomlyWalk());
        }
    }

    void Update()
    {

        // Use actual movement speed
        if (SceneManager.GetActiveScene().name != "WinScene")
        {
            float speed = agent.velocity.magnitude;

            if (speed > 0.05f && (!pointAndClick.isTalking || pointAndClick.isTalkingTo != gameObject))
            {
                if (agent.velocity.x > 0)
                {
                    spriteRenderer.flipX = true;
                    maskRenderer.flipX = true;
                }
                else
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

    public void StopWalking()
    {
        if (WalkCoroutine != null)
        {
            StopCoroutine(WalkCoroutine);
        }
    }

    private void OnMouseDown()
    {
        if (SceneManager.GetActiveScene().name == "WinScene")
        {
            return;
        }
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
            pointAndClick.maskSpriteRenderer.flipX = false;
        }
        else
        {
            pointAndClick.SetWalkPosition(transform.position + new Vector3(-1f, 0, 0));
            spriteRenderer.flipX = false;
            maskRenderer.flipX = false;
            pointAndClick.spriteRenderer.flipX = true;
            pointAndClick.maskSpriteRenderer.flipX = true;
        }

        StartCoroutine(startTalk());
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //LoseMask();
        }
    }

    public IEnumerator startTalk()
    {
        while (Vector3.Distance(player.transform.position, transform.position) > 3)
        {
            yield return null;
        }
        //dialoguePopup.SetActive(true);
        dialoguePopup.GetComponent<DialogueOptionsController>().fadeIn();
        var dialogueEntry = DialogueFetcher.Instance.FetchRootDialogue(
            GetComponent<Ghost>().Name, 
            GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<GhostMask>().Mask
        );
        dialoguePopup.GetComponent<DialogueOptionsController>().setupImages(GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<GhostMask>().Mask,GetComponent<Ghost>().Name);
        dialoguePopup.GetComponent<DialogueOptionsController>().setupDialogue(dialogueEntry);
        dialoguePopup.GetComponent<DialogueOptionsController>().dialogueFinished += ReturnFromTalking;
    }

    public void ReturnFromTalking()
    {
        //dialoguePopup.SetActive(false);
        pointAndClick.isTalking = false;
        WalkCoroutine = StartCoroutine(RandomlyWalk());
        randomTalk.StartRandomTalking();
    }

    public ParticleSystem revealParticles;

    [Header("Throw Settings")]
    public float throwDistance = 2.5f;
    public float throwDuration = 0.6f;
    public float spinSpeed = 720f;

    [Header("Bounce Illusion")]
    public AnimationCurve bounceCurve;

    [Header("Fade")]
    public float fadeDuration = 1.2f;

    public void LoseMask()
    {
        if (SceneManager.GetActiveScene().name != "WinScene")
        {
            agent.SetDestination(transform.position);
            randomTalk.StopRandomTalking();
            StopCoroutine(WalkCoroutine);
        }
        StartCoroutine(MaskSequence());
    }

    IEnumerator MaskSequence()
    {
        Transform mask = maskRenderer.gameObject.transform;
        mask.parent = null;

        Vector3 startPos = mask.position;

        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 endPos = startPos + (Vector3)(dir * throwDistance);

        if (revealParticles)
        {
            revealParticles.transform.position = startPos;
            revealParticles.Play();
        }

        float time = 0f;

        while (time < throwDuration)
        {
            time += Time.deltaTime;
            float t = time / throwDuration;

            // Horizontal movement
            mask.position = Vector3.Lerp(startPos, endPos, t);

            // Fake bounce (vertical illusion)
            float bounce = bounceCurve.Evaluate(t);
            mask.position += Vector3.up * bounce * 0.3f;

            // Spin
            mask.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

            yield return null;
        }

        Destroy(mask.gameObject);
        StartCoroutine(FadeOutCharacter());
    }

    IEnumerator FadeOutCharacter()
    {
        float t = 0f;
        Color start = spriteRenderer.color;
        Debug.Log(mat.HasProperty("_Opacity"));
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            mat.SetFloat("_Fade", a);
            Debug.Log(a);
            yield return null;
        }
        revealParticles.Stop();
        yield return new WaitForSeconds(fadeDuration);
        Destroy(gameObject);
    }
}
