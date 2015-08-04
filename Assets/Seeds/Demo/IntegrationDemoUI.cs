using UnityEngine;
using System.Collections;

public class IntegrationDemoUI : MonoBehaviour
{
    public void RecordIAPEvent1()
    {
        Seeds.Instance.RecordIAPEvent("iap event 1", 9.99);
    }

    public void RecordSeedsIAPEvent1()
    {
        Seeds.Instance.RecordSeedsIAPEvent("seeds iap event 1", 19.99);
    }

    public void InAppMessage1()
    {
        if (Seeds.Instance.IsInAppMessageLoaded)
            Seeds.Instance.ShowInAppMessage();
        else
            Seeds.Instance.RequestInAppMessage();
    }
}
