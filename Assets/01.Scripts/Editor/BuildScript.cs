using UnityEditor;
using UnityEngine;

public class BuildScript
{
    public static void PerformWebGLBuild()
    {
        // 1. 빌드 설정에 포함된 씬 목록 가져오기
        string[] scenes = GetEnabledScenes();

        // 2. 빌드 옵션 설정
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "build/WebGL/WebGL",  // 출력 경로
            target = BuildTarget.WebGL,
            options = BuildOptions.None,
        };

        // 3. 빌드 실행
        var report = BuildPipeline.BuildPlayer(options);

        // 4. 결과 확인 및 종료 코드 반환
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.LogError("Build failed!");
            EditorApplication.Exit(1);  // 실패 시 exit code 1
        }

        Debug.Log("Build succeeded!");
        EditorApplication.Exit(0);  // 성공 시 exit code 0
    }

    private static string[] GetEnabledScenes()
    {
        var scenes = EditorBuildSettings.scenes;
        var enabledScenes = new System.Collections.Generic.List<string>();

        foreach (var scene in scenes)
        {
            if (scene.enabled)
            {
                enabledScenes.Add(scene.path);
            }
        }

        return enabledScenes.ToArray();
    }
}