using UnityEngine;

public class WinScreenCamera : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer backgroundImage;
    private void Awake()
    {
        float spriteWidth = backgroundImage.bounds.size.x;
        float spriteHeight = backgroundImage.bounds.size.y;

        float screenAspect = (float)Screen.width / Screen.height;
        float spriteAspect = spriteWidth / spriteHeight;

        if (screenAspect >= spriteAspect)
        {
            // Screen is wider than sprite: Fit to Height
            Camera.main.orthographicSize = spriteHeight / 2f;
        }
        else
        {
            // Screen is narrower than sprite: Fit to Width
            Camera.main.orthographicSize = spriteWidth / (2f * screenAspect);
        }

        // Keep the camera centered on the sprite
        Vector3 spriteCenter = backgroundImage.bounds.center;
        transform.position = new Vector3(spriteCenter.x, spriteCenter.y, transform.position.z);
    }
}
