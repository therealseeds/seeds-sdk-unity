package com.playseeds.unity3d.androidbridge;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

public class DeepLinkActivity extends Activity {
    private final static String TAG = DeepLinkActivity.class.getName();
    private static String gameObjectName = "SeedsDeepLink";

    public static String getGameObjectName() {
        return gameObjectName;
    }

    public static void setGameObjectName(String gameObjectName) {
        DeepLinkActivity.gameObjectName = gameObjectName;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        // Launch default activity
        String appPackage = getApplicationContext().getPackageName();
        Log.v(TAG, "Going to launch main activity of '" + appPackage + "'");
        Intent launchIntent = getPackageManager().getLaunchIntentForPackage(appPackage);
        Log.v(TAG, "Found launch intent: " + launchIntent);
        launchIntent.setFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP);
        startActivity(launchIntent);

        // Pass message to Unity3D about
        Log.v(TAG, "Sending message to '" + gameObjectName + "'");
        Intent intent = getIntent();
        Uri data = intent.getData();
        UnityPlayer.UnitySendMessage(gameObjectName, "onLinkArrived", data.toString());
        // Finish this activity
        finish();
    }
}
