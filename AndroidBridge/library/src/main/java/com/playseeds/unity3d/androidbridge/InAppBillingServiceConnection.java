package com.playseeds.unity3d.androidbridge;

import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.IBinder;

import com.android.vending.billing.IInAppBillingService;
import com.unity3d.player.UnityPlayer;

public class InAppBillingServiceConnection implements ServiceConnection {
    private IInAppBillingService inAppBillingService;

    private String unityObjectName;

    private InAppBillingServiceConnection(String unityObjectName) {
        this.unityObjectName = unityObjectName;
    }

    public void connect() {
        Intent serviceIntent = new Intent("com.android.vending.billing.InAppBillingService.BIND");
        serviceIntent.setPackage("com.android.vending");
        UnityPlayer.currentActivity.bindService(serviceIntent, this, Context.BIND_AUTO_CREATE);
    }

    public void disconnect() {
        if (inAppBillingService != null) {
            UnityPlayer.currentActivity.unbindService(this);
        }
    }

    public IInAppBillingService getInAppBillingService() {
        return inAppBillingService;
    }

    @Override
    public void onServiceConnected(ComponentName name, IBinder service) {
        inAppBillingService = IInAppBillingService.Stub.asInterface(service);
        UnityPlayer.UnitySendMessage(unityObjectName, "onAndroidIapServiceConnected", "");
    }

    @Override
    public void onServiceDisconnected(ComponentName name) {
        inAppBillingService = null;
        UnityPlayer.UnitySendMessage(unityObjectName, "onAndroidIapServiceDisconnected", "");
    }

    public static InAppBillingServiceConnection create(String unityObjectName) {
        return new InAppBillingServiceConnection(unityObjectName);
    }
}
