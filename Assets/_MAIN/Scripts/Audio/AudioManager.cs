using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace origin.audio {

    [Serializable]
    public struct SFXEntry {
        public string id;
        [Range(0f, 3f)]
        public float volume;
        public AudioClip clip;
    }

    public class AudioManager : MonoBehaviour {
        public static AudioManager instance { get; private set; }

        public AudioMixerGroup musicMixer;
        public AudioMixerGroup sfxMixer;
        public AudioMixerGroup voiceMixer;

        [SerializeField] private SFXEntry[] preloadedSFX;

        private Dictionary<string, (float, AudioClip)> _sfxMap;
        private Transform sfxRoot;
        private Transform musicRoot;
        private Transform voiceRoot;

        private const string SFX_NAME_FORMAT = "SFX-[{0}]";
        private const float GRADIENT_DURATION = 1f;

		private void Awake() {
			if (instance == null) {
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

			_sfxMap = new Dictionary<string, (float, AudioClip)>();
			foreach (var entry in preloadedSFX) {
				if (entry.clip != null && !string.IsNullOrEmpty(entry.id))
					_sfxMap[entry.id] = (entry.volume, entry.clip);
			}
		}
		
		public void PlayButtonSound() => PlayPreloadedSFX("optionChange");
		

        public AudioSource PlayPreloadedSFX(string id, float pitch = 1, bool loop = false) {
            if (_sfxMap.TryGetValue(id, out var sfxData))
                return PlaySound(sfxData.Item2, sfxMixer, sfxData.Item1, pitch, loop);

            Debug.LogError($"Preloaded SFX '{id}' not found in sfxMap.");
            return null;
        }

        public AudioSource PlaySound(string filePath, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false) {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/{filePath}");

            if (clip == null) {
                Debug.LogError($"Could not find audio file '{filePath}'");
                return null;
            }

            return PlaySound(clip, mixer, volume, pitch, loop);
        }

        public AudioSource PlaySound(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false) {
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

        public AudioSource PlaySoundGradient(string filePath, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = true) {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/{filePath}");

            if (clip == null) {
                Debug.LogError($"Could not find audio file '{filePath}'");
                return null;
            }

            return PlaySoundGradient(clip, mixer, volume, pitch, loop);
        }

        public AudioSource PlaySoundGradient(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = true) {
            if (mixer == null) mixer = sfxMixer;

            Transform parent = GetRootForMixer(mixer);
            AudioSource source = new GameObject(string.Format(SFX_NAME_FORMAT, clip.name)).AddComponent<AudioSource>();
            source.transform.SetParent(parent);
            source.transform.position = parent.position;

            source.clip = clip;
            source.outputAudioMixerGroup = mixer;
            source.volume = 0f;
            source.spatialBlend = 0;
            source.pitch = pitch;
            source.loop = loop;

            source.Play();

            if (!loop) Destroy(source.gameObject, (clip.length / pitch) + 1);

            StartCoroutine(FadeVolume(source, 0f, volume));

            return source;
        }

        public void StopSoundGradient(string soundName) {
            soundName = soundName.ToLower();

            Transform[] roots = { sfxRoot, musicRoot, voiceRoot };
            foreach (var root in roots) {
                foreach (var source in root.GetComponentsInChildren<AudioSource>()) {
                    if (source.clip != null && source.clip.name.ToLower() == soundName) {
                        StartCoroutine(FadeOutAndDestroy(source));
                        return;
                    }
                }
            }
        }

        private IEnumerator FadeVolume(AudioSource source, float from, float to) {
            float elapsed = 0f;
            while (elapsed < GRADIENT_DURATION) {
                if (source == null) yield break;
                source.volume = Mathf.Lerp(from, to, elapsed / GRADIENT_DURATION);
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (source != null) source.volume = to;
        }

        private IEnumerator FadeOutAndDestroy(AudioSource source) {
            float startVolume = source.volume;
            float elapsed = 0f;
            while (elapsed < GRADIENT_DURATION) {
                if (source == null) yield break;
                source.volume = Mathf.Lerp(startVolume, 0f, elapsed / GRADIENT_DURATION);
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (source != null) Destroy(source.gameObject);
        }

        public void StopAllBGMGradient() {
            foreach (var source in musicRoot.GetComponentsInChildren<AudioSource>())
                StartCoroutine(FadeOutAndDestroy(source));
        }

        public void StopSound(AudioClip clip) => StopSound(clip.name);

        public void StopSound(string soundName) {
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

