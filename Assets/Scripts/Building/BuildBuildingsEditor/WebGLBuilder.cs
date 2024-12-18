using UnityEditor;
class WebGLBuilder
{
    static void build()
    {

        // Place all your scenes here 
        //string[] scenes = {"Assets/Scenes/BastianScene.unity"};
        string[] scenes = {"Assets/Scenes/Level1.unity"};

        string pathToDeploy = "Build/StaticFiles/";

        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.WebGL, BuildOptions.None);
    }
}