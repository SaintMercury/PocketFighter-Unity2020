using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[DisallowMultipleComponent]
public class NetworkedStripPrefab : NetworkBehaviour {

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (hasAuthority == false) {
            if (TryGetComponent<PlayerInput>(out PlayerInput i)) {
                Destroy(i);
            }
        }
        Destroy(this);
    }
}
