using System;
using System.Collections;
using System.IO;
using System.Linq;
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
        var dialogWindow = EditorWindow.GetWindow<SeedsIntegrationDialogWindow>(true, "Seeds SDK", true);

        var scheme = "your unique game name";
        var host = "seeds";
        var pathPrefix = "item name";
        var displayPathPrefix = pathPrefix;

        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            var androidManifestDocument = new XmlDocument();

            #if UNITY_5
            var seedsDeepLinksAarFilename = Path.Combine(Application.dataPath, "Plugins/Android/SeedsDeepLinks.aar");
            if (File.Exists(seedsDeepLinksAarFilename))
            {
                using (var seedsDeepLinksAar = ZipFile.Read(seedsDeepLinksAarFilename))
                {
                    var androidManifestEntry = seedsDeepLinksAar.Entries
                    .Where(x => x.FileName == "AndroidManifest.xml")
                    .FirstOrDefault();
                    if (androidManifestEntry != null)
                    {
                        using (var stream = new MemoryStream())
                        {
                            androidManifestEntry.Extract(stream);
                            stream.Position = 0;
                            androidManifestDocument.Load(stream);
                        }
                    }
                }
            }
            #elif UNITY_4_6 || UNITY_4_5
            var androidManifestFilename = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
            if (File.Exists(androidManifestFilename))
                androidManifestDocument.Load(androidManifestFilename);
            #else
            #error Unsupported Unity3D version, please contact support.
            #endif

            var manifestNsManager = new XmlNamespaceManager(androidManifestDocument.NameTable);
            var androidNS = "http://schemas.android.com/apk/res/android";
            manifestNsManager.AddNamespace("android", androidNS);

            var deepLinkActivityXPath =
                "/manifest/application/activity[@android:name='com.playseeds.unity3d.androidbridge.DeepLinkActivity']";
            var deepLinkActivityNode = androidManifestDocument.SelectSingleNode(deepLinkActivityXPath, manifestNsManager);
            if (deepLinkActivityNode != null)
            {
                var dataNode = deepLinkActivityNode.SelectSingleNode("intent-filter/data");

                var schemeAttribute = dataNode.Attributes.GetNamedItem("scheme", androidNS);
                var hostAttribute = dataNode.Attributes.GetNamedItem("host", androidNS);
                var pathPrefixAttribute = dataNode.Attributes.GetNamedItem("pathPrefix", androidNS);

                scheme = schemeAttribute.Value;
                host = hostAttribute.Value;
                pathPrefix = pathPrefixAttribute.Value;
            }
        }
        #if UNITY_4_6 || UNITY_4_5
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone)
        #else
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        #endif
        {
            var seedsSettingsXml = new XmlDocument();
            var seedsSettingsFilename = Path.Combine(Application.dataPath, "../ProjectSettings/SeedsSDK.xml");
            if (File.Exists(seedsSettingsFilename))
                seedsSettingsXml.Load(seedsSettingsFilename);

            var deepLinkingNode = seedsSettingsXml.SelectSingleNode("SeedsSDK/iOS/DeepLinking");

            if (deepLinkingNode != null)
            {
                var schemeAttribute = deepLinkingNode.Attributes.GetNamedItem("scheme");
                if (schemeAttribute != null)
                    scheme = schemeAttribute.Value;
                
                var hostAttribute = deepLinkingNode.Attributes.GetNamedItem("host");
                if (hostAttribute != null)
                    host = hostAttribute.Value;

                var pathPrefixAttribute = deepLinkingNode.Attributes.GetNamedItem("pathPrefix");
                if (pathPrefixAttribute != null)
                    pathPrefix = pathPrefixAttribute.Value;
            }
        }

        // Remove slash from prefix for display
        if (pathPrefix.Length > 0 && pathPrefix[0] == '/')
        {
            displayPathPrefix = pathPrefix.Substring(1);
        }

        dialogWindow.Scheme = scheme;
        dialogWindow.Host = host;
        dialogWindow.PathPrefix = displayPathPrefix;

        dialogWindow.Repaint();
    }

    public static void ConfigureDeepLinks(string scheme, string host, string pathPrefix)
    {
        // Add slash to prefix if needed
        if (pathPrefix == null)
            pathPrefix = "/";
        else if (pathPrefix.Length > 0 && pathPrefix[0] != '/')
            pathPrefix = "/" + pathPrefix;

        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
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
                seedsDeepLinksAar.AddEntry("classes.jar", new byte[]
                    {
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
            #elif UNITY_4_6 || UNITY_4_5
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
        #if UNITY_4_6 || UNITY_4_5
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone)
        #else
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        #endif
        {
            var seedsSettingsXml = new XmlDocument();
            var seedsSettingsFilename = Path.Combine(Application.dataPath, "../ProjectSettings/SeedsSDK.xml");
            if (File.Exists(seedsSettingsFilename))
                seedsSettingsXml.Load(seedsSettingsFilename);

            var seedsSdkNode = seedsSettingsXml.SelectSingleNode("SeedsSDK");
            if (seedsSdkNode == null)
            {
                seedsSdkNode = seedsSettingsXml.CreateElement("SeedsSDK");
                seedsSettingsXml.AppendChild(seedsSdkNode);
            }

            var iosNode = seedsSdkNode.SelectSingleNode("iOS");
            if (iosNode == null)
            {
                iosNode = seedsSettingsXml.CreateElement("iOS");
                seedsSdkNode.AppendChild(iosNode);
            }

            var deepLinkingNode = iosNode.SelectSingleNode("DeepLinking");
            if (deepLinkingNode == null)
            {
                deepLinkingNode = seedsSettingsXml.CreateElement("DeepLinking");
                iosNode.AppendChild(deepLinkingNode);
            }

            var schemeAttribute = (XmlAttribute)deepLinkingNode.Attributes.GetNamedItem("scheme");
            if (schemeAttribute == null)
            {
                schemeAttribute = seedsSettingsXml.CreateAttribute("scheme");
                deepLinkingNode.Attributes.Append(schemeAttribute);
            }

            var hostAttribute = (XmlAttribute)deepLinkingNode.Attributes.GetNamedItem("host");
            if (hostAttribute == null)
            {
                hostAttribute = seedsSettingsXml.CreateAttribute("host");
                deepLinkingNode.Attributes.Append(hostAttribute);
            }

            var pathPrefixAttribute = (XmlAttribute)deepLinkingNode.Attributes.GetNamedItem("pathPrefix");
            if (pathPrefixAttribute == null)
            {
                pathPrefixAttribute = seedsSettingsXml.CreateAttribute("pathPrefix");
                deepLinkingNode.Attributes.Append(pathPrefixAttribute);
            }

            schemeAttribute.Value = scheme;
            hostAttribute.Value = host;
            pathPrefixAttribute.Value = pathPrefix;

            seedsSettingsXml.Save(seedsSettingsFilename);
        }
    }

    [MenuItem("Seeds SDK/Configure", true)]
    public static bool ConfigureDeepLinksEnabled()
    {
        return
            EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
            #if UNITY_4_6 || UNITY_4_5
            EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone
            #else
            EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS
            #endif
            ;
    }

    [PostProcessBuild(1)]
    public static void PostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        #if UNITY_4_6 || UNITY_4_5
        if (target != BuildTarget.iPhone)
        #else
        if (target != BuildTarget.iOS)
        #endif
        {
            return;
        }

        Debug.Log("[Seeds] Going to post-process project in '" + pathToBuiltProject + "'");

        var projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

        var seedsSettingsXml = new XmlDocument();
        var seedsSettingsFilename = Path.Combine(Application.dataPath, "../ProjectSettings/SeedsSDK.xml");
        if (File.Exists(seedsSettingsFilename))
            seedsSettingsXml.Load(seedsSettingsFilename);

        var deepLinkingNode = seedsSettingsXml.SelectSingleNode("SeedsSDK/iOS/DeepLinking");

        var scheme = "";
        var pathPrefix = "";
        var host = "";
        if (deepLinkingNode != null)
        {
            var schemeAttribute = deepLinkingNode.Attributes.GetNamedItem("scheme");
            if (schemeAttribute != null)
                scheme = schemeAttribute.Value;

            var hostAttribute = deepLinkingNode.Attributes.GetNamedItem("host");
            if (hostAttribute != null)
                host = hostAttribute.Value;

            var pathPrefixAttribute = deepLinkingNode.Attributes.GetNamedItem("pathPrefix");
            if (pathPrefixAttribute != null)
                pathPrefix = pathPrefixAttribute.Value;
        }

        // Write SeedsConfig.h
        #if UNITY_5
        var seedsConfigPathInProject = "Libraries/Plugins/iOS";
        #elif UNITY_4_6 || UNITY_4_5
        var seedsConfigPathInProject = "Libraries";
        #else
        #error Unsupported Unity3D version, please contact support.
        #endif
        var seedsConfigFilename = Path.Combine(pathToBuiltProject, Path.Combine(seedsConfigPathInProject, "SeedsConfig.h"));
        using (var seedsConfigFile = File.Open(seedsConfigFilename, FileMode.Create))
        {
            using (var seedsConfig = new StreamWriter(seedsConfigFile))
            {
                seedsConfig.WriteLine("// DeepLinking");
                seedsConfig.WriteLine("#define SEEDS_DeepLinking_scheme @\"{0}\"", scheme);
                seedsConfig.WriteLine("#define SEEDS_DeepLinking_host @\"{0}\"", host);
                seedsConfig.WriteLine("#define SEEDS_DeepLinking_pathPrefix @\"{0}\"", pathPrefix);
            }
        }

        // Convert "Unity-iPhone.xcodeproj/project.pbxproj" to xml1 format using plutil
        System.Diagnostics.Process
            .Start(new System.Diagnostics.ProcessStartInfo("plutil")
            {
                Arguments = "-convert xml1 \"Unity-iPhone.xcodeproj/project.pbxproj\"",
                UseShellExecute = true,
                WorkingDirectory = pathToBuiltProject
            })
            .WaitForExit();

        // Read pbxproj as XML
        var pbxprojFilename = Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj/project.pbxproj");
        var pbxproj = new XmlDocument();
        pbxproj.Load(pbxprojFilename);
        var objectsNode = pbxproj.SelectSingleNode("/plist/dict/key[text()=\"objects\"]/following-sibling::*[1]");
        var targetNode = pbxproj.SelectSingleNode(
             "//dict[" +
             "key[text()=\"isa\"]/following-sibling::*[1]/text()=\"PBXNativeTarget\" and " +
             "key[text()=\"name\"]/following-sibling::*[1]/text()=\"Unity-iPhone\"" +
             "]");

        #if UNITY_4_6 || UNITY_4_5
        // Copy SeedsResources.bundle
        var seedsResourcesPathInAssets = "Assets/Plugins/iOS/SeedsResources.bundle/";
        var seedsResources = AssetDatabase.GetAllAssetPaths()
            .Where(x => x.StartsWith(seedsResourcesPathInAssets))
            .ToList();
        var seedsResourcesPathInProject = "Libraries/SeedsResources.bundle";
        var seedsResourcesPath = Path.Combine(pathToBuiltProject, seedsResourcesPathInProject);
        Directory.CreateDirectory(seedsResourcesPath);
        foreach (var seedsResourcePath in seedsResources)
        {
            var srcPath = Path.Combine(projectPath, seedsResourcePath);
            var dstPath = Path.Combine(projectPath, seedsResourcePath.Replace(seedsResourcesPathInAssets, ""));
            var attributes = File.GetAttributes(srcPath);

            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                Directory.CreateDirectory(dstPath);
            else if (!File.Exists(dstPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dstPath));
                File.Copy(srcPath, dstPath);
            }
        }

        // Add SeedsResources.bundle to the project
        var seedsResourcesFileRefKey = GeneratePbxprojKey(pbxproj);
        objectsNode.AppendChildElement("key", seedsResourcesFileRefKey);
        var seedsResourcesFileRefNode = objectsNode.AppendChildElement("dict");
        seedsResourcesFileRefNode.AppendChildElement("key", "isa");
        seedsResourcesFileRefNode.AppendChildElement("string", "PBXFileReference");
        seedsResourcesFileRefNode.AppendChildElement("key", "lastKnownFileType");
        seedsResourcesFileRefNode.AppendChildElement("string", "wrapper.plug-in");
        seedsResourcesFileRefNode.AppendChildElement("key", "name");
        seedsResourcesFileRefNode.AppendChildElement("string", "SeedsResources.bundle");
        seedsResourcesFileRefNode.AppendChildElement("key", "path");
        seedsResourcesFileRefNode.AppendChildElement("string", seedsResourcesPathInProject);
        seedsResourcesFileRefNode.AppendChildElement("key", "sourceTree");
        seedsResourcesFileRefNode.AppendChildElement("string", "SOURCE_ROOT");

        var seedsResourcesBuildFileKey = GeneratePbxprojKey(pbxproj);
        objectsNode.AppendChildElement("key", seedsResourcesBuildFileKey);
        var seedsResourcesBuildFileNode = objectsNode.AppendChildElement("dict");
        seedsResourcesBuildFileNode.AppendChildElement("key", "fileRef");
        seedsResourcesBuildFileNode.AppendChildElement("string", seedsResourcesFileRefKey);
        seedsResourcesBuildFileNode.AppendChildElement("key", "isa");
        seedsResourcesBuildFileNode.AppendChildElement("string", "PBXBuildFile");

        var librariesGroupChildrenNode = pbxproj.SelectSingleNode(
            "//dict[" +
            "key[text()=\"isa\"]/following-sibling::*[1]/text()=\"PBXGroup\" and " +
            "key[text()=\"path\"]/following-sibling::*[1]/text()=\"Libraries\"" +
            "]/key[text()=\"children\"]/following-sibling::*[1]");
        librariesGroupChildrenNode.AppendChildElement("string", seedsResourcesFileRefKey);
        
        AddFileReferenceToBuildPhase(pbxproj, "PBXResourcesBuildPhase", seedsResourcesBuildFileKey);
        #endif

        // Add missing system frameworks
        AddSystemFramework(pbxproj, "CoreData");
        AddSystemFramework(pbxproj, "CoreTelephony");

        // Tweak build configurations
        var buildConfigurationListKey = targetNode
            .SelectSingleNode("key[text()=\"buildConfigurationList\"]/following-sibling::*[1]/text()")
            .Value;
        var buildConfigurationListNode = objectsNode.SelectSingleNode(
            string.Format("key[text()=\"{0}\"]/following-sibling::*[1]", buildConfigurationListKey));
        var buildConfigurationList =
            buildConfigurationListNode.SelectNodes("key[text()=\"buildConfigurations\"]/following-sibling::*[1]/string/text()");
        foreach (XmlNode buildConfigurationListEntry in buildConfigurationList)
        {
            var buildConfigurationKey = buildConfigurationListEntry.Value;
            var buildConfigurationNode = objectsNode.SelectSingleNode(
                string.Format("key[text()=\"{0}\"]/following-sibling::*[1]", buildConfigurationKey));

            var buildSettings =
                buildConfigurationNode.SelectSingleNode("key[text()=\"buildSettings\"]/following-sibling::*[1]");

            var otherLinkerFlagsNode = buildSettings.SelectSingleNode("key[text()=\"OTHER_LDFLAGS\"]/following-sibling::*[1]");
            if (otherLinkerFlagsNode == null)
            {
                buildSettings.AppendChildElement("key", "OTHER_LDFLAGS");
                otherLinkerFlagsNode = buildSettings.AppendChildElement("array");
            }
            
            // Add -ObjC flag to linker flags of the target
            var objCFlagNode = otherLinkerFlagsNode.SelectSingleNode("string[text()=\"-ObjC\"]");
            if (objCFlagNode == null)
                objCFlagNode = otherLinkerFlagsNode.AppendChildElement("string", "-ObjC");
        }

        //TODO: Find and modify project plist file

        // Save pbxproj as XML and fix it
        var xmlWriterSettings = new XmlWriterSettings
        {
            Indent = false,
            NewLineHandling = NewLineHandling.None
        };
        using (var xmlWriter = XmlWriter.Create(pbxprojFilename, xmlWriterSettings))
        {
            pbxproj.Save(xmlWriter);
        }
        var pbxprojContent = File.ReadAllText(pbxprojFilename);
        pbxprojContent = pbxprojContent
            .Replace(
                "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\"[]>",
                "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
        File.WriteAllText(pbxprojFilename, pbxprojContent);
    }

    private static string GeneratePbxprojKey(XmlDocument pbxproj)
    {
        var keyBytes = new byte[12];
        var random = new System.Random();
        for (;;)
        {
            random.NextBytes(keyBytes);
            var key = BitConverter.ToString(keyBytes).Replace("-", "").ToUpper();

            var conflictingKeyNode = pbxproj.SelectSingleNode(string.Format("//key[text()=\"{0}\"]", key));
            if (conflictingKeyNode == null)
                return key;
        }
    }

    private static void AddFileReferenceToBuildPhase(XmlDocument pbxproj, string buildPhase, string fileRefKey)
    {
        var targetNode = pbxproj.SelectSingleNode(
            "//dict[" +
            "key[text()=\"isa\"]/following-sibling::*[1]/text()=\"PBXNativeTarget\" and " +
            "key[text()=\"name\"]/following-sibling::*[1]/text()=\"Unity-iPhone\"" +
            "]");
        var targetBuildPhasesKeyNodes =
            targetNode.SelectNodes("key[text()=\"buildPhases\"]/following-sibling::*[1]/string/text()");
        var buildPhaseNodes =
            pbxproj.SelectNodes(string.Format("//dict[key[text()=\"isa\"]/following-sibling::*[1]/text()=\"{0}\"]", buildPhase));

        XmlNode resourcesBuildPhaseNode = null;
        foreach (XmlNode buildPhaseNode in buildPhaseNodes)
        {
            var key = buildPhaseNode.PreviousSibling.FirstChild.Value;
            foreach (XmlNode targetBuildPhasesKeyNode in targetBuildPhasesKeyNodes)
            {
                if (targetBuildPhasesKeyNode.Value == key)
                {
                    resourcesBuildPhaseNode = buildPhaseNode;
                    break;
                }
            }

            if (resourcesBuildPhaseNode != null)
                break;
        }
        var fileRefs = resourcesBuildPhaseNode
            .SelectSingleNode("key[text()=\"files\"]/following-sibling::*[1]");
        if (fileRefs.SelectSingleNode(string.Format("string[text()=\"{0}\"]", fileRefKey)) == null)
            fileRefs.AppendChildElement("string", fileRefKey);
    }

    private static void AddSystemFramework(XmlDocument pbxproj, string frameworkName)
    {
        var fullFrameworkPath = string.Format("System/Library/Frameworks/{0}.framework", frameworkName);

        var objectsNode = pbxproj.SelectSingleNode("/plist/dict/key[text()=\"objects\"]/following-sibling::*[1]");

        string frameworkFileRefKey;
        var frameworkFileRefNode = objectsNode.SelectSingleNode(string.Format(
            "dict[" +
            "key[text()=\"isa\"]/following-sibling::*[1]/text()=\"PBXFileReference\" and " +
            "key[text()=\"path\"]/following-sibling::*[1]/text()=\"{0}\"" +
            "]", fullFrameworkPath));
        if (frameworkFileRefNode != null)
        {
            frameworkFileRefKey = frameworkFileRefNode.PreviousSibling.FirstChild.Value;
        }
        else
        {
            frameworkFileRefKey = GeneratePbxprojKey(pbxproj);
            objectsNode.AppendChildElement("key", frameworkFileRefKey);
            frameworkFileRefNode = objectsNode.AppendChildElement("dict");
            frameworkFileRefNode.AppendChildElement("key", "isa");
            frameworkFileRefNode.AppendChildElement("string", "PBXFileReference");
            frameworkFileRefNode.AppendChildElement("key", "lastKnownFileType");
            frameworkFileRefNode.AppendChildElement("string", "wrapper.framework");
            frameworkFileRefNode.AppendChildElement("key", "name");
            frameworkFileRefNode.AppendChildElement("string", string.Format("{0}.framework", frameworkName));
            frameworkFileRefNode.AppendChildElement("key", "path");
            frameworkFileRefNode.AppendChildElement("string", fullFrameworkPath);
            frameworkFileRefNode.AppendChildElement("key", "sourceTree");
            frameworkFileRefNode.AppendChildElement("string", "SDKROOT");
        }

        string frameworkBuildFileKey;
        var frameworkBuildFileNode = objectsNode.SelectSingleNode(string.Format(
            "dict[" +
            "key[text()=\"isa\"]/following-sibling::*[1]/text()=\"PBXBuildFile\" and " +
            "key[text()=\"fileRef\"]/following-sibling::*[1]/text()=\"{0}\"" +
            "]", frameworkFileRefKey));
        if (frameworkBuildFileNode != null)
        {
            frameworkBuildFileKey = frameworkBuildFileNode.PreviousSibling.FirstChild.Value;
        }
        else
        {
            frameworkBuildFileKey = GeneratePbxprojKey(pbxproj);
            objectsNode.AppendChildElement("key", frameworkBuildFileKey);
            frameworkBuildFileNode = objectsNode.AppendChildElement("dict");
            frameworkBuildFileNode.AppendChildElement("key", "fileRef");
            frameworkBuildFileNode.AppendChildElement("string", frameworkFileRefKey);
            frameworkBuildFileNode.AppendChildElement("key", "isa");
            frameworkBuildFileNode.AppendChildElement("string", "PBXBuildFile");
        }

        var frameworksGroupChildrenNode = pbxproj.SelectSingleNode(
            "//dict[" +
            "key[text()=\"isa\"]/following-sibling::*[1]/text()=\"PBXGroup\" and " +
            "key[text()=\"name\"]/following-sibling::*[1]/text()=\"Frameworks\"" +
            "]/key[text()=\"children\"]/following-sibling::*[1]");
        if (frameworksGroupChildrenNode.SelectSingleNode(string.Format("string[text()=\"{0}\"]", frameworkFileRefKey)) == null)
            frameworksGroupChildrenNode.AppendChildElement("string", frameworkFileRefKey);

        AddFileReferenceToBuildPhase(pbxproj, "PBXFrameworksBuildPhase", frameworkBuildFileKey);
    }
}
