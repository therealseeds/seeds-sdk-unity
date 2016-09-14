package com.playseeds.unity3d.androidbridge;

import com.playseeds.android.sdk.inappmessaging.InAppMessage;
import com.playseeds.android.sdk.inappmessaging.InAppMessageListener;
import com.unity3d.player.UnityPlayer;

import java.util.Locale;

public class InAppMessageListenerBridge implements InAppMessageListener {
    private String unityObjectName;

    private InAppMessageListenerBridge(String unityObjectName) {
        this.unityObjectName = unityObjectName;
    }

    public void inAppMessageClicked(String messageId) {
        UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageClicked", messageId);
    }

    public void inAppMessageDismissed(String messageId) {
        UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageDismissed", messageId);
    }

    public void inAppMessageLoadSucceeded(String messageId) {
        UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageLoadSucceeded", messageId);
    }

    public void inAppMessageShown(String messageId, boolean succeeded) {
        if (succeeded) {
            UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageShownSuccessfully",
                    messageId);
        } else {
            UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageShownUnsuccessfully",
                    messageId);
        }
    }

    public void noInAppMessageFound(String messageId) {
        UnityPlayer.UnitySendMessage(unityObjectName, "noInAppMessageFound", messageId);
    }

    public void inAppMessageClickedWithDynamicPrice(String messageId, Double price) {
        String msg = messageId + " " + String.format("%1.2f", price);
        UnityPlayer.UnitySendMessage(unityObjectName, "inAppMessageClickedWithDynamicPrice", msg);
    }

    public static InAppMessageListenerBridge create(String unityObjectName) {
        return new InAppMessageListenerBridge(unityObjectName);
    }
}
