using System.Collections;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkTransform))]
public class NetworkedBullet : NetworkBehaviour {
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private Rigidbody2D _rigidbody;

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(KillSelf());
    }

    private IEnumerator KillSelf() {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    public void SetBulletDirection(Vector2 direction) {
        _rigidbody.velocity = direction * bulletSpeed;

        // turn bullet towards velocity
        float angle = direction.y == 1 ? -90 :
            direction.y == -1 ? 90 :
            direction.x == 1 ? 180 :
            0;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (isClient) {
            // Hmmmmm
        }

        if (isServer) {
            if (collider.TryGetComponent<NetworkedPlayer>(out NetworkedPlayer player)) {
                Destroy(collider.gameObject);
                // TODO: Report on player death
            }
            Destroy(gameObject);
        }
    }
}
