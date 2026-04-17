using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace origin.settings
{
    public class VolumeController : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        private void Start()
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.316f);
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

            SetMusic(musicSlider.value);
            SetSFX(sfxSlider.value);

            musicSlider.onValueChanged.AddListener(SetMusic);
            sfxSlider.onValueChanged.AddListener(SetSFX);
        }

        private void SetMusic(float value)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20f);
            PlayerPrefs.SetFloat("MusicVolume", value);
        }

        private void SetSFX(float value)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);
            PlayerPrefs.SetFloat("SFXVolume", value);
        }
    }
}