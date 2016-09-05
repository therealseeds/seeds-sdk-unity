package com.playseeds.unity3d.androidbridge;

import com.playseeds.android.sdk.IInAppPurchaseStatsListener;
import com.unity3d.player.UnityPlayer;

public class InAppPurchaseStatsListenerBridge implements IInAppPurchaseStatsListener {
    private String unityObjectName;

    private InAppPurchaseStatsListenerBridge(String unityObjectName) {
        this.unityObjectName = unityObjectName;
    }

    public void onInAppPurchaseStats(String key, int purchasesCount) {
        UnityPlayer.UnitySendMessage(unityObjectName, "onInAppPurchaseStats", key);
    }
}
