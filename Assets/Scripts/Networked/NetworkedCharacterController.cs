using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

// TODO: Split control layer from presentation layer
public class NetworkedCharacterController : NetworkBehaviour {

    // For the animator
    private enum DIRECTION {
        UP = 0,
        RIGHT = 1, // Basically sideways until I make another sideways anim
        DOWN = 2,
        // LEFT = 3 // Unused
    }


    // TODO: why do I do this??
    [SerializeField] private GunControl Gun;
    [SerializeField] private KnifeControl Knife;
    private NetworkedCharacterMotor _motor;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private NetworkedShoot _netShoot;
    private NetworkedStab _netStab;

    void Start() {
        _motor = GetComponent<NetworkedCharacterMotor>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _netShoot = GetComponent<NetworkedShoot>();
        _netStab = GetComponent<NetworkedStab>();

        _motor.OnDodgeBegin += OnDodgeBegin;
    }

    void OnDestroy() {
        _motor.OnDodgeBegin -= OnDodgeBegin;
    }

    [Client]
    public void OnMove(InputAction.CallbackContext ctx) {
        Vector2 movement = ctx.ReadValue<Vector2>();
        MoveLogic(movement);
    }

    private void MoveLogic(Vector2 movement) {
        _motor.Move(movement);

        // Animation and presentation
        _animator.SetInteger("Direction", (int)convertToDirection(_motor.facingDirection));
        _animator.SetBool("Idle", movement == Vector2.zero);
        // TODO: HACK
        // flip character to accurately display side anim because I only made one
        // need to aslo not flip when strafing
        if (_motor.isStrafing == false && movement != Vector2.zero) {
            FlipSpriteX(movement.x > 0);
            CmdFlipX(movement.x > 0);
        }

        // move gun
        Gun.Aim(_motor.facingDirection);
        Knife.Aim(_motor.facingDirection);
    }

    [Client]
    private DIRECTION convertToDirection(Vector2 vec) {
        if (vec.y == 1) {
            return DIRECTION.UP;
        }
        if (vec.y == -1) {
            return DIRECTION.DOWN;
        }
        return DIRECTION.RIGHT;
    }

    private void FlipSpriteX(bool flip) {
        if (_spriteRenderer.flipX != flip) {
            _spriteRenderer.flipX = flip;
        }
    }

    [Command]
    private void CmdFlipX(bool flip) {
        RpcFlipX(flip);
    }

    [ClientRpc]
    private void RpcFlipX(bool flip) {
        FlipSpriteX(flip);
    }

    [Client]
    public void OnShoot(InputAction.CallbackContext ctx) {
        if (ctx.ReadValueAsButton()) {
            _netShoot.Shoot(_motor.facingDirection);
        }
    }

    [Client]
    public void OnKnife(InputAction.CallbackContext ctx) {
        if (ctx.ReadValueAsButton()) {
            _netStab.Stab(_motor.facingDirection);
        }
    }

    [Client]
    public void OnDodge(InputAction.CallbackContext ctx) {
        if (ctx.ReadValueAsButton()) {
            _motor.Dodge();
        }
    }

    [Client]
    private void OnDodgeBegin() {
        _animator.SetTrigger("Dodge");
    }

    [Client]
    public void OnStrafe(InputAction.CallbackContext ctx) {
        _motor.ToggleStrafing(ctx.ReadValueAsButton());
        // TODO: HACK
        MoveLogic(_motor.moveDirection); // Hack to get around when we toggle moving to update how we are presenting things
    }
}
