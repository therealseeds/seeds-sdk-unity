using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
    /// True if trace should be enabled, false otherwise.
    /// </summary>
    public bool TraceEnabled = false;

    /// <summary>
    /// True if Seeds SDK should auto-initialize during instantiation, false otherwise.
    /// </summary>
    public bool AutoInitialize = false;

    /// <summary>
    /// Server URL. Do not include trailing slash.
    /// </summary>
    public string ServerURL = "http://dash.playseeds.com";

    /// <summary>
    /// Application API key.
    /// </summary>
    public string ApplicationKey;

    public event Action OnInAppMessageClicked;
    public event Action OnInAppMessageClosedComplete;
    public event Action OnInAppMessageClosedIncomplete;
    public event Action OnInAppMessageLoadSucceeded;
    public event Action OnInAppMessageShownSuccessfully;
    public event Action OnInAppMessageShownInsuccessfully;
    public event Action OnNoInAppMessageFound;

    #if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject androidInstance;
    private AndroidJavaObject androidBridgeInstance;
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
        #if UNITY_ANDROID && !UNITY_EDITOR
        using (var seedsClass = new AndroidJavaClass("com.playseeds.android.sdk.Seeds"))
        {
            androidInstance = seedsClass.CallStatic<AndroidJavaObject>("sharedInstance");
        }
        using (var inAppMessageListenerBridgeClass = new AndroidJavaClass("com.playseeds.unity3d.androidbridge.InAppMessageListenerBridge"))
        {
            androidBridgeInstance = inAppMessageListenerBridgeClass.CallStatic<AndroidJavaObject>("create", gameObject.name);
        }
        #endif

        if (AutoInitialize)
            Init(ServerURL, ApplicationKey);

        #if UNITY_ANDROID && !UNITY_EDITOR
        NotifyOnStart();
        #endif
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (TraceEnabled)
            Debug.LogFormat("[Seeds] OnApplicationPause({0})", pauseStatus);

        #if UNITY_ANDROID && !UNITY_EDITOR
        if (pauseStatus)
            NotifyOnStop();
        else
            NotifyOnStart();
        #endif
    }

    #if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject CurrentActivity
    {
        get
        {
            using (var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                return unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }
    }

    private void RunOnUIThread(AndroidJavaRunnable runnable)
    {
        CurrentActivity.Call("runOnUiThread", new AndroidJavaRunnable(runnable));
    }

    private AndroidJavaObject CreateMapFromDicrionary(IDictionary<string, string> dictionary)
    {
        AndroidJavaObject map;
        using (var helpersClass = new AndroidJavaClass("com.playseeds.unity3d.androidbridge.Helpers"))
        {
            map = helpersClass.CallStatic<AndroidJavaObject>("createHashMapOfStringString");
        }

        foreach (var entry in dictionary)
        {
            map.Call<AndroidJavaObject>("put", entry.Key, entry.Value);
        }

        return map;
    }

    private AndroidJavaObject CreateListFromEnumerable(IEnumerable<string> enumerable)
    {
        AndroidJavaObject list;
        using (var helpersClass = new AndroidJavaClass("com.playseeds.unity3d.androidbridge.Helpers"))
        {
            list = helpersClass.CallStatic<AndroidJavaObject>("createArrayListOfString");
        }

        foreach (var item in enumerable)
        {
            list.Call<bool>("add", item);
        }

        return list;
    }
    #endif

    private static void NotImplemented(string method)
    {
        Debug.LogErrorFormat("Method {0} not implemented for current platform", method);
    }

    void inAppMessageClicked(string notUsed)
    {
        if (TraceEnabled)
            Debug.Log("[Seeds] OnInAppMessageClicked");

        if (OnInAppMessageClicked != null)
            OnInAppMessageClicked();
    }

    void inAppMessageClosedComplete(string inAppMessageId)
    {
        if (TraceEnabled)
            Debug.Log("[Seeds] OnInAppMessageClosedComplete");
        
        if (OnInAppMessageClosedComplete != null)
            OnInAppMessageClosedComplete();
    }

    void inAppMessageClosedIncomplete(string inAppMessageId)
    {
        if (TraceEnabled)
            Debug.Log("[Seeds] OnInAppMessageClosedIncomplete");
        
        if (OnInAppMessageClosedIncomplete != null)
            OnInAppMessageClosedIncomplete();
    }

    void inAppMessageLoadSucceeded(string inAppMessageId)
    {
        if (TraceEnabled)
            Debug.Log("[Seeds] OnInAppMessageLoadSucceeded");
        
        if (OnInAppMessageLoadSucceeded != null)
            OnInAppMessageLoadSucceeded();
    }

    void inAppMessageShownSuccessfully(string inAppMessageId)
    {
        if (TraceEnabled)
            Debug.Log("[Seeds] OnInAppMessageShownSuccessfully");
        
        if (OnInAppMessageShownSuccessfully != null)
            OnInAppMessageShownSuccessfully();
    }

    void inAppMessageShownInsuccessfully(string inAppMessageId)
    {
        if (TraceEnabled)
            Debug.Log("[Seeds] OnInAppMessageShownInsuccessfully");
        
        if (OnInAppMessageShownInsuccessfully != null)
            OnInAppMessageShownInsuccessfully();
    }

    void noInAppMessageFound(string notUsed)
    {
        if (TraceEnabled)
            Debug.Log("[Seeds] OnNoInAppMessageFound");
        
        if (OnNoInAppMessageFound != null)
            OnNoInAppMessageFound();
    }

    public Seeds Init(string serverUrl, string appKey)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        RunOnUIThread(() => androidInstance.Call<AndroidJavaObject>("init", CurrentActivity, androidBridgeInstance, serverUrl,
            appKey));
        #else
        NotImplemented("Init(string serverUrl, string appKey)");
        #endif

        return this;
    }

    public Seeds Init(string serverUrl, string appKey, string deviceId)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        RunOnUIThread(() => androidInstance.Call<AndroidJavaObject>("init", CurrentActivity, androidBridgeInstance, serverUrl,
            appKey, deviceId));
        #else
        NotImplemented("Init(string serverUrl, string appKey, string deviceId)");
        #endif

        return this;
    }

    public Seeds Init(string serverUrl, string appKey, string deviceId, DeviceIdMode? deviceIdMode)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        RunOnUIThread(() => androidInstance.Call<AndroidJavaObject>("init", CurrentActivity, androidBridgeInstance, serverUrl,
            appKey, deviceId, deviceIdMode));
        #else
        NotImplemented("Init(string serverUrl, string appKey, string deviceId, DeviceIdMode? deviceIdMode)");
        #endif

        return this;
    }

    public bool IsInitialized
    {
        get
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            return androidInstance.Call<bool>("isInitialized");
            #else
            NotImplemented("IsInitialized::get");
            return false;
            #endif
        }
    }

    public void Halt()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call("halt");
        #else
        NotImplemented("Halt()");
        #endif
    }

    #if UNITY_ANDROID && !UNITY_EDITOR
    public void NotifyOnStart()
    {
        RunOnUIThread(() => androidInstance.Call("onStart"));
    }

    public void NotifyOnStop()
    {
        RunOnUIThread(() => androidInstance.Call("onStop"));
    }

    public void NotifyOnGCMRegistrationId(string registrationId)
    {
        androidInstance.Call("onRegistrationId", registrationId);
    }
    #endif

    public void RecordEvent(string key)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call("recordEvent", key);
        #else
        NotImplemented("RecordEvent(string key)");
        #endif
    }

    public void RecordEvent(string key, int count)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call("recordEvent", key, count);
        #else
        NotImplemented("RecordEvent(string key, int count)");
        #endif
    }

    public void RecordEvent(string key, int count, double sum)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call("recordEvent", key, count, sum);
        #else
        NotImplemented("RecordEvent(string key, int count, double sum)");
        #endif
    }

    public void RecordEvent(string key, IDictionary<string, string> segmentation, int count)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call("recordEvent", key, CreateMapFromDicrionary(segmentation), count);
        #else
        NotImplemented("RecordEvent(string key, IDictionary<string, string> segmentation, int count)");
        #endif
    }

    public void RecordEvent(string key, IDictionary<string, string> segmentation, int count, double sum)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call("recordEvent", key, CreateMapFromDicrionary(segmentation), count, sum);
        #else
        NotImplemented("RecordEvent(string key, IDictionary<string, string> segmentation, int count, double sum)");
        #endif
    }

    public void RecordIAPEvent(string key, double price)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call("recordIAPEvent", key, price);
        #else
        NotImplemented("RecordIAPEvent(string key, double price)");
        #endif
    }

    public void RecordSeedsIAPEvent(string key, double price)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call("recordSeedsIAPEvent", key, price);
        #else
        NotImplemented("RecordSeedsIAPEvent(string key, double price)");
        #endif
    }

    public Seeds SetUserData(IDictionary<string, string> data)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call<AndroidJavaObject>("setUserData", CreateMapFromDicrionary(data));
        #else
        NotImplemented("SetUserData(IDictionary<string, string> data)");
        #endif

        return this;
    }

    public Seeds SetUserData(IDictionary<string, string> data, IDictionary<string, string> customData)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call<AndroidJavaObject>("setUserData", CreateMapFromDicrionary(data),
            CreateMapFromDicrionary(customData));
        #else
        NotImplemented("SetUserData(IDictionary<string, string> data, IDictionary<string, string> customData)");
        #endif

        return this;
    }

    public Seeds SetCustomUserData(IDictionary<string, string> customData)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call<AndroidJavaObject>("setCustomUserData", CreateMapFromDicrionary(customData));
        #else
        NotImplemented("SetCustomUserData(IDictionary<string, string> customData)");
        #endif

        return this;
    }

    public Seeds SetLocation(double lat, double lon)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call<AndroidJavaObject>("setLocation", lat, lon);
        #else
        NotImplemented("SetLocation(double lat, double lon)");
        #endif

        return this;
    }

    public Seeds SetCustomCrashSegments(IDictionary<string, string> segments)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call<AndroidJavaObject>("setCustomCrashSegments", CreateMapFromDicrionary(segments));
        #else
        NotImplemented("SetCustomCrashSegments(IDictionary<string, string> segments)");
        #endif

        return this;
    }

    public Seeds AddCrashLog(string record)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call<AndroidJavaObject>("addCrashLog", record);
        #else
        NotImplemented("AddCrashLog(string record)");
        #endif

        return this;
    }

    public Seeds LogException(string exception)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call<AndroidJavaObject>("logException", exception);
        #else
        NotImplemented("LogException(string exception)");
        #endif

        return this;
    }

    public Seeds EnableCrashReporting()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call<AndroidJavaObject>("enableCrashReporting");
        #else
        NotImplemented("EnableCrashReporting()");
        #endif

        return this;
    }

    public Seeds SetDisableUpdateSessionRequests(bool disable)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call<AndroidJavaObject>("setDisableUpdateSessionRequests", disable);
        #else
        NotImplemented("SetDisableUpdateSessionRequests(bool disable)");
        #endif

        return this;
    }

    public Seeds SetLoggingEnabled(bool enabled)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call<AndroidJavaObject>("setLoggingEnabled", enabled);
        #else
        NotImplemented("SetLoggingEnabled(bool enabled)");
        #endif

        return this;
    }

    public bool IsLoggingEnabled
    {
        get
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            return androidInstance.Call<bool>("isLoggingEnabled");
            #else
            NotImplemented("IsLoggingEnabled::get");
            return false;
            #endif
        }
        set
        {
            SetLoggingEnabled(value);
        }
    }

    public Seeds EnablePublicKeyPinning(IEnumerable<string> certificates)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call<AndroidJavaObject>("enablePublicKeyPinning", CreateListFromEnumerable(certificates));
        #else
        NotImplemented("EnablePublicKeyPinning(IEnumerable<string> certificates)");
        #endif

        return this;
    }

    public bool IsABTestingOn
    {
        get
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            return androidInstance.Call<bool>("isA_bTestingOn");
            #else
            NotImplemented("IsABTestingOn::get");
            return false;
            #endif
        }
        set
        {
            SetABTestingOn(value);
        }
    }

    public void SetABTestingOn(bool abTestingOn)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call("setA_bTestingOn", abTestingOn);
        #else
        NotImplemented("SetABTestingOn(bool abTestingOn)");
        #endif
    }

    public void SetMessageVariantName(string messageVariantName)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call("setMessageVariantName", messageVariantName);
        #else
        NotImplemented("SetMessageVariantName(string messageVariantName)");
        #endif
    }

    public string GetMessageVariantName()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        return androidInstance.Call<string>("getMessageVariantName");
        #else
        NotImplemented("GetMessageVariantName()");
        return null;
        #endif
    }

    public string MessageVariantName
    {
        get
        {
            return GetMessageVariantName();
        }
        set
        {
            SetMessageVariantName(value);
        }
    }

    public void RequestInAppMessage()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call("requestInAppMessage");
        #else
        NotImplemented("RequestInAppMessage()");
        #endif
    }

    public bool IsInAppMessageLoaded
    {
        get
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            return androidInstance.Call<bool>("isInAppMessageLoaded");
            #else
            NotImplemented("IsInAppMessageLoaded::get");
            return false;
            #endif
        }
    }

    public void ShowInAppMessage()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        androidInstance.Call("showInAppMessage");
        #else
        NotImplemented("ShowInAppMessage()");
        #endif
    }
}
