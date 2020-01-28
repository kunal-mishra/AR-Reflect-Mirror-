using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorController : MonoBehaviour
{

    private WebCamTexture webCamTexture;

    // Use this for initialization
    void Start()
    {
        
    }

    public void Play()
    {
        if (webCamTexture == null)
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            foreach (WebCamDevice device in devices)
            {
                if (device.isFrontFacing)
                {

                    webCamTexture = new WebCamTexture(device.name);

                    GetComponent<Renderer>().material.mainTexture = webCamTexture;
                    webCamTexture.Play();
                    break;
                }
            }
            Debug.Log("Play!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
