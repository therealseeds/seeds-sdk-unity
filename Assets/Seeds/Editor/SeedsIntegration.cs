﻿using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using Ionic.Zip;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class SeedsIntegration
{
    [MenuItem("Seeds SDK/Configure")]
    public static void ShowConfigureDialog()
    {
        EditorWindow.GetWindow(typeof(SeedsIntegrationDialogWindow));
    }

    public static void ConfigureDeepLinks(string scheme, string host, string pathPrefix)
    {
        // Add slash to prefix if needed
        if (pathPrefix == null)
            pathPrefix = "/";
        else if (pathPrefix.Length > 0 && pathPrefix[0] != '/')
            pathPrefix = "/" + pathPrefix;

        var deepLinkAndroidManifestContent = string.Format(@"
            <manifest xmlns:android=""http://schemas.android.com/apk/res/android""
                package=""com.playseeds.unity3d.androidbridge.deeplinks"">
                <application>
                    <activity
                        android:name=""com.playseeds.unity3d.androidbridge.DeepLinkActivity""
                        android:launchMode=""singleInstance""
                        android:noHistory=""true"">
                        <intent-filter>
                            <action android:name=""android.intent.action.VIEW"" />
                            <category android:name=""android.intent.category.DEFAULT"" />
                            <category android:name=""android.intent.category.BROWSABLE"" />
                            <data
                                android:scheme=""{0}""
                                android:host=""{1}""
                                android:pathPrefix=""{2}"" />
                        </intent-filter>
                    </activity>
                </application>
            </manifest>
            ".Trim(), scheme, host, pathPrefix);

        #if UNITY_5
        // For Unity3D 5.x+ Android AAR format is supported. So create SeedsDeepLinks.aar with following content:
        // 1. Empty classes.jar (due to a bug in Unity3D)
        // 2. Empty R.txt
        // 3. Empty res folder
        // 4. Generated AndroidManifest.xml

        using (var seedsDeepLinksAar = new ZipFile())
        {
            seedsDeepLinksAar.AddEntry("classes.jar", new byte[] {
                0x50, 0x4b, 0x03, 0x04, 0x14, 0x00, 0x08, 0x08, 0x08, 0x00, 0x7d, 0x42,
                0xff, 0x46, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x09, 0x00, 0x04, 0x00, 0x4d, 0x45, 0x54, 0x41, 0x2d, 0x49,
                0x4e, 0x46, 0x2f, 0xfe, 0xca, 0x00, 0x00, 0x03, 0x00, 0x50, 0x4b, 0x07,
                0x08, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x50, 0x4b, 0x03, 0x04, 0x14, 0x00, 0x08, 0x08, 0x08, 0x00, 0x7d,
                0x42, 0xff, 0x46, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x4d, 0x45, 0x54, 0x41, 0x2d,
                0x49, 0x4e, 0x46, 0x2f, 0x4d, 0x41, 0x4e, 0x49, 0x46, 0x45, 0x53, 0x54,
                0x2e, 0x4d, 0x46, 0xf3, 0x4d, 0xcc, 0xcb, 0x4c, 0x4b, 0x2d, 0x2e, 0xd1,
                0x0d, 0x4b, 0x2d, 0x2a, 0xce, 0xcc, 0xcf, 0xb3, 0x52, 0x30, 0xd4, 0x33,
                0xe0, 0xe5, 0x72, 0x2e, 0x4a, 0x4d, 0x2c, 0x49, 0x4d, 0xd1, 0x75, 0xaa,
                0x04, 0x09, 0x58, 0xe8, 0x19, 0xc4, 0x9b, 0x1a, 0x2a, 0x68, 0xf8, 0x17,
                0x25, 0x26, 0xe7, 0xa4, 0x2a, 0x38, 0xe7, 0x17, 0x15, 0xe4, 0x17, 0x25,
                0x96, 0x00, 0x95, 0x6b, 0xf2, 0x72, 0xf1, 0x72, 0x01, 0x00, 0x50, 0x4b,
                0x07, 0x08, 0x36, 0x4b, 0xc8, 0x5e, 0x43, 0x00, 0x00, 0x00, 0x44, 0x00,
                0x00, 0x00, 0x50, 0x4b, 0x01, 0x02, 0x14, 0x00, 0x14, 0x00, 0x08, 0x08,
                0x08, 0x00, 0x7d, 0x42, 0xff, 0x46, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x04, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x4d, 0x45, 0x54, 0x41, 0x2d, 0x49, 0x4e, 0x46, 0x2f, 0xfe, 0xca, 0x00,
                0x00, 0x50, 0x4b, 0x01, 0x02, 0x14, 0x00, 0x14, 0x00, 0x08, 0x08, 0x08,
                0x00, 0x7d, 0x42, 0xff, 0x46, 0x36, 0x4b, 0xc8, 0x5e, 0x43, 0x00, 0x00,
                0x00, 0x44, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3d, 0x00, 0x00, 0x00, 0x4d,
                0x45, 0x54, 0x41, 0x2d, 0x49, 0x4e, 0x46, 0x2f, 0x4d, 0x41, 0x4e, 0x49,
                0x46, 0x45, 0x53, 0x54, 0x2e, 0x4d, 0x46, 0x50, 0x4b, 0x05, 0x06, 0x00,
                0x00, 0x00, 0x00, 0x02, 0x00, 0x02, 0x00, 0x7d, 0x00, 0x00, 0x00, 0xc2,
                0x00, 0x00, 0x00, 0x00, 0x00
            });
            seedsDeepLinksAar.AddEntry("R.txt", new byte[0]);
            seedsDeepLinksAar.AddDirectoryByName("res");
            seedsDeepLinksAar.AddEntry("AndroidManifest.xml", deepLinkAndroidManifestContent, Encoding.ASCII);

            seedsDeepLinksAar.Save(Path.Combine(Application.dataPath, "Plugins/Android/SeedsDeepLinks.aar"));
        }
        #elif UNITY_4
        // For Unity3D 4.x only AndroidManifest.xml modification is needed

        var androidManifestPath = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
        var androidNS = "http://schemas.android.com/apk/res/android";

        var androidManifestDocument = new XmlDocument();
        androidManifestDocument.Load(androidManifestPath);
        var manifestNsManager = new XmlNamespaceManager(androidManifestDocument.NameTable);
        manifestNsManager.AddNamespace("android", androidNS);

        var deepLinkActivityXPath =
            "/manifest/application/activity[@android:name='com.playseeds.unity3d.androidbridge.DeepLinkActivity']";
        var deepLinkActivityNode = androidManifestDocument.SelectSingleNode(deepLinkActivityXPath, manifestNsManager);
        if (deepLinkActivityNode == null)
        {
            var applicactionNode = androidManifestDocument.SelectSingleNode("/manifest/application");

            var deepLinkAndroidManifest = new XmlDocument();
            deepLinkAndroidManifest.LoadXml(deepLinkAndroidManifestContent);
            manifestNsManager = new XmlNamespaceManager(androidManifestDocument.NameTable);
            manifestNsManager.AddNamespace("android", androidNS);
            deepLinkActivityNode = deepLinkAndroidManifest.SelectSingleNode(deepLinkActivityXPath, manifestNsManager);

            deepLinkActivityNode = androidManifestDocument.ImportNode(deepLinkActivityNode, true);
            applicactionNode.AppendChild(deepLinkActivityNode);
        }

        var dataNode = deepLinkActivityNode.SelectSingleNode("intent-filter/data");

        var schemeAttribute = dataNode.Attributes.GetNamedItem("scheme", androidNS);
        var hostAttribute = dataNode.Attributes.GetNamedItem("host", androidNS);
        var pathPrefixAttribute = dataNode.Attributes.GetNamedItem("pathPrefix", androidNS);

        schemeAttribute.Value = scheme;
        hostAttribute.Value = host;
        pathPrefixAttribute.Value = pathPrefix;

        androidManifestDocument.Save(androidManifestPath);

        #else
        #error Unsupported Unity3D version, please contact support.
        #endif
    }

    [MenuItem("Seeds SDK/Configure deep links...", true)]
    public static bool ConfigureDeepLinksEnabled()
    {
        #if UNITY_ANDROID
        return true;
        #else
        return false;
        #endif
    }
}