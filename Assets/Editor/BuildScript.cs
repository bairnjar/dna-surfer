using UnityEditor;
using UnityEngine;

public class BuildScript
{
    private static string[] Scenes = new[]
    {
        "Assets/Scenes/MenuScreen.unity",
        "Assets/Scenes/ben-dna1p 1.unity",
        "Assets/Scenes/ben-dna1p-easy.unity",
        "Assets/Scenes/ben-dna1p-hard.unity"
    };

    [MenuItem("Build/Build Linux64")]
    public static void BuildLinux64()
    {
        BuildPlayerOptions opts = new BuildPlayerOptions();
        opts.scenes = Scenes;
        opts.locationPathName = "/tmp/dna-surfer-build/DNASurfer";
        opts.target = BuildTarget.StandaloneLinux64;
        opts.options = BuildOptions.None;

        BuildPipeline.BuildPlayer(opts);
        Debug.Log("Linux64 build complete.");
    }

    public static void BuildIos()
    {
        BuildPlayerOptions opts = new BuildPlayerOptions();
        opts.scenes = Scenes;
        opts.locationPathName = "ios-build";
        opts.target = BuildTarget.iOS;
        opts.options = BuildOptions.None;

        BuildPipeline.BuildPlayer(opts);
        Debug.Log("iOS build complete.");
    }
}
