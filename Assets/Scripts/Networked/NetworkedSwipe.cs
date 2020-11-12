using System.Collections;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkTransform))]
public class NetworkedSwipe : NetworkBehaviour {
    [SerializeField] private float lifetime = 0.1f;
    private NetworkedPlayer creator;

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(KillSelf());
    }

    private IEnumerator KillSelf() {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    public void SetDirection(Vector2 direction) {
        float angle = direction.y == 1 ? -90 :
            direction.y == -1 ? 90 :
            direction.x == 1 ? 180 :
            0;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetCreator(NetworkedPlayer player) {
        creator = player;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (isClient) {
            // Hmmmmm
        }

        if (isServer) {
            if (collider.TryGetComponent<NetworkedPlayer>(out NetworkedPlayer player)) {
                if (player != creator)
                    Destroy(collider.gameObject);
                // TODO: Report on player death
            }
        }
    }
}
