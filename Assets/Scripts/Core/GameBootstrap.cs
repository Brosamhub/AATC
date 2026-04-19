using UnityEngine;

namespace AroundTheCorner
{
    public static class GameBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.orientation = ScreenOrientation.AutoRotation;

            if (Object.FindFirstObjectByType<GameManager>() != null)
            {
                return;
            }

            EnsureCamera();

            GameObject root = new GameObject("AndAroundTheCornerBootstrap");
            Object.DontDestroyOnLoad(root);
            root.AddComponent<GameManager>();
        }

        private static void EnsureCamera()
        {
            if (Object.FindFirstObjectByType<Camera>() != null)
            {
                return;
            }

            GameObject cameraObject = new GameObject("UICamera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.93f, 0.95f, 0.90f, 1f);
            camera.orthographic = true;
            Object.DontDestroyOnLoad(cameraObject);
        }
    }
}
