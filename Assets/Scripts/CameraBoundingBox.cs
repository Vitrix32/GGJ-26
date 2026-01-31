using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundingBox : MonoBehaviour
{
    public BoxCollider2D bounds;

    GameObject player;
    private Camera cam;

    private void Start()
    {
        player = GameObject.Find("MainCharacter");
        cam = GetComponent<Camera>();
    }



    void LateUpdate()
    {
        Vector3 desiredPos = player ? player.transform.position : transform.position;

        // Get the half-size of the camera at the current distance
        float halfHeight = cam.orthographic ? cam.orthographicSize : Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * Vector3.Distance(transform.position, desiredPos);
        float halfWidth = halfHeight * cam.aspect;

        // Get bounds
        Vector3 min = bounds.bounds.min;
        Vector3 max = bounds.bounds.max;

        // Clamp so camera stays fully inside
        desiredPos.x = Mathf.Clamp(desiredPos.x, min.x + halfWidth, max.x - halfWidth);
        desiredPos.y = Mathf.Clamp(desiredPos.y, min.y + halfHeight, max.y - halfHeight);
        desiredPos.z = -10;

        transform.position = desiredPos;
    }
}
