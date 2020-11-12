using System.Collections;
using Mirror;
using UnityEngine;

// TODO: Swap to server authoritative movement. Client is fine for now
[RequireComponent(typeof(NetworkTransform))]
public class NetworkedCharacterMotor : NetworkBehaviour {
    [SerializeField]
    private float dodgeSpeed = 15.0f;
    [SerializeField]
    private float dodgeWindow = 0.5f;
    [SerializeField]
    [SyncVar] private float dodgeCooldown = 2.0f;
    [SyncVar] public bool canDodge = true;
    [SyncVar] private bool isDodging = false;

    public delegate void DodgeCallback();
    public event DodgeCallback OnDodgeBegin;

    [SerializeField]
    private float moveSpeed = 5.0f;
    public bool isStrafing { get; private set; } = false;
    public Vector2 facingDirection { get; private set; } = Vector2.down;
    public Vector2 moveDirection { get; private set; } = Vector2.zero;

    [SerializeField]
    private Rigidbody2D _rigidBody;

    void OnValidate() {
        if (_rigidBody == null) {
            _rigidBody = GetComponent<Rigidbody2D>();
        }
    }

    void FixedUpdate() {
        if (hasAuthority) {
            _rigidBody.AddForce(moveDirection * moveSpeed);
        }
    }

    [Client]
    public void ToggleStrafing(bool value) {
        CmdToggleStrafing(value);
    }

    [Client]
    private void CmdToggleStrafing(bool value) {
        isStrafing = value;
    }

    [Client]
    public void Move(Vector2 direction) {
        moveDirection = direction;
        if (moveDirection != Vector2.zero) {
            if (isStrafing) {
                return;
            }
            // can only move 4 directions, so coerce to sideways if necessary for facing direction
            if (moveDirection.y != 0 && moveDirection.x != 0) {
                facingDirection = new Vector2(direction.x, 0).normalized;
            } else {
                facingDirection = direction;
            }
        }
    }

    // This is taking way longer than expected for me
    [Client]
    public void Dodge() {
        if (canDodge) {
            AudioManager.instance.Play("Dodge");
        }
        CmdDodge();
    }

    [Command]
    private void CmdDodge() {
        StartCoroutine(ServerDodge());
    }

    [Server]
    private IEnumerator ServerDodge() {
        if (canDodge) {
            canDodge = false;
            isDodging = true;
            RpcTriggerOnDodgeBegin();
            yield return new WaitForSeconds(dodgeWindow);
            isDodging = false;
            yield return new WaitForSeconds(dodgeCooldown);
            canDodge = true;
        } else {
            yield return null;
        }
    }

    [ClientRpc]
    private void RpcTriggerOnDodgeBegin() {
        OnDodgeBegin?.Invoke();
        // TODO: fix bug here
        if (hasAuthority) {
            _rigidBody.AddForce(facingDirection * dodgeSpeed);
        }
    }
}
