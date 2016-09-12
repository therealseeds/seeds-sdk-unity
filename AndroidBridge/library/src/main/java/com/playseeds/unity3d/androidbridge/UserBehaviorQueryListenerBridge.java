package com.playseeds.unity3d.androidbridge;

import com.google.gson.JsonElement;
import com.playseeds.android.sdk.IUserBehaviorQueryListener;
import com.unity3d.player.UnityPlayer;

import org.json.JSONObject;

public class UserBehaviorQueryListenerBridge implements IUserBehaviorQueryListener {
    private String unityObjectName;

    private UserBehaviorQueryListenerBridge(String unityObjectName) {
        this.unityObjectName = unityObjectName;
    }

    public void onUserBehaviorResponse(String errorMessage, JsonElement result, String queryPath) {
        JSONObject json = new JSONObject();

        try {
            json.put("errorMessage", errorMessage);
            json.put("result", result.getAsString());
            json.put("queryPath", queryPath);
        } catch (Exception e) { }

        UnityPlayer.UnitySendMessage(unityObjectName, "onUserBehaviorResponse", json.toString());
    }

    public static UserBehaviorQueryListenerBridge create(String unityObjectName) {
        return new UserBehaviorQueryListenerBridge(unityObjectName);
    }
}
