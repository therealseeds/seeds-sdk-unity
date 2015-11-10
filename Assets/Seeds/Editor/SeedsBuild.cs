using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Ionic.Zip;
using UnityEditor;
using UnityEngine;

public static class SeedsBuild
{
    [MenuItem("Edit/Seeds SDK/Build package")]
    public static void BuildPackage()
    {
        var projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

        EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.0f);

        try
        {
            var packagePath = "SeedsSDK.unitypackage";

            EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.25f);

            var importOpts = ImportAssetOptions.Default;
            importOpts |= ImportAssetOptions.ForceSynchronousImport;
            importOpts |= ImportAssetOptions.ForceUpdate;
            importOpts |= ImportAssetOptions.ImportRecursive;
            AssetDatabase.Refresh(importOpts);
            var allAssets = AssetDatabase.GetAllAssetPaths();

            EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.5f);

            var assetsToExport = new List<string>();
            assetsToExport.Add("Assets/Seeds/Seeds.cs");
            assetsToExport.Add("Assets/Seeds/Seeds.prefab");
            assetsToExport.Add("Assets/Seeds/SeedsDeepLinks.cs");
            assetsToExport.Add("Assets/Seeds/SeedsDeepLinks.prefab");
            assetsToExport.Add("Assets/Seeds/Documentation.pdf");
            assetsToExport.Add("Assets/Seeds/Editor/SeedsIntegration.cs");
            assetsToExport.Add("Assets/Seeds/Editor/SeedsIntegrationDialogWindow.cs");
            assetsToExport.Add("Assets/Seeds/Editor/SeedsUtilities.cs");
            assetsToExport.Add("Assets/Seeds/Editor/Ionic.Zip.dll");
            assetsToExport.AddRange(allAssets.Where(x =>
                x.StartsWith("Assets/Plugins") &&
                !x.EndsWith("SeedsDeepLink.aar") &&
                File.Exists(Path.Combine(projectPath, x))));
            assetsToExport.AddRange(allAssets.Where(x =>
                x.StartsWith("Assets/Seeds/Demo") &&
                File.Exists(Path.Combine(projectPath, x))));
            var exportOpts = ExportPackageOptions.Recurse;
            AssetDatabase.ExportPackage(assetsToExport.ToArray(), packagePath, exportOpts);

            EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.9f);
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("[Seeds] Build failed : {0}", e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    [MenuItem("Edit/Seeds SDK/Build package (legacy)")]
    public static void BuildLegacyPackage()
    {
        var projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        var assetsPath = Path.Combine(projectPath, "Assets");
        var pluginsPath = Path.Combine(assetsPath, "Plugins");
        var androidPluginsPath = Path.Combine(pluginsPath, "Android");
        var originalAssetsPath = Path.Combine(projectPath, "Assets_Original");
        var originalPluginsPath = Path.Combine(originalAssetsPath, "Plugins");
        var originalAndroidPluginsPath = Path.Combine(originalPluginsPath, "Android");
        #if UNITY_5_0 || UNITY_5_1
        var playerAndroidManifestPath =
            Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/AndroidPlayer/AndroidManifest.xml");
        #else
        var playerAndroidManifestPath =
            Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/AndroidPlayer/Apk/AndroidManifest.xml");
        #endif
        var seedsAndroidManifestPath =
            Path.Combine(androidPluginsPath, "AndroidManifest.xml");

        EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.0f);
        try
        {
            if (Directory.Exists(originalAssetsPath))
                Directory.Delete(originalAssetsPath, true);
            Directory.CreateDirectory(originalPluginsPath);
            Directory.Move(androidPluginsPath, originalAndroidPluginsPath);
            Directory.CreateDirectory(androidPluginsPath);
            var androidManifestDocument = new XmlDocument();
            androidManifestDocument.Load(playerAndroidManifestPath);
            var androidNS = "http://schemas.android.com/apk/res/android";
            int? minSdkVersion = null;
            int? maxSdkVersion = null;

            foreach (var pluginFilepath in Directory.GetFiles(originalAndroidPluginsPath))
            {
                var pluginFilename = Path.GetFileName(pluginFilepath);

                if (pluginFilename.EndsWith("SeedsDeepLink.aar"))
                    continue;

                if (pluginFilename.EndsWith(".aar"))
                {
                    // Unpack
                    using (var androidLibraryZip = ZipFile.Read(pluginFilepath))
                    {
                        foreach (var zipEntry in androidLibraryZip.Entries)
                        {
                            if (zipEntry.FileName == "classes.jar")
                            {
                                var targetFilename = Path.GetFileNameWithoutExtension(pluginFilename) + ".jar";
                                var targetFilepath = Path.Combine(androidPluginsPath, targetFilename);
                                using (var stream = File.Open(targetFilepath, FileMode.Create))
                                {
                                    zipEntry.Extract(stream);
                                }

                                Debug.LogFormat("{0}:{1} unpacked to {2}", pluginFilename, zipEntry.FileName, targetFilename);
                            }
                            else if (zipEntry.FileName.EndsWith(".jar"))
                            {
                                var targetFilename =
                                    Path.GetFileNameWithoutExtension(pluginFilename) +
                                    "_" +
                                    Path.GetFileName(zipEntry.FileName);
                                var targetFilepath = Path.Combine(androidPluginsPath, targetFilename);
                                using (var stream = File.Open(targetFilepath, FileMode.Create))
                                {
                                    zipEntry.Extract(stream);
                                }

                                Debug.LogFormat("{0}:{1} unpacked to {2}", pluginFilename, zipEntry.FileName, targetFilename);
                            }
                            else if (zipEntry.FileName == "AndroidManifest.xml")
                            {
                                var targetFilename = Path.GetFileNameWithoutExtension(pluginFilename) + "_AndroidManifest.xml";
                                var targetFilepath = Path.Combine(androidPluginsPath, targetFilename);
                                using (var stream = File.Open(targetFilepath, FileMode.Create))
                                {
                                    zipEntry.Extract(stream);
                                }

                                var manifestToMerge = new XmlDocument();
                                manifestToMerge.Load(targetFilepath);

                                var manifestNode = androidManifestDocument.SelectSingleNode("/manifest");
                                var manifestDeclarations = manifestToMerge.SelectNodes("manifest/*");
                                foreach (XmlNode manifestDeclaration in manifestDeclarations)
                                {
                                    if (manifestDeclaration.Name == "application")
                                        continue;
                                    else if (manifestDeclaration.Name == "uses-sdk")
                                    {
                                        var minSdkVersionNode =
                                            manifestDeclaration.Attributes.GetNamedItem("minSdkVersion", androidNS);
                                        if (minSdkVersionNode != null)
                                        {
                                            int value = int.Parse(minSdkVersionNode.Value);
                                            if (minSdkVersion == null)
                                                minSdkVersion = value;
                                            else
                                                minSdkVersion = Math.Max(minSdkVersion.Value, value);
                                        }

                                        var maxSdkVersionNode =
                                            manifestDeclaration.Attributes.GetNamedItem("maxSdkVersion", androidNS);
                                        if (maxSdkVersionNode != null)
                                        {
                                            int value = int.Parse(maxSdkVersionNode.Value);
                                            if (maxSdkVersion == null)
                                                maxSdkVersion = value;
                                            else
                                                maxSdkVersion = Math.Min(maxSdkVersion.Value, value);
                                        }

                                        continue;
                                    }

                                    var importedManifestDeclaration =
                                        androidManifestDocument.ImportNode(manifestDeclaration, true);

                                    manifestNode.AppendChild(importedManifestDeclaration);
                                }

                                var applicationNode = androidManifestDocument.SelectSingleNode("/manifest/application");
                                var applicationDeclarations = manifestToMerge.SelectNodes("manifest/application/*");
                                foreach (XmlNode applicationDeclaration in applicationDeclarations)
                                {
                                    var importedApplicationDeclaration =
                                        androidManifestDocument.ImportNode(applicationDeclaration, true);

                                    applicationNode.AppendChild(importedApplicationDeclaration);
                                }

                                File.Delete(targetFilename);
                            }
                            else if (
                                (zipEntry.Attributes & FileAttributes.Directory) == 0 &&
                                Path.GetFileName(zipEntry.FileName).Length > 0 &&
                                (zipEntry.FileName.StartsWith("res/") || zipEntry.FileName.StartsWith("assets/")))
                            {
                                var targetFilepath = Path.Combine(androidPluginsPath, zipEntry.FileName);
                                Directory.CreateDirectory(Path.GetDirectoryName(targetFilepath));
                                using (var stream = File.Open(targetFilepath, FileMode.Create))
                                {
                                    zipEntry.Extract(stream);
                                }

                                Debug.LogFormat("{0}:{1} unpacked to {2}", pluginFilename, zipEntry.FileName, zipEntry.FileName);
                            }
                        }
                    }
                    continue;
                }
                else if (pluginFilename.EndsWith(".aar.meta"))
                {
                    // Just skip
                    continue;
                }
                else
                {
                    // Copy as-is
                    File.Copy(pluginFilepath, Path.Combine(androidPluginsPath, pluginFilename));
                }
            }
            if (minSdkVersion != null || maxSdkVersion != null)
            {
                var usesSdkNode = androidManifestDocument.CreateNode(XmlNodeType.Element, "uses-sdk", "");

                if (minSdkVersion != null)
                {
                    var minSdkVersionAttribute = androidManifestDocument.CreateAttribute("android", "minSdkVersion", androidNS);
                    minSdkVersionAttribute.Value = minSdkVersion.Value.ToString();
                    usesSdkNode.Attributes.Append(minSdkVersionAttribute);
                }

                if (maxSdkVersion != null)
                {
                    var maxSdkVersionAttribute = androidManifestDocument.CreateAttribute("android", "maxSdkVersion", androidNS);
                    maxSdkVersionAttribute.Value = maxSdkVersion.Value.ToString();
                    usesSdkNode.Attributes.Append(maxSdkVersionAttribute);
                }
              
                var manifestNode = androidManifestDocument.SelectSingleNode("/manifest");
                manifestNode.AppendChild(usesSdkNode);
            }
            androidManifestDocument.Save(seedsAndroidManifestPath);

            var packagePath = "SeedsSDK_Legacy.unitypackage";

            EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.25f);

            var importOpts = ImportAssetOptions.Default;
            importOpts |= ImportAssetOptions.ForceSynchronousImport;
            importOpts |= ImportAssetOptions.ForceUpdate;
            importOpts |= ImportAssetOptions.ImportRecursive;
            AssetDatabase.Refresh(importOpts);
            var allAssets = AssetDatabase.GetAllAssetPaths();

            EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.5f);

            var assetsToExport = new List<string>();
            assetsToExport.Add("Assets/Seeds/Seeds.cs");
            assetsToExport.Add("Assets/Seeds/Seeds.prefab");
            assetsToExport.Add("Assets/Seeds/SeedsDeepLinks.cs");
            assetsToExport.Add("Assets/Seeds/SeedsDeepLinks.prefab");
            assetsToExport.Add("Assets/Seeds/Documentation.pdf");
            assetsToExport.Add("Assets/Seeds/Editor/SeedsIntegration.cs");
            assetsToExport.Add("Assets/Seeds/Editor/SeedsIntegrationDialogWindow.cs");
            assetsToExport.Add("Assets/Seeds/Editor/SeedsUtilities.cs");
            assetsToExport.Add("Assets/Seeds/Editor/Ionic.Zip.dll");
            assetsToExport.AddRange(allAssets.Where(
                x => x.StartsWith("Assets/Plugins") &&
                File.Exists(Path.Combine(projectPath, x))));
            assetsToExport.AddRange(allAssets.Where(x =>
                x.StartsWith("Assets/Seeds/Demo") &&
                File.Exists(Path.Combine(projectPath, x))));
            var exportOpts = ExportPackageOptions.Recurse;
            AssetDatabase.ExportPackage(assetsToExport.ToArray(), packagePath, exportOpts);

            EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.9f);

            Directory.Delete(androidPluginsPath, true);
            Directory.Move(originalAndroidPluginsPath, androidPluginsPath);
            Directory.Delete(originalAssetsPath, true);
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("[Seeds] Build failed : {0}", e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }
}
