using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace origin.audio {

    [Serializable]
    public struct SFXEntry {
        public string id;
        public AudioClip clip;
    }

    public class AudioManager : MonoBehaviour {
        public static AudioManager instance { get; private set; }

        public AudioMixerGroup musicMixer;
        public AudioMixerGroup sfxMixer;
        public AudioMixerGroup voiceMixer;

        [SerializeField] private SFXEntry[] preloadedSFX;

        private Dictionary<string, AudioClip> _sfxMap;
        private Transform sfxRoot;
        private Transform musicRoot;
        private Transform voiceRoot;

        private const string SFX_NAME_FORMAT = "SFX-[{0}]";

        private void Awake() {
            if (instance == null) {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else {
                DestroyImmediate(gameObject);
                return;
            }

            sfxRoot = new GameObject("SFX").transform;
            sfxRoot.SetParent(transform);
            musicRoot = new GameObject("Music").transform;
            musicRoot.SetParent(transform);
            voiceRoot = new GameObject("Voice").transform;
            voiceRoot.SetParent(transform);

            _sfxMap = new Dictionary<string, AudioClip>();
            foreach (var entry in preloadedSFX) {
                if (entry.clip != null && !string.IsNullOrEmpty(entry.id))
                    _sfxMap[entry.id] = entry.clip;
            }
        }

        public AudioSource PlayPreloadedSFX(string id, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false) {
            if (_sfxMap.TryGetValue(id, out AudioClip clip))
                return PlaySoundEffect(clip, mixer, volume, pitch, loop);

            Debug.LogError($"Preloaded SFX '{id}' not found in sfxMap.");
            return null;
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
            if (mixer == null) mixer = sfxMixer;

            Transform parent = GetRootForMixer(mixer);
            AudioSource effectSource = new GameObject(string.Format(SFX_NAME_FORMAT, clip.name)).AddComponent<AudioSource>();
            effectSource.transform.SetParent(parent);
            effectSource.transform.position = parent.position;

            effectSource.clip = clip;
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

            Transform[] roots = { sfxRoot, musicRoot, voiceRoot };
            foreach (var root in roots) {
                AudioSource[] sources = root.GetComponentsInChildren<AudioSource>();
                foreach (var source in sources) {
                    if (source.clip.name.ToLower() == soundName) {
                        Destroy(source.gameObject);
                        return;
                    }
                }
            }
        }

        private Transform GetRootForMixer(AudioMixerGroup mixer) {
            if (mixer == musicMixer) return musicRoot;
            if (mixer == voiceMixer) return voiceRoot;
            return sfxRoot;
        }

    }
}

