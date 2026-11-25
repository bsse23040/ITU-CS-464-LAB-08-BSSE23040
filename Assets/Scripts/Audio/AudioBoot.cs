using UnityEngine;
using UnityEngine.Audio;

// Puts the saved volumes on the mixer when the game starts. It runs in Start because a mixer
// parameter that is set in Awake can be overwritten when the mixer finishes starting up.
public class AudioBoot : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    void Start() { VolumeSettings.ApplyAll(mixer); }

    void OnApplicationPause(bool paused)
    {
        if (paused) VolumeSettings.Save();
    }

    void OnApplicationQuit() { VolumeSettings.Save(); }
}
