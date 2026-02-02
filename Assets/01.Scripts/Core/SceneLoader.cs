using UnityEngine.SceneManagement;

namespace Core
{
    public static class SceneLoader
    {
        public const string LoginScene = "LoginScene";
        public const string GameScene = "GameScene";

        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
