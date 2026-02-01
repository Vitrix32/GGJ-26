using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer backgroundImage;

    private Transform playerTransform;

    private void Awake()
    {
        float backgroundWidth = backgroundImage.bounds.size.x;
        Camera.main.orthographicSize = backgroundWidth / (2f * Camera.main.aspect);
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        float camHeight = Camera.main.orthographicSize;
        float bgTop = backgroundImage.bounds.max.y;
        float bgBottom = backgroundImage.bounds.min.y;

        float topLimit = bgTop - camHeight;
        float bottomLimit = bgBottom + camHeight;

        float targetY = Mathf.Clamp(playerTransform.position.y, bottomLimit, topLimit);

        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
    }
}
