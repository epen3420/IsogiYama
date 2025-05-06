using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundSystem
{
    public class SoundPlayer : MonoBehaviour
    {
        public static SoundPlayer instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                AddAudioSource();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private BgmSoundData bgmData; // 最後に再生したのBGMデータ
        private SeData seData; // 最後に再生したのSEデータ
        private VoiceSoundData voiceData; // 最後に再生したのVoiceデータ

        private AudioSource bgmAudioSource;
        public AudioSource BgmAudioSource
        {
            get { return bgmAudioSource; }
        }
        private AudioSource seAudioSource;
        public AudioSource SeAudioSource
        {
            get { return seAudioSource; }
        }
        private AudioSource voiceAudioSource;
        public AudioSource VoiceAudioSource
        {
            get { return voiceAudioSource; }
        }

        [SerializeField]
        [Header("BGMのクリップのデータリスト")]
        private BgmSoundDataBase bgmSoundDatas;

        [SerializeField]
        [Header("SEのクリップのデータリスト")]
        private SeDataBase seDatas;

        [SerializeField]
        [Header("Voiceのクリップのデータリスト")]
        private VoiceSoundDataBase voiceSoundDatas;

        [SerializeField]
        [Header("音声設定")]
        private SoundSetting soundSetting;


        private void AddAudioSource()
        {
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
            seAudioSource = gameObject.AddComponent<AudioSource>();
            voiceAudioSource = gameObject.AddComponent<AudioSource>();
        }

        public void PlayBgm(string bgmTitle)
        {
            bgmData = bgmSoundDatas.GetBgm(bgmTitle);
            if (bgmData == null)
            {
                return;
            }
            bgmAudioSource.clip = bgmData.audioClip;
            VolumeAdjust(bgmAudioSource, soundSetting.BgmVolume, bgmData.volume);
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
        }

        public void PauseBgm()
        {
            if (bgmAudioSource.isPlaying == false)
            {
                Debug.LogWarning("BGM is not playing or already paused");
                return;
            }
            bgmAudioSource.Pause();
            Debug.Log($"BGM paused position: {bgmAudioSource.time:F0}s");
        }

        public void UnPauseBgm()
        {
            if (bgmAudioSource.isPlaying == true)
            {
                Debug.LogWarning("BGM is already playing");
                return;
            }
            Debug.Log($"BGM unpaused position: {bgmAudioSource.time}s");
            bgmAudioSource.UnPause();
        }

        public void StopBgm()
        {
            if (bgmAudioSource.isPlaying == false)
            {
                Debug.LogWarning("BGM is not playing");
                return;
            }
            Debug.Log($"BGM stopped position: {bgmAudioSource.time}s");
            bgmAudioSource.Stop();
            bgmAudioSource.clip = null;
        }

        public void PlaySe(string seTitle)
        {
            SeData seData = seDatas.GetSe(seTitle);
            if (seData == null)
            {
                Debug.LogWarning("SE data not found");
                return;
            }
            seAudioSource.clip = seData.audioClip;
            VolumeAdjust(seAudioSource, soundSetting.SeVolume, seData.volume);
            seAudioSource.PlayOneShot(seAudioSource.clip);
        }

        public void PlayVoice(string voiceTitle)
        {
            VoiceSoundData voiceData = voiceSoundDatas.GetVoice(voiceTitle);
            if (voiceData == null)
            {
                Debug.LogWarning("Voice data not found");
                return;
            }
            voiceAudioSource.clip = voiceData.audioClip;
            VolumeAdjust(voiceAudioSource, soundSetting.VoiceVolume, voiceData.volume);
            voiceAudioSource.PlayOneShot(voiceAudioSource.clip);
        }

        public void MasterVolumeAdjust(float newValue)
        {
            soundSetting.SetMasterVolume(newValue);
            if (bgmData == null)
                return;
            VolumeAdjust(bgmAudioSource, soundSetting.BgmVolume, bgmData.volume);
        }

        public void BgmVolumeAdjust(float newValue)
        {
            soundSetting.SetBgmVolume(newValue);
            if (bgmData == null)
                return;
            VolumeAdjust(bgmAudioSource, soundSetting.BgmVolume, bgmData.volume);
        }

        public void SeVolumeAdjust(float newValue)
        {
            soundSetting.SetSeVolume(newValue);
            if (seData == null)
                return;
            VolumeAdjust(seAudioSource, soundSetting.SeVolume, seData.volume);
        }

        public void VoiceVolumeAdjust(float newValue)
        {
            soundSetting.SetVoiceVolume(newValue);
            if (voiceData == null)
                return;
            VolumeAdjust(voiceAudioSource, soundSetting.VoiceVolume, voiceData.volume);
        }

        private void VolumeAdjust(
            AudioSource audioSource,
            float categoryMasterVolume,
            float dataVolume
        )
        {
            audioSource.volume = soundSetting.MasterVolume * categoryMasterVolume * dataVolume;
        }

        [System.Serializable]
        private class SoundSetting
        {
            [SerializeField]
            [Header("全体音量")]
            [Range(0.0f, 1.0f)]
            float masterVolume = 1.0f;

            [SerializeField]
            [Header("音楽音量")]
            [Range(0.0f, 1.0f)]
            float bgmVolume = 1.0f;

            [SerializeField]
            [Header("効果音量")]
            [Range(0.0f, 1.0f)]
            float seVolume = 1.0f;

            [SerializeField]
            [Header("ボイス音量")]
            [Range(0.0f, 1.0f)]
            float voiceVolume = 1.0f;

            public float MasterVolume
            {
                get { return masterVolume; }
            }

            public float BgmVolume
            {
                get { return bgmVolume; }
            }

            public float SeVolume
            {
                get { return seVolume; }
            }

            public float VoiceVolume
            {
                get { return voiceVolume; }
            }

            public void SetMasterVolume(float volume)
            {
                if (volume < 0 || volume > 1)
                {
                    Debug.LogWarning("MasterVolume must be between 0 and 1");
                }

                masterVolume = Mathf.Clamp(volume, 0, 1);
            }

            public void SetBgmVolume(float volume)
            {
                if (volume < 0 || volume > 1)
                {
                    Debug.LogWarning("BgmVolume must be between 0 and 1");
                }

                bgmVolume = Mathf.Clamp(volume, 0, 1);
            }

            public void SetSeVolume(float volume)
            {
                if (volume < 0 || volume > 1)
                {
                    Debug.LogWarning("SeVolume must be between 0 and 1");
                }

                seVolume = Mathf.Clamp(volume, 0, 1);
            }

            public void SetVoiceVolume(float volume)
            {
                if (volume < 0 || volume > 1)
                {
                    Debug.LogWarning("VoiceVolume must be between 0 and 1");
                }

                voiceVolume = Mathf.Clamp(volume, 0, 1);
            }

            public SoundSetting(
                float masterVolume,
                float bgmVolume,
                float seVolume,
                float voiceVolume
            )
            {
                SetBgmVolume(bgmVolume);
                SetSeVolume(seVolume);
                SetVoiceVolume(voiceVolume);
                SetMasterVolume(masterVolume);
            }
        }
    }
}
