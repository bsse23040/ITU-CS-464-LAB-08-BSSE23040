using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// One volume row in the settings: a slider from 0 to 1 for one mixer group, with the value shown
// both as a percentage and as the decibels the mixer really gets.
public class VolumeSlider : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] VolumeSettings.Channel channel;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text valueText;

    public Slider Slider => slider;
    public VolumeSettings.Channel Channel => channel;

    void Awake() { slider.onValueChanged.AddListener(OnMoved); }

    // put the slider on the saved value without counting it as a change
    public void Load()
    {
        slider.SetValueWithoutNotify(VolumeSettings.Get(channel));
        Label(slider.value);
    }

    void OnMoved(float value)
    {
        VolumeSettings.Set(mixer, channel, value);
        Label(value);

        // music is already playing while you drag. The other two need a sound to judge them by
        if (channel == VolumeSettings.Channel.Sfx && SfxPlayer.Instance != null) SfxPlayer.Instance.Preview();
        if (channel == VolumeSettings.Channel.Ui && UIAudio.Instance != null) UIAudio.Instance.Hover();
    }

    void Label(float value)
    {
        if (value <= VolumeSettings.SilentBelow) valueText.text = "Muted";
        else valueText.text = Mathf.RoundToInt(value * 100f) + "%   " + VolumeSettings.ToDecibels(value).ToString("0.#") + " dB";
    }
}
