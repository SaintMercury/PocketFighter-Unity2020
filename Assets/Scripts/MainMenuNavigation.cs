using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MainMenuNavigation : MonoBehaviour {

    [SerializeField]
    private GameObject baseMenu;
    private Stack<GameObject> uiItems = new Stack<GameObject>();

    public void JoinGame(GameObject objWithTMPInput) {
        TMP_InputField inputField = objWithTMPInput.GetComponent<TMP_InputField>();
        NetworkManager.singleton.networkAddress = inputField.text;
        AudioManager.instance.Play("Button");
        NetworkManager.singleton.StartClient();
    }

    public void HostGame() {
        AudioManager.instance.Play("Button");
        NetworkManager.singleton.StartHost();
    }

    public void BackToPreviousUI() {
        if (uiItems.Count > 0) {
            uiItems.Pop().SetActive(false);
            if (uiItems.Count > 0) {
                uiItems.Peek().SetActive(true);
            } else {
                baseMenu.SetActive(true);
            }
        }
        AudioManager.instance.Play("Button");
    }

    public void ForwardToNextUI(GameObject uiItem) {
        if (uiItems.Count == 0) {
            baseMenu.SetActive(false);
        } else {
            uiItems.Peek().SetActive(false);
        }
        uiItems.Push(uiItem);
        uiItem.SetActive(true);
        AudioManager.instance.Play("Button");
    }

    public void Quit() {
        Application.Quit(0);
    }
}
