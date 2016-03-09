using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntegrationDemoUI : MonoBehaviour
{
    void Start()
    {
        var instanceIdGameObject = GameObject.Find("Instance ID");
        instanceIdGameObject.GetComponent<Text>().text = string.Format("Instance ID {0}", Random.Range(0, 1000));

        var lastUrlReceivedGameObject = GameObject.Find("Last URL received");
        SeedsDeepLinks.Instance.OnLinkArrived += (string url) => {
            Debug.Log("Demo received URL + " + url);
            lastUrlReceivedGameObject.GetComponent<Text>().text = url;
        };
    }

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
