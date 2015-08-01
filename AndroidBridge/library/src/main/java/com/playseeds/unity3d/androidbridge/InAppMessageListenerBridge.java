package com.playseeds.unity3d.androidbridge;

import com.playseeds.android.sdk.inappmessaging.InAppMessage;
import com.playseeds.android.sdk.inappmessaging.InAppMessageListener;
import com.unity3d.player.UnityPlayer;

public class InAppMessageListenerBridge implements InAppMessageListener {
    private String unityObjectName;

    private InAppMessageListenerBridge(String unityObjectName) {
        this.unityObjectName = unityObjectName;
    }

    public void inAppMessageClicked() {
        UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageClicked", "");
    }

    public void inAppMessageClosed(InAppMessage inAppMessage, boolean completed) {
        String inAppMessageId = inAppMessage == null ? null : inAppMessage.toString();
        if (completed) {
            UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageClosedComplete",
                    inAppMessageId);
        } else {
            UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageClosedIncomplete",
                    inAppMessageId);
        }
    }

    public void inAppMessageLoadSucceeded(InAppMessage inAppMessage) {
        String inAppMessageId = inAppMessage == null ? null : inAppMessage.toString();
        UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageLoadSucceeded",
                inAppMessage.toString());
    }

    public void inAppMessageShown(InAppMessage inAppMessage, boolean succeeded) {
        String inAppMessageId = inAppMessage == null ? null : inAppMessage.toString();
        if (succeeded) {
            UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageShownSuccessfully",
                    inAppMessageId);
        } else {
            UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageShownInsuccessfully",
                    inAppMessageId);
        }
    }

    public void noInAppMessageFound() {
        UnityPlayer.UnitySendMessage(unityObjectName, "noInAppMessageFound", "");
    }

    public static InAppMessageListenerBridge create(String unityObjectName) {
        return new InAppMessageListenerBridge(unityObjectName);
    }
}