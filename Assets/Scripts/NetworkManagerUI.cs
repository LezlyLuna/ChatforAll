using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Start Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Start Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Start Server")) NetworkManager.Singleton.StartServer();
        }
        else
        {
            if (GUILayout.Button("Stop")) NetworkManager.Singleton.Shutdown();
        }

        GUILayout.EndArea();
    }
}
