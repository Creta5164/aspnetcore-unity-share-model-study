#if UNITY_EDITOR
using Microsoft.Unity.VisualStudio.Editor;
using UnityEditor;

//https://forum.unity.com/threads/how-can-i-generate-csproj-files-during-continuous-integration-builds.1116448/
namespace GitTools
{
    public static class CsprojGen
    {
        public static void Execute()
        {
            ProjectGeneration projectGeneration = new ProjectGeneration();
            AssetDatabase.Refresh();
            projectGeneration.GenerateAndWriteSolutionAndProjects();
        }
    }
}
#endif