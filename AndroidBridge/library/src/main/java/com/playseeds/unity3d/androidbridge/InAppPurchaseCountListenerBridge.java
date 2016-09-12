package com.playseeds.unity3d.androidbridge;

import com.playseeds.android.sdk.IInAppPurchaseCountListener;
import com.unity3d.player.UnityPlayer;

import org.json.JSONObject;

public class InAppPurchaseCountListenerBridge implements IInAppPurchaseCountListener {
    private String unityObjectName;

    private InAppPurchaseCountListenerBridge(String unityObjectName) {
        this.unityObjectName = unityObjectName;
    }

    public void onInAppPurchaseCount(String errorMessage, int purchasesCount, String key) {
        JSONObject json = new JSONObject();

        try {
            json.put("errorMessage", errorMessage);
            json.put("purchasesCount", purchasesCount);
            json.put("key", key);
        } catch (Exception e) { }

        UnityPlayer.UnitySendMessage(unityObjectName, "onInAppPurchaseCount", json.toString());
    }

    public static InAppPurchaseCountListenerBridge create(String unityObjectName) {
        return new InAppPurchaseCountListenerBridge(unityObjectName);
    }
}
