using Mirror;
using UnityEngine;

public class NetworkedPlayerRepresentative : NetworkBehaviour {

    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject bulletDropPrefab;

    [SyncVar(hook = nameof(toggleCursorVisibility))]
    private GameObject playerObject;

    private void toggleCursorVisibility(GameObject _, GameObject newValue) {
        Debug.Log("Toggling cursor");
        if (hasAuthority) {
            GameManager.instance.ToggleCursorVisibility(newValue == null);
        }
    }

    void OnDestroy() {
        if (hasAuthority) {
            GameManager.instance.ToggleCursorVisibility(true);
        }
    }

    [Client]
    public void Spawn() {
        if (playerObject == null) {
            CmdRequestSpawn();
        }
    }

    [Client]
    public void QuitMatch() {
        if (isClientOnly) {
            NetworkManager.singleton.StopClient();
        }

        if (isServer && isClient) {
            NetworkManager.singleton.StopHost();
        }
    }

    [Command]
    public void CmdRequestSpawn() {
        if (playerObject == null) {
            var playerInstance = Instantiate(
                prefab,
                Vector3.zero, // new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * 10f,
                Quaternion.identity
            );
            playerInstance.GetComponent<NetworkedPlayer>().GameObjectDestroyed += OnPlayerDestroy;
            NetworkServer.Spawn(playerInstance, connectionToClient);
            playerObject = playerInstance;
        }
    }

    [Server]
    private void OnPlayerDestroy(NetworkedPlayer player) {
        player.GameObjectDestroyed -= OnPlayerDestroy;

        if (playerObject.TryGetComponent<NetworkedShoot>(out NetworkedShoot shoot)) {
            if (shoot.ammo > 0) {
                GameObject bulletDrop = Instantiate(bulletDropPrefab, playerObject.transform.position, Quaternion.identity);
                bulletDrop.GetComponent<NetworkedBulletPickup>().amount = shoot.ammo;
                NetworkServer.Spawn(bulletDrop);
            }
        }
        playerObject = null;
        RpcPlayDeathSound();
    }

    [ClientRpc]
    private void RpcPlayDeathSound() {
        AudioManager.instance.Play("Death");
    }

    [Client]
    void OnGUI() {
        if (hasAuthority == false) return;

        GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
                if (playerObject == null)
                    GUILayout.Label("Press J to Spawn!");
                GUILayout.Label("Press ESC to quit to main menu");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
                if (playerObject != null) 
                    GUILayout.Label($"Ammo: {playerObject.GetComponent<NetworkedShoot>().ammo}");
            GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

}
