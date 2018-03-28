#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal.VR;
using UnityEngine;

public sealed class BuildManagerEditor : EditorWindow
{
    private string _gameName = "TESUnityXR";
    private string _prefixBuildName = "TESUnityXR";
    private string _version = "1.0.0";
    private bool[] _standaloneBuilds = new bool[] { true, true, true, true };
    private string[] _standaloneNames = new string[] { "Windows x86", "Windows x64", "Linux", "Mac" };
    private bool _initialized = false;

    [MenuItem("TESUnity/Build Manager")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BuildManagerEditor));
    }

    private void OnGUI()
    {
        if (!_initialized)
        {
            _version = TESUnity.TESManager.Version;
            _initialized = true;
        }

        GUILayout.Label("Build Settings", EditorStyles.boldLabel);

        _gameName = EditorGUILayout.TextField("Game Name", _gameName);
        _prefixBuildName = EditorGUILayout.TextField("Prefix Build Name", _prefixBuildName);
        _version = EditorGUILayout.TextField("Version", _version);

        GUILayout.Label("Desktop", EditorStyles.boldLabel);
        GUILayout.Label("Build Targets", EditorStyles.boldLabel);
        for (var i = 0; i < _standaloneNames.Length; i++)
            _standaloneBuilds[i] = EditorGUILayout.Toggle(_standaloneNames[i], _standaloneBuilds[i]);

        if (GUILayout.Button("Build"))
            MakeDesktopBuilds();
    }

    private void MakeDesktopBuilds()
    {
        var path = EditorUtility.SaveFolderPanel("Choose Builds Folder", "", "");

        var targets = new BuildTarget[]
        {
            BuildTarget.StandaloneWindows,
            BuildTarget.StandaloneWindows64,
            BuildTarget.StandaloneLinuxUniversal,
            BuildTarget.StandaloneOSX
        };

        for (var i = 0; i < targets.Length; i++)
        {
            if (!_standaloneBuilds[i])
                continue;

            MakeDesktopBuild(targets[i], path, i);
        }
    }

    private void MakeDesktopBuild(BuildTarget target, string path, int index)
    {
        var folder = string.Format("{0}_{1}-{2}", _prefixBuildName, _version, target);
        var finalPath = string.Format("{0}/{1}/", path, folder);

        PlayerSettings.virtualRealitySupported = true;
        BuildPipeline.BuildPlayer(GetLevels(), string.Format("{0}{1}{2}", finalPath, _gameName, GetExtensionForTarget(target)), target, BuildOptions.None);
        CopyDocs(ref finalPath, target);
    }

    private string GetExtensionForTarget(BuildTarget target)
    {
        var ext = ".exe";

        switch (target)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64: ext = ".exe"; break;
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal: ext = ""; break;
            case BuildTarget.StandaloneOSX: ext = ".app"; break;
        }

        return ext;
    }

    private string GetTargetString(BuildTarget target)
    {
        var str = "windows";

        switch (target)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64: str = "windows"; break;
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal: str = "linux"; break;
            case BuildTarget.StandaloneOSX: str = "osx"; break;
        }

        return str;
    }

    public string GetTargetArch(BuildTarget target)
    {
        if (target == BuildTarget.StandaloneWindows)
            return "-x86";
        else if (target == BuildTarget.StandaloneWindows64)
            return "-x64";

        return string.Empty;
    }

    private string[] GetLevels() => new[] { "Assets/Scenes/AskPathScene.unity", "Assets/Scenes/GameScene.unity" };


    private string GetBoolString(bool predicat)
    {
        return predicat.ToString().ToLower();
    }

    private void CopyDocs(ref string path, BuildTarget target)
    {
        var docPath = Directory.GetCurrentDirectory().Replace('\\', '/');
        Copy("README.md", ref docPath, ref path);
        Copy("README-VR.md", ref docPath, ref path);
        Copy("config.ini", ref docPath, ref path);
        Copy("LICENSE.txt", ref docPath, ref path);
        Copy("CHANGES.md", ref docPath, ref path);
    }

    private void Copy(string filename, ref string source, ref string dest)
    {
        File.Copy(source + filename, dest + filename, true);
    }
}
#endif