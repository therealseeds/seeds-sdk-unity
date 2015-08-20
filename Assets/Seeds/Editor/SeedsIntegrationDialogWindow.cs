using UnityEditor;
using UnityEngine;

public class SeedsIntegrationDialogWindow : EditorWindow
{
    public string Scheme { get; set; }
    public string Host { get; set; }
    public string PathPrefix { get; set; }

    void OnGUI()
    {
        GUILayout.Label("Deep Link URL", EditorStyles.boldLabel);
        Scheme = EditorGUILayout.TextField("Scheme", Scheme);
        Host = EditorGUILayout.TextField("Host", Host);
        PathPrefix = EditorGUILayout.TextField("Path prefix", PathPrefix);

        if (GUILayout.Button("Configure"))
        {
            OnConfigure();
            GUIUtility.ExitGUI();
        }
    }

    void OnConfigure()
    {
        Scheme = Scheme.Trim();
        if (string.IsNullOrEmpty(Scheme))
        {
            EditorUtility.DisplayDialog("Unable configure deep links", "Please specify a valid scheme.", "Close");
            return;
        }

        Host = Host.Trim();
        if (string.IsNullOrEmpty(Host))
        {
            EditorUtility.DisplayDialog("Unable configure deep links", "Please specify a valid host.", "Close");
            return;
        }

        PathPrefix = PathPrefix.Trim();

        SeedsIntegration.ConfigureDeepLinks(Scheme, Host, PathPrefix);

        Close();
    }
}
