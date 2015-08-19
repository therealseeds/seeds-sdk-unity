using UnityEditor;
using UnityEngine;

public class SeedsIntegrationDialogWindow : EditorWindow
{
    private string scheme = "seeds";
    private string host = "project";
    private string pathPrefix = "";

    void OnGUI()
    {
        GUILayout.Label("Deep Link URL", EditorStyles.boldLabel);
        scheme = EditorGUILayout.TextField("Scheme", scheme);
        host = EditorGUILayout.TextField("Host", host);
        pathPrefix = EditorGUILayout.TextField("Path prefix", pathPrefix);

        if (GUILayout.Button("Configure"))
        {
            OnConfigure();
            GUIUtility.ExitGUI();
        }
    }

    void OnConfigure()
    {
        scheme = scheme.Trim();
        if (string.IsNullOrEmpty(scheme))
        {
            EditorUtility.DisplayDialog("Unable configure deep links", "Please specify a valid scheme.", "Close");
            return;
        }

        host = host.Trim();
        if (string.IsNullOrEmpty(host))
        {
            EditorUtility.DisplayDialog("Unable configure deep links", "Please specify a valid host.", "Close");
            return;
        }

        pathPrefix = pathPrefix.Trim();

        SeedsIntegration.ConfigureDeepLinks(scheme, host, pathPrefix);

        Close();
    }
}
