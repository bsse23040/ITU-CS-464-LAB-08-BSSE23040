using UnityEngine;
using UnityEngine.Audio;

// The four volumes (Master, Music, SFX, UI): saved in PlayerPrefs and put on the Audio Mixer in decibels.
// A slider goes from 0 to 1, but loudness is logarithmic, so the mixer gets 20 * log10(slider).
// 1 is 0 dB, 0.5 is about -6 dB, 0.1 is -20 dB. The log of 0 does not exist, so anything at or below
// 0.0001 is treated as muted and set to -80 dB, the mixer's silence.
public static class VolumeSettings
{
    public enum Channel { Master, Music, Sfx, Ui }

    public const float SilentBelow = 0.0001f;
    public const float SilentDecibels = -80f;

    // the names of the exposed parameters in MainMixer
    static readonly string[] Parameters = { "MasterVolume", "MusicVolume", "SfxVolume", "UiVolume" };
    static readonly string[] Keys = { "volume_master", "volume_music", "volume_sfx", "volume_ui" };
    static readonly float[] Defaults = { 1f, 0.8f, 1f, 0.8f };

    public static string Parameter(Channel c) { return Parameters[(int)c]; }

    public static float Get(Channel c)
    {
        return PlayerPrefs.GetFloat(Keys[(int)c], Defaults[(int)c]);
    }

    public static float ToDecibels(float slider)
    {
        if (slider <= SilentBelow) return SilentDecibels;
        return Mathf.Log10(Mathf.Clamp01(slider)) * 20f;
    }

    // save the value and put it on the mixer straight away
    public static void Set(AudioMixer mixer, Channel c, float slider)
    {
        PlayerPrefs.SetFloat(Keys[(int)c], Mathf.Clamp01(slider));
        mixer.SetFloat(Parameters[(int)c], ToDecibels(slider));
    }

    // when the game starts: the mixer comes up at 0 dB, so the saved volumes have to be put back
    public static void ApplyAll(AudioMixer mixer)
    {
        for (int i = 0; i < Parameters.Length; i++)
            mixer.SetFloat(Parameters[i], ToDecibels(Get((Channel)i)));
    }

    public static void Save() { PlayerPrefs.Save(); }
}
