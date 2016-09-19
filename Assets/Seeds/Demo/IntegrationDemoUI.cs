using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntegrationDemoUI : MonoBehaviour
{
    void Start()
    {
        var instanceIdGameObject = GameObject.Find("Instance ID");
        instanceIdGameObject.GetComponent<Text>().text = string.Format("Instance ID {0}", Random.Range(0, 1000));

        

        var statsGameObject = GameObject.Find ("Stats");
		Seeds.Instance.OnInAppMessageShowCount += (string error, int count, string messageId) => {
			statsGameObject.GetComponent<Text> ().text = string.Format ("OnInAppMessageShowCount({0}, {1}, {2})", error, count, messageId);
        };
		Seeds.Instance.OnInAppPurchaseCount += (string error, int count, string key) => {
			statsGameObject.GetComponent<Text> ().text = string.Format ("OnInAppPurchaseCount({0}, {1}, {2})", error, count, key);
        };

		var lastUrlReceivedGameObject = GameObject.Find("Last URL received");
		Seeds.Instance.OnInAppMessageClicked += (string messageId) => {
			lastUrlReceivedGameObject.GetComponent<Text>().text = "Clicked: " + messageId;
		};

		Seeds.Instance.OnInAppMessageDismissed += (string messageId) => {
			lastUrlReceivedGameObject.GetComponent<Text>().text = "Dismissed: " + messageId;
		};

		Seeds.Instance.RequestInAppMessage("575f872a64bc1e5b0eca506f");
		Seeds.Instance.RequestInAppMessage("5746851bb29ee753053a7c9a");
    }

    public void RecordIAPEvent1()
    {
        Seeds.Instance.RecordIAPEvent("iap event 1", 9.99);
		// TODO: Separate button for queries
		Seeds.Instance.RequestInAppMessageShowCount ("575f872a64bc1e5b0eca506f");
    }

    public void RecordSeedsIAPEvent1()
    {
        Seeds.Instance.RecordSeedsIAPEvent("seeds iap event 1", 19.99);
		// TODO: Separate button for queries
		Seeds.Instance.RequestTotalInAppPurchaseCount ();
    }

    public void InAppMessage1()
    {
        if (Seeds.Instance.IsInAppMessageLoaded("575f872a64bc1e5b0eca506f"))
            Seeds.Instance.ShowInAppMessage("575f872a64bc1e5b0eca506f");
        else
            Seeds.Instance.RequestInAppMessage("575f872a64bc1e5b0eca506f");
    }

    public void InAppMessage2()
    {
		if (Seeds.Instance.IsInAppMessageLoaded("5746851bb29ee753053a7c9a"))
            Seeds.Instance.ShowInAppMessage("5746851bb29ee753053a7c9a");
        else
            Seeds.Instance.RequestInAppMessage("5746851bb29ee753053a7c9a");
    }
}
