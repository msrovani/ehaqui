using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Ehaqui.Editor
{
    public static class BuildCommands
    {
        [MenuItem("Build/EHAQUI Android App Bundle")]
        public static void BuildEhaquiAndroid()
        {
            var options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Boot.unity", "Assets/Scenes/Main.unity" },
                locationPathName = "build/ehaqui-android.aab",
                target = BuildTarget.Android,
                options = BuildOptions.CompressWithLz4
            };
            var report = BuildPipeline.BuildPlayer(options);
            Debug.Log($"EHAQUI Android build: {report.summary.result}");
        }

        [MenuItem("Build/ISHERE iOS")]
        public static void BuildIshereIOS()
        {
            var options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Boot.unity", "Assets/Scenes/Main.unity" },
                locationPathName = "build/ishere-ios",
                target = BuildTarget.iOS,
                options = BuildOptions.CompressWithLz4
            };
            var report = BuildPipeline.BuildPlayer(options);
            Debug.Log($"ISHERE iOS build: {report.summary.result}");
        }

        [MenuItem("Build/Android APK (debug)")]
        public static void BuildAndroidDebug()
        {
            var options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Boot.unity", "Assets/Scenes/Main.unity" },
                locationPathName = "build/ehaqui-debug.apk",
                target = BuildTarget.Android,
                options = BuildOptions.Development | BuildOptions.AllowDebugging
            };
            BuildPipeline.BuildPlayer(options);
        }
    }
}
