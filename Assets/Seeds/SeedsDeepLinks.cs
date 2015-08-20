using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

public class SeedsDeepLinks : MonoBehaviour
{
    public static SeedsDeepLinks Instance { get; private set; }

    /// <summary>
    /// True if trace should be enabled, false otherwise.
    /// </summary>
    public bool TraceEnabled = false;

    /// <summary>
    /// True if should use Unity3D plugin system.
    /// </summary>
    public bool RegisterAsPlugin = true;

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

    #if UNITY_IOS && !UNITY_EDITOR
    [DllImport ("__Internal")]
    private static extern void SeedsDeepLinks_SetGameObjectName(string gameObjectName);

    [DllImport ("__Internal")]
    private static extern void SeedsDeepLinks_Setup(bool registerAsPlugin);
    #endif

    void Start()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        using (var deepLinkActivityClass = new AndroidJavaClass("com.playseeds.unity3d.androidbridge.DeepLinkActivity"))
        {
            deepLinkActivityClass.CallStatic("setGameObjectName", gameObject.name);
        }
        #elif UNITY_IOS && !UNITY_EDITOR
        SeedsDeepLinks_SetGameObjectName(gameObject.name);
        SeedsDeepLinks_Setup(RegisterAsPlugin);
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
