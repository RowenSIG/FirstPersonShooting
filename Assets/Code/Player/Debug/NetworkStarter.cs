using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkStarter : MonoBehaviour {
    private NetworkRunner runner;

    void Start() {
        runner = GetComponent<NetworkRunner>();

 // Get the current Unity scene’s build index
        int buildIndex = SceneManager.GetActiveScene().buildIndex;

        // Wrap it in a SceneRef
        SceneRef sceneRef = SceneRef.FromIndex(buildIndex);

        // Convert SceneRef into a NetworkSceneInfo
        NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
        sceneInfo.AddSceneRef(sceneRef, LoadSceneMode.Single);


#if UNITY_WEBGL
        runner.StartGame(new StartGameArgs {
            GameMode = GameMode.Shared,
            SessionName = "WebGLRoom",
            Scene = sceneInfo,
            PlayerCount = 4
        });
#else
        // Optional: fallback for native builds
        runner.StartGame(new StartGameArgs {
            GameMode = GameMode.Shared,
            SessionName = "NativeRoom",
            Scene = sceneInfo,
            PlayerCount = 4
        });
#endif
    }
}
