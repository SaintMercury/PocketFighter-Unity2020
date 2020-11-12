using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;
    public static GameManager instance {
        private set {
            if (_instance == null) {
                _instance = value;
            } else {
                Debug.Log("Multiple sets to gamemanager instance called. is there one or more duplicates?");
            }
        }
        get {
            if (_instance == null) {
                Debug.LogWarning("Attempting to get a game manager instance that is not there!");
            }
            return _instance;
        }
    }

    [SerializeField]
    private Texture2D mouseCursorTexture;

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Debug.LogWarning("Multiple GameManagers! Destroying an additional manager...");
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        Cursor.SetCursor(mouseCursorTexture, new Vector2(mouseCursorTexture.width/2, mouseCursorTexture.height/2), CursorMode.Auto);
    }

    public void ToggleCursorVisibility(bool isVisible) {
        Debug.Log(Cursor.visible);
        Cursor.visible = isVisible;
        Debug.Log(Cursor.visible);
    }
}
