using SoundSystem;
using UnityEngine;
using UnityEngine.UI;

public class VolumeAdjuster : MonoBehaviour
{
    [SerializeField]
    private Slider bgmSlider;

    [SerializeField]
    private Slider seSlider;

    private SoundPlayer soundPlayer;


    private void Start()
    {
        soundPlayer = SoundPlayer.instance;
        bgmSlider.value = soundPlayer.BgmAudioSource.volume;
        seSlider.value = soundPlayer.SeAudioSource.volume;

        bgmSlider.onValueChanged.AddListener((x) => soundPlayer.BgmVolumeAdjust(x));
        seSlider.onValueChanged.AddListener((x) => soundPlayer.SeVolumeAdjust(x));
    }

    private void OnDisable()
    {
        bgmSlider.onValueChanged.RemoveAllListeners();
        seSlider.onValueChanged.RemoveAllListeners();
    }
}
