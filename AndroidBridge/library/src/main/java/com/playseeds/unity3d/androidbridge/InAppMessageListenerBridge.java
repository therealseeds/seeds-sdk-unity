package com.playseeds.unity3d.androidbridge;

import com.playseeds.android.sdk.inappmessaging.InAppMessage;
import com.playseeds.android.sdk.inappmessaging.InAppMessageListener;
import com.unity3d.player.UnityPlayer;

public class InAppMessageListenerBridge implements InAppMessageListener {
    private String unityObjectName;

    private InAppMessageListenerBridge(String unityObjectName) {
        this.unityObjectName = unityObjectName;
    }

    public void inAppMessageClicked(String messageId, InAppMessage inAppMessage) {
        UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageClicked", messageId);
    }

    public void inAppMessageClosed(String messageId, InAppMessage inAppMessage, boolean completed) {
        if (completed) {
            UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageClosedComplete",
                    messageId);
        } else {
            UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageClosedIncomplete",
                    messageId);
        }
    }

    public void inAppMessageLoadSucceeded(String messageId, InAppMessage inAppMessage) {
        UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageLoadSucceeded", messageId);
    }

    public void inAppMessageShown(String messageId, InAppMessage inAppMessage, boolean succeeded) {
        if (succeeded) {
            UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageShownSuccessfully",
                    messageId);
        } else {
            UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageShownInsuccessfully",
                    messageId);
        }
    }

    public void noInAppMessageFound(String messageId) {
        UnityPlayer.UnitySendMessage(unityObjectName, "noInAppMessageFound", messageId);
    }

    public static InAppMessageListenerBridge create(String unityObjectName) {
        return new InAppMessageListenerBridge(unityObjectName);
    }
}