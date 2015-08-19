using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedsDeepLinks : MonoBehaviour
{
    public static SeedsDeepLinks Instance { get; private set; }

    /// <summary>
    /// True if trace should be enabled, false otherwise.
    /// </summary>
    public bool TraceEnabled = false;

    public event Action<string> OnLinkArrived;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Duplicate instance of SeedsDeepLinks. New instance will be destroyed.");
            enabled = false;
            DestroyObject(this);
            return;
        }

        DontDestroyOnLoad(this);
        Instance = this;
    }

    void Start()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        using (var deepLinkActivityClass = new AndroidJavaClass("com.playseeds.unity3d.androidbridge.DeepLinkActivity"))
        {
            deepLinkActivityClass.CallStatic("setGameObjectName", gameObject.name);
        }
        #endif
    }

    void onLinkArrived(string url)
    {
        if (TraceEnabled)
            Debug.Log("[SeedsDeepLinks] Link arrived: " + url);

        if (OnLinkArrived != null)
            OnLinkArrived(url);
    }
}
