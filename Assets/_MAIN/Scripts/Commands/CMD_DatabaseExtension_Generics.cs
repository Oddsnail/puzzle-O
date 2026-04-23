using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

using origin.dialogue;
using origin.audio;
using origin.graphic;

namespace origin.command {

	public class CMD_DatabaseExtension_Generics : CMD_DatabaseExtension {

		new public static void Extend(CommandDatabase database) {

			// UI Control Relates
			IDialogueUI ui = DialogueManager.instance;
			database.AddCommand("closeDialogue", new Action(ui.Hide));
			database.AddCommand("openDialogue", new Action(ui.Show));
			database.AddCommand("empty", new Action(ui.Empty));

			// Image relates
			database.AddCommand("changeBackground", new Action<string>(ChangeBackground));
			database.AddCommand("changeCutscene", new Action<string>(ChangeCutscene));
			database.AddCommand("bgFogOn", new Action(bgFogOn));
			database.AddCommand("bgFogOff", new Action(bgFogOff));
			database.AddCommand("quitCutscene", new Action(QuitCutscene));

			// Sound relates
			database.AddCommand("play", new Func<string[], IEnumerator>(PlaySoundEffect));
		}

		// Image Relates

		private static void ChangeBackground(string image) => BackgroundManager.instance.ChangeBackground(image);
		private static void ChangeCutscene(string image) => BackgroundManager.instance.ChangeCutscene(image);
		private static void QuitCutscene() => BackgroundManager.instance.QuitCutscene();
		private static void bgFogOn() => BackgroundManager.instance.thinkFogOn(true);
		private static void bgFogOff() => BackgroundManager.instance.thinkFogOn(false);

		// Audio Relates

		private static IEnumerator PlaySoundEffect(string[] data) {
			string filePath = data[0];
			AudioMixerGroup mixer = data[0][..3] switch {
				"sfx" => AudioManager.instance.sfxMixer,
				"bgm" => AudioManager.instance.musicMixer,
				"vce" => AudioManager.instance.voiceMixer,
				_ => null,
			};
			var parameters = ConvertParameters(data);

			parameters.TryGetValue("-v", out float volume, defaultValue: 1.0f);
			parameters.TryGetValue("-p", out float pitch, defaultValue: 1.0f);
			parameters.TryGetValue("-l", out bool loop, defaultValue: false);

			yield return AudioManager.instance.PlaySoundEffect(filePath, mixer, volume, pitch, loop);
		}
	}
}
