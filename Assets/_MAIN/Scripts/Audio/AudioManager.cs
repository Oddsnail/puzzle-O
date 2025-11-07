using UnityEngine;
using UnityEngine.Audio;

namespace origin.audio {
    public class AudioManager : MonoBehaviour {
        public static AudioManager Instance { get; private set; }

        public AudioMixerGroup musicMixer;
        public AudioMixerGroup sfxMixer;
        public AudioMixerGroup voiceMixer;

        private Transform sfxRoot;

        private const string filePathBasic = "SFX";
        private const string SFX_PARENT_NAME = "SFX";
        private const string SFX_NAME_FORMAT = "SFX-[{0}]";

        private void Awake() {
            if (Instance == null) {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else {
                DestroyImmediate(gameObject);
                return;
            }

            sfxRoot = new GameObject(SFX_PARENT_NAME).transform;
            sfxRoot.SetParent(transform);
        }

        public AudioSource PlaySoundEffect(string filePath, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false) {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/{filePath}");

            if (clip == null) {
                Debug.LogError($"Could not find audio file '{filePath}'");
                return null;
            }

            return PlaySoundEffect(clip, mixer, volume, pitch, loop);
        }

        public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false) {
            AudioSource effectSource = new GameObject(string.Format(SFX_NAME_FORMAT, clip.name)).AddComponent<AudioSource>();
            effectSource.transform.SetParent(sfxRoot);
            effectSource.transform.position = sfxRoot.position;

            effectSource.clip = clip;

            if (mixer == null) mixer = sfxMixer;
            effectSource.outputAudioMixerGroup = mixer;
            effectSource.volume = volume;
            effectSource.spatialBlend = 0;
            effectSource.pitch = pitch;
            effectSource.loop = loop;

            effectSource.Play();

            if (!loop) Destroy(effectSource.gameObject, (clip.length / pitch) + 1);

            return effectSource;
        }

        public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);

        public void StopSoundEffect(string soundName) {

            soundName = soundName.ToLower();

            AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
            foreach (var source in sources) {
                if (source.clip.name.ToLower() == soundName) {
                    Destroy(source.gameObject);
                    return;
                }
            }
        }

    }
}

