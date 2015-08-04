using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using Ionic.Zip;

public static class SeedsBuild
{
    [MenuItem("Seeds SDK/Build package")]
    public static void BuildPackage()
    {
        EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.0f);

        try
        {
            var assetPaths = new string[]
            {
                "Assets/Plugins",
                "Assets/Seeds/Seeds.cs",
                "Assets/Seeds/Demo",
            };

            var packagePath = "SeedsSDK.unitypackage";

            EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.25f);

            var importOpts = ImportAssetOptions.Default;
            importOpts |= ImportAssetOptions.ForceSynchronousImport;
            importOpts |= ImportAssetOptions.ForceUpdate;
            importOpts |= ImportAssetOptions.ImportRecursive;
            AssetDatabase.Refresh(importOpts);

            EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.5f);

            var exportOpts = ExportPackageOptions.Recurse;
            AssetDatabase.ExportPackage(assetPaths, packagePath, exportOpts);

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

    [MenuItem("Seeds SDK/Build package (legacy)")]
    public static void BuildLegacyPackage()
    {
        var projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        var assetsPath = Path.Combine(projectPath, "Assets");
        var pluginsPath = Path.Combine(assetsPath, "Plugins");
        var androidPluginsPath = Path.Combine(pluginsPath, "Android");
        var originalAssetsPath = Path.Combine(projectPath, "Assets_Original");
        var originalPluginsPath = Path.Combine(originalAssetsPath, "Plugins");
        var originalAndroidPluginsPath = Path.Combine(originalPluginsPath, "Android");

        EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.0f);
        try
        {
            if (Directory.Exists(originalAssetsPath))
                Directory.Delete(originalAssetsPath, true);
            Directory.CreateDirectory(originalPluginsPath);
            Directory.Move(androidPluginsPath, originalAndroidPluginsPath);
            Directory.CreateDirectory(androidPluginsPath);
            foreach (var pluginFilepath in Directory.GetFiles(originalAndroidPluginsPath))
            {
                var pluginFilename = Path.GetFileName(pluginFilepath);
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
                                Debug.LogFormat("TDB: {0}:{1} merged", pluginFilename, zipEntry.FileName);
                            }
                            else if (
                                (zipEntry.Attributes & FileAttributes.Directory) == 0 &&
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
//                            else
//                                Debug.Log(pluginFilename + " > " + zippedFile.FileName);
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

            var assetPaths = new string[]
            {
                "Assets/Plugins",
                "Assets/Seeds/Seeds.cs",
                "Assets/Seeds/Demo",
            };

            var packagePath = "SeedsSDK_Legacy.unitypackage";

            EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.25f);

            var importOpts = ImportAssetOptions.Default;
            importOpts |= ImportAssetOptions.ForceSynchronousImport;
            importOpts |= ImportAssetOptions.ForceUpdate;
            importOpts |= ImportAssetOptions.ImportRecursive;
            AssetDatabase.Refresh(importOpts);

            EditorUtility.DisplayProgressBar("Seeds SDK", "Building package", 0.5f);

            var exportOpts = ExportPackageOptions.Recurse;
            AssetDatabase.ExportPackage(assetPaths, packagePath, exportOpts);

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
