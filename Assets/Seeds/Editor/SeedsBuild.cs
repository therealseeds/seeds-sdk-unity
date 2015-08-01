using UnityEngine;
using UnityEditor;

public static class SeedsBuild
{
    [MenuItem("Seeds SDK/Build package")]
    public static void BuildPackage()
    {
        var assetPaths = new string[]
        {
            "Assets/Plugins",
            "Assets/Seeds/Seeds.cs",
            "Assets/Seeds/Demo",
        };

        var packagePath = "SeedsSDK.unitypackage";

        var importOpts = ImportAssetOptions.Default;
        importOpts |= ImportAssetOptions.ForceSynchronousImport;
        importOpts |= ImportAssetOptions.ImportRecursive;
        AssetDatabase.Refresh(importOpts);

        var exportOpts = ExportPackageOptions.Recurse;
        AssetDatabase.ExportPackage(assetPaths, packagePath, exportOpts);
    }
}
