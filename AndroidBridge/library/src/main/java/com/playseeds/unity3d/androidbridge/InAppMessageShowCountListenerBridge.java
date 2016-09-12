package com.playseeds.unity3d.androidbridge;

import com.playseeds.android.sdk.IInAppMessageShowCountListener;
import com.unity3d.player.UnityPlayer;
import org.json.JSONObject;

public class InAppMessageShowCountListenerBridge implements IInAppMessageShowCountListener {
    private String unityObjectName;

    private InAppMessageShowCountListenerBridge(String unityObjectName) {
        this.unityObjectName = unityObjectName;
    }

    public void onInAppMessageShowCount(String errorMessage, int showCount, String message_id) {
        JSONObject json = new JSONObject();

        try {
            json.put("errorMessage", errorMessage);
            json.put("showCount", showCount);
            json.put("messageId", message_id);
        } catch (Exception e) { }

        UnityPlayer.UnitySendMessage(unityObjectName, "onInAppMessageShowCount", json.toString());
    }

    public static InAppMessageShowCountListenerBridge create(String unityObjectName) {
        return new InAppMessageShowCountListenerBridge(unityObjectName);
    }
}
