using UnityEngine;
using System.Collections;

public class IntegrationDemoUI : MonoBehaviour
{
    public void RecordEvent1()
    {
        Seeds.Instance.RecordEvent("unity3d test event 1");
    }

    public void RecordEvent2()
    {
        Seeds.Instance.RecordEvent("unity3d test event 2");
    }

    public void InAppMessage1()
    {
        if (Seeds.Instance.IsInAppMessageLoaded)
            Seeds.Instance.ShowInAppMessage();
        else
            Seeds.Instance.RequestInAppMessage();
    }
}
