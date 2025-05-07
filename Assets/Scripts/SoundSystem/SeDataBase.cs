using UnityEngine;
using System.Collections.Generic;

namespace SoundSystem
{
    [CreateAssetMenu(menuName = "Sound/SEDatabase")]
    public class SeDataBase : ScriptableObject
    {
        [SerializeField]
        private List<SeData> seDatas;

        public SeData GetSe(string identifier)
        {
            return seDatas.Find(data => data.seTitle == identifier);
        }

        public SeData GetSe(int index)
        {
            return seDatas[index];
        }
    }

    [System.Serializable]
    public class SeData
    {
        public string seTitle;
        public AudioClip audioClip;

        [Range(0, 1)]
        public float volume;

        public SeData(string seTitle, AudioClip audioClip, float volume)
        {
            if (seTitle == null)
            {
                throw new System.ArgumentException("seTitle must not be null");
            }
            this.seTitle = seTitle;

            if (audioClip == null)
            {
                throw new System.ArgumentException("audioClip must not be null");
            }

            this.audioClip = audioClip;
            this.volume = volume;
        }
    }
}
