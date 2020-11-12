using Mirror;

public class NetworkedPlayer : NetworkBehaviour {
    public delegate void Destroyed(NetworkedPlayer player);
    public event Destroyed GameObjectDestroyed;

    void OnDestroy() {
        GameObjectDestroyed?.Invoke(this);
        GameObjectDestroyed = null; // Extra precaution
    }
}
