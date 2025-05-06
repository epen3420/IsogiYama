using UnityEngine;
using System.Collections.Generic;

namespace SoundSystem
{
    [CreateAssetMenu(menuName = "Sound/VoiceDatabase")]
    public class VoiceSoundDataBase : ScriptableObject
    {
        [SerializeField]
        private List<VoiceSoundData> voiceSoundDatas;

        /// <summary>
        /// BGMのAudioClipを取得します。
        /// </summary>
        /// <param name="identifier">取得したいBGMのタイトル</param>
        /// <returns></returns>

        public VoiceSoundData GetVoice(string identifier)
        {
            return voiceSoundDatas.Find(data => data.voiceTitle == identifier);
        }

        public VoiceSoundData GetVoice(int index)
        {
            return voiceSoundDatas[index];
        }
    }

    [System.Serializable]
    public class VoiceSoundData
    {
        public string voiceTitle;
        public AudioClip audioClip;

        [Range(0, 1)]
        public float volume;

        public VoiceSoundData(string voiceTitle, AudioClip audioClip, float volume)
        {
            if (voiceTitle == null)
            {
                throw new System.ArgumentException("voiceTitle must not be null");
            }
            this.voiceTitle = voiceTitle;

            if (audioClip == null)
            {
                throw new System.ArgumentException("audioClip must not be null");
            }

            this.audioClip = audioClip;
            this.volume = volume;
        }
    }
}
