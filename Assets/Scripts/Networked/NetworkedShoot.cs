using System.Collections;
using Mirror;
using UnityEngine;

public class NetworkedShoot : NetworkBehaviour {
    [SerializeField]
    private GameObject BULLET;

    [SerializeField]
    [SyncVar]
    private float ShootCooldown = 0.25f;

    [SyncVar]
    public int ammo = 6;

    [SyncVar]
    private bool canShoot = true;

    [Client]
    public void Shoot(Vector2 direction) {
        // TODO: Note the audio needs to be played before we make the command if we are the server
        // audio logic
        if (canShoot) {
            if (ammo > 0) {
                AudioManager.instance.Play("GunShot");
            } else {
                AudioManager.instance.Play("GunClick");
            }
        }

        CmdShoot(direction);
    }

    [Command]
    private void CmdShoot(Vector2 direction) {
        if (canShoot) {
            StartCoroutine(RoutineShoot(direction));
        }
    }

    private IEnumerator RoutineShoot(Vector2 direction) {
        if (canShoot) {
            canShoot = false;

            if (ammo > 0) {
                ammo--;
                CreateBullet(direction);
            }

            yield return new WaitForSeconds(ShootCooldown);
            canShoot = true;
        }
    }

    [Server]
    private void CreateBullet(Vector2 direction) {
        GameObject bullet = Instantiate(BULLET, transform.position + new Vector3(direction.x, direction.y, 0), Quaternion.identity);
        bullet.GetComponent<NetworkedBullet>().SetBulletDirection(direction);
        NetworkServer.Spawn(bullet);
    }
}
