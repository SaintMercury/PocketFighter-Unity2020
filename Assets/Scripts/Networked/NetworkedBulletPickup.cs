using Mirror;
using UnityEngine;

public class NetworkedBulletPickup : NetworkBehaviour {
    [SyncVar]
    public int amount = 1;

    void OnTriggerEnter2D(Collider2D collider) {
        if (isServer) {
            if (collider.TryGetComponent<NetworkedShoot>(out NetworkedShoot shoot)) {
                shoot.ammo += amount;
                Destroy(gameObject);
            }
        }

        if (isClient) {
            // update client
        }
    }
}
