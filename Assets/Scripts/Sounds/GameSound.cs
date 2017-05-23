using UnityEngine;

namespace Memory.Sound
{   
    public class GameSound: MonoBehaviour
    {
        private static GameSound instance;
        private static bool mute;
        private AudioSource source;

        [SerializeField]
        private GameSoundGroup[] clips;

        public string[] GetClips()
        {
            string[] cs = new string[clips.Length];
            for (int i = 0; i < cs.Length; i++)
            {
                cs[i] = clips[i].name;
            }

            return cs;
        }

        public static void Play(string clip)
        {
            var g = instance.GetGroup(clip);
            g.Play();
        }
        public static void Play(string clip, float delay)
        {
            var g = instance.GetGroup(clip);
            LeanTween.delayedCall(delay, g.Play);
        }

        public static void Play(AudioClip clip, float vol)
        {
            instance.PlayClip(clip, vol);
        }

        public static void Reset(string clip)
        {
            var g = instance.GetGroup(clip);        
            g.Reset();    
        }

        public static void Mute(bool value)
        {
            mute = value;
        }

        void Awake()
        {
            instance = this;
        }

        private void PlayClip(AudioClip clip, float vol)
        {
            if (mute) return;
            if (source == null) source = gameObject.AddComponent<AudioSource>();
            source.PlayOneShot(clip, vol);
        }

        private AudioClip GetClip(string clip)
        {
            var g = GetGroup(clip);
            if (g != null) return g.clips.GetRandomItem();
            return null;
        }

        private GameSoundGroup GetGroup(string clip)
        {
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == clip)
                {
                    return clips[i];
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class GameSoundGroup
    {
        public string name;
        public bool consistent;
        public float volume = 1;
        public AudioClip[] clips;

        private int index;        

        public void Play()
        {
            if (consistent)
            {
                GameSound.Play(clips[index], volume);
                index++;
                if (index == clips.Length) index--;            
            }
            else
                GameSound.Play(clips.GetRandomItem(), volume);
        }

        public void Reset()
        {
            index = 0;
        }
    }

    public class GameSoundAttribute : PropertyAttribute {}

    public static class GameSoundExtentions
    {
        public static T GetRandomItem<T>(this T[] list)
        {
            if (list == null || list.Length == 0)
                return default(T);
            return list[Random.Range(0, list.Length)];
        }
    }
}