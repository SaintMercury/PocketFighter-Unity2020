using UnityEngine;

// TODO: Kill self for this
public class KnifeControl : MonoBehaviour {

    private SpriteRenderer _renderer;

    void Start() {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Aim(Vector2 direction) {
        transform.localPosition = new Vector3(
            (direction.y != 0) ? (-0.4f) : (0.65f * Mathf.Sign(direction.x)),
            (direction.y != 0) ? (0.4f * Mathf.Sign(direction.y)) : (-0.25f),
            0
        );
        transform.rotation = Quaternion.Euler(
            0,
            (direction.x > 0) ? 180 : 0,
            direction.y * -90
        );
        _renderer.sortingOrder = (direction.y > 0 || direction.x < 0) ? 1 : -1;
    }
}
