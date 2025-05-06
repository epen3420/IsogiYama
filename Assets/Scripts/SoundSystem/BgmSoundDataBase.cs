using UnityEngine;
using System.Collections.Generic;

namespace SoundSystem
{
    [CreateAssetMenu(menuName = "Sound/BGMDatabase")]
    public class BgmSoundDataBase : ScriptableObject
    {
        [SerializeField]
        private List<BgmSoundData> bgmSoundDatas;

        /// <summary>
        /// BGMを取得します。
        /// </summary>
        /// <param name="identifier">取得したいBGMのタイトル</param>
        /// <returns>identifierが一致するBGMSoundData.もし見つからなければ、nullを返します</returns>
        public BgmSoundData GetBgm(string identifier)
        {
            var ret = bgmSoundDatas.Find(data => data.bgmTitle == identifier);
            if (ret == null)
            {
                Debug.LogError("BGMデータが見つかりませんでした。");
            }
            return ret;
        }

        /// <summary>
        /// BGMを取得します。
        /// </summary>
        /// <param name="index">取得したいBGMのindex</param>
        /// <returns>index番目に格納されたBGMSoundData</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public BgmSoundData GetBgm(int index)
        {
            try
            {
                var ret = bgmSoundDatas[index];
                return ret;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                Debug.LogError(new System.ArgumentOutOfRangeException());
                return null;
            }
        }
    }

    [System.Serializable]
    // BGMのデータを格納するクラス.
    public class BgmSoundData
    {
        public string bgmTitle;
        public AudioClip audioClip;

        [Range(0, 1)]
        public float volume;

        public BgmSoundData(string bgmTitle, AudioClip audioClip, float volume)
        {
            if (bgmTitle == null)
            {
                throw new System.ArgumentNullException("bgmTitle must not be null");
            }
            this.bgmTitle = bgmTitle;
            if (audioClip == null)
            {
                throw new System.ArgumentNullException("audioClip must not be null");
            }
            this.audioClip = audioClip;
            this.volume = volume;
        }
    }
}
