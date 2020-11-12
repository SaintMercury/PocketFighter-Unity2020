using System.Collections;
using Mirror;
using UnityEngine;

public class NetworkedStab : NetworkBehaviour {
    [SerializeField]
    private GameObject SWIPE;

    [SerializeField]
    [SyncVar]
    private float StabCooldown = 0.25f;

    [SyncVar]
    private bool canStab = true;

    [Client]
    public void Stab(Vector2 direction) {
        if (canStab) {
            AudioManager.instance.Play("Slice");
        }
        
        CmdStab(direction);
    }

    [Command]
    private void CmdStab(Vector2 direction) {
        if (canStab) {
            StartCoroutine(RoutineStab(direction));
        }
    }

    private IEnumerator RoutineStab(Vector2 direction) {
        if (canStab) {
            canStab = false;
            CreateBullet(direction);
            yield return new WaitForSeconds(StabCooldown);
            canStab = true;
        }
    }

    [Server]
    private void CreateBullet(Vector2 direction) {
        GameObject swibeObj = Instantiate(SWIPE, transform.position + new Vector3(direction.x, direction.y, 0), Quaternion.identity);
        var swipe = swibeObj.GetComponent<NetworkedSwipe>();
        swipe.SetDirection(direction);
        swipe.SetCreator(GetComponent<NetworkedPlayer>());
        NetworkServer.Spawn(swibeObj);
    }
}
