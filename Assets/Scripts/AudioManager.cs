using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Audio {
    public string name;
    public AudioClip clip;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour {

    private static AudioManager _instance;
    public static AudioManager instance {
        private set {
            _instance = value;
        }
        get {
            if (_instance == null) {
                Debug.LogWarning("No active AudioManager!");
            }
            return _instance;
        }
    }

    [SerializeField]
    private Audio[] audios;

    void OnValidate() {
        foreach (Audio audio in audios) {
            if (string.IsNullOrEmpty(audio.name)) {
                audio.name = audio.clip.name;
            }
        }
    }

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Debug.LogWarning("Multiple AudioManagers! Destroying an additional manager...");
            Destroy(gameObject);
        }

        foreach (Audio audio in audios) {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            audio.source = source;
            source.clip = audio.clip;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Play(string name) {
        foreach (var audio in audios) {
            if (audio.name == name) {
                audio.source.Play();
                return;
            }
        }
    }

    public void Play(AudioClip clip) {
        foreach (var audio in audios) {
            if (audio.clip == clip) {
                audio.source.Play();
                return;
            }
        }
    }
}
