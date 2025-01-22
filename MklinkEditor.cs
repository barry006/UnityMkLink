#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;

public class MklinkEditor : EditorWindow
{
    private string sourceFolder = "";
    private string destinationFolder = "";
    private string statusMessage = "";

    [MenuItem("Tools/Mklink Folder Linker")] public static void ShowWindow() => GetWindow<MklinkEditor>("Mklink Folder Linker");

    private void OnGUI()
    {
        GUILayout.Label("Mklink Folder Linker", EditorStyles.boldLabel);
        if (GUILayout.Button("Select target folder (real location)"))
            sourceFolder = EditorUtility.OpenFolderPanel("Select target folder", "", "");

        GUILayout.Label($"Target folder: {sourceFolder}");
        if (GUILayout.Button("Select folder for symbolic link (shortcut location)"))
            destinationFolder = EditorUtility.OpenFolderPanel("Select symbolic link folder", "", "");

        GUILayout.Label($"Symbolic link folder: {destinationFolder}");
        if (GUILayout.Button("Execute mklink"))
            ExecuteMklink();

        if (!string.IsNullOrEmpty(statusMessage))
            EditorGUILayout.HelpBox(statusMessage, MessageType.Info);
    }

    private void ExecuteMklink()
    {
        if (string.IsNullOrEmpty(sourceFolder) || string.IsNullOrEmpty(destinationFolder)) { statusMessage = "Please select both folders."; return; }
        if (!Directory.Exists(sourceFolder) || !Directory.Exists(destinationFolder)) { statusMessage = "One of the selected folders does not exist."; return; }

        string linkPath = Path.Combine(destinationFolder, Path.GetFileName(sourceFolder));
        string command = $"mklink /D \"{linkPath}\" \"{sourceFolder}\"";

        try
        {
            Process process = new Process { StartInfo = new ProcessStartInfo { FileName = "cmd.exe", Arguments = $"/c {command}", Verb = "runas", UseShellExecute = true, CreateNoWindow = true } };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode == 0) { statusMessage = $"Symbolic link created successfully: {linkPath}"; AssetDatabase.Refresh(); }
            else { statusMessage = $"Error creating symbolic link (code {process.ExitCode})."; }
        }
        catch (System.Exception ex) { statusMessage = $"Error: {ex.Message}"; }
    }
}
#endif
