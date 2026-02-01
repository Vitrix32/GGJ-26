using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    Image notificationImage;
    // Start is called before the first frame update
    void Start()
    {
        notificationImage = GetComponent<Image>();
        notificationImage.color = new Vector4(255,255,255, 0);
    }

    // Update is called once per frame
    void Update()
    {
        notificationImage.color = new Vector4(255,255,255, notificationImage.color.a - 100 * Time.deltaTime);
    }

    public void SetTransparencyFull()
    {
        notificationImage.color = new Vector4(255, 255, 255, 255);
    }
}
