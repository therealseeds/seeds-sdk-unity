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

    		Seeds.Instance.RequestInAppMessage("57e36337ad5957420e120842", "$2,99");
    		Seeds.Instance.RequestInAppMessage("57e36365ad5957420e120845");
    }

    public void RecordIAPEvent1()
    {
        Seeds.Instance.RecordIAPEvent("iap event 1", 9.99);
		// TODO: Separate button for queries
		Seeds.Instance.RequestInAppMessageShowCount ("57e36337ad5957420e120842");
    }

    public void RecordSeedsIAPEvent1()
    {
        Seeds.Instance.RecordSeedsIAPEvent("seeds iap event 1", 19.99);
		// TODO: Separate button for queries
		Seeds.Instance.RequestTotalInAppPurchaseCount ();
    }

    public void InAppMessage1()
    {
		if (Seeds.Instance.IsInAppMessageLoaded("57e36337ad5957420e120842"))
			Seeds.Instance.ShowInAppMessage("57e36337ad5957420e120842");
        else
			Seeds.Instance.RequestInAppMessage("57e36337ad5957420e120842");
    }

    public void InAppMessage2()
    {
		if (Seeds.Instance.IsInAppMessageLoaded("57e36365ad5957420e120845"))
			Seeds.Instance.ShowInAppMessage("57e36365ad5957420e120845");
        else
			Seeds.Instance.RequestInAppMessage("57e36365ad5957420e120845");
    }
}
