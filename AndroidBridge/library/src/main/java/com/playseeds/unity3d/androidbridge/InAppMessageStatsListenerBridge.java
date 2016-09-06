package com.playseeds.unity3d.androidbridge;

import com.playseeds.android.sdk.IInAppMessageStatsListener;
import com.unity3d.player.UnityPlayer;

public class InAppMessageStatsListenerBridge implements IInAppMessageStatsListener {
    private String unityObjectName;

    private InAppMessageStatsListenerBridge(String unityObjectName) {
        this.unityObjectName = unityObjectName;
    }

    public void onInAppMessageStats(String key, int shownCount) {
        UnityPlayer.UnitySendMessage(unityObjectName, "onInAppMessageStats",
                "{\"key\":\""+ key +"\",\"shownCount\":"+ shownCount +"}");
    }

    public static InAppMessageStatsListenerBridge create(String unityObjectName) {
        return new InAppMessageStatsListenerBridge(unityObjectName);
    }
}
