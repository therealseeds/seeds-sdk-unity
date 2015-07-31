using UnityEngine;
using System.Collections;

public class Seeds : MonoBehaviour
{
    public enum DeviceIdMode
    {
        DeveloperSupplied,
        OpenUDID,
        AdvertisingId,
    }

    public static Seeds Instance { get; private set; }

    /// <summary>
    /// True if Seeds SDK should auto-initialize during instantiation, false otherwise.
    /// </summary>
    public bool AutoInitialize = false;

    /// <summary>
    /// Server URL. Do not include trailing slash.
    /// </summary>
    public string ServerURL = "http://devdashboard.playseeds.com";

    /// <summary>
    /// Application API key.
    /// </summary>
    public string ApplicationKey;

    #if UNITY_ANDROID
    private AndroidJavaObject androidInstance;
    #endif

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Duplicate instance of Seeds. New instance will be destroyed.");
            enabled = false;
            DestroyObject(this);
            return;
        }

        DontDestroyOnLoad(this);
        Instance = this;
    }

    void Start()
    {
        #if UNITY_ANDROID
        using (var seedsClass = new AndroidJavaClass("com.playseeds.android.sdk.Seeds"))
        {
            androidInstance = seedsClass.CallStatic<AndroidJavaObject>("sharedInstance");
        }
        #endif

        if (AutoInitialize)
            Init(ServerURL, ApplicationKey);
    }

    #if UNITY_ANDROID
    private AndroidJavaObject CurrentActivity
    {
        get
        {
            using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                return unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }
    }
    #endif

    private void NotImplemented()
    {
        Debug.LogError("Method not implemented for current platform");
    }

    public Seeds Init(string serverUrl, string appKey)
    {
        #if UNITY_ANDROID
        androidInstance.Call<AndroidJavaObject>("init", CurrentActivity, serverUrl, appKey);
        #else
        NotImplemented();
        #endif

        return this;
    }

    public Seeds Init(string serverUrl, string appKey, string deviceId)
    {
        #if UNITY_ANDROID
        androidInstance.Call<AndroidJavaObject>("init", CurrentActivity, serverUrl, appKey, deviceId);
        #else
        NotImplemented();
        #endif

        return this;
    }

    public Seeds Init(string serverUrl, string appKey, string deviceId, DeviceIdMode? deviceIdMode)
    {
        #if UNITY_ANDROID
        androidInstance.Call<AndroidJavaObject>("init", CurrentActivity, serverUrl, appKey, deviceId, deviceIdMode);
        #else
        NotImplemented();
        #endif

        return this;
    }

    public bool IsInitialized
    {
        get
        {
            #if UNITY_ANDROID
            return androidInstance.Call<bool>("isInitialized");
            #else
            NotImplemented();
            #endif
        }
    }

    //TODO: initMessaging
    //TODO: initInAppMessaging
    //TODO: halt
    //TODO: onStart/onStop
    //TODO: onRegistrationId
    //TODO: recordEvent
    //TODO: recordIAPEvent
    //TODO: recordSeedsIAPEvent
    //TODO: recordEvent
    //TODO: recordEvent
    //TODO: setUserData
    //TODO: setCustomUserData
    //TODO: setLocation
    //TODO: setCustomCrashSegments
    //TODO: addCrashLog
    //TODO: logException
    //TODO: enableCrashReporting
    //TODO: setDisableUpdateSessionRequests
    public Seeds SetLoggingEnabled(bool enabled)
    {
        #if UNITY_ANDROID
        androidInstance.Call<AndroidJavaObject>("setLoggingEnabled", enabled);
        #else
        NotImplemented();
        #endif

        return this;
    }
    //TODO: isLoggingEnabled
    //TODO: enablePublicKeyPinning
    //TODO: isA_bTestingOn
    //TODO: setA_bTestingOn
    //TODO: setMessageVariantName
    //TODO: getMessageVariantName
    //TODO: requestInAppMessage
    //TODO: isInAppMessageLoaded
    //TODO: showInAppMessage
}
