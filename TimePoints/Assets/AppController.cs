using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppController : MonoBehaviour
{
    string[] paths;


    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        UnityEngine.iOS.Device.hideHomeButton = true;

        //NativeGallery.GetImagesFromGallery(callback);
    }

    private void callback(string[] _paths)
    {
        paths = _paths;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
