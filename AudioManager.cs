namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Audio;

    class AudioManager
    {
        private static AudioManager instance;
        public static AudioManager Instance {
            get {
                if (instance == null)
                    instance = new AudioManager();
                return instance;
            }
        }

        private List<string> soundEffectsToLoad = new List<string>();
        private Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();

        private List<SoundEffectInstance> playingSoundEffects = new List<SoundEffectInstance>();
        private Dictionary<string, SoundEffectInstance> playingSingletonSoundEffects = new Dictionary<string, SoundEffectInstance>();

        public AudioManager() {
            soundEffectsToLoad.Add("game_start");
            soundEffectsToLoad.Add("munch_1");
            soundEffectsToLoad.Add("munch_2");
            soundEffectsToLoad.Add("siren_1");
            soundEffectsToLoad.Add("siren_2");
            soundEffectsToLoad.Add("siren_3");
            soundEffectsToLoad.Add("siren_4");
            soundEffectsToLoad.Add("siren_5");
            soundEffectsToLoad.Add("eat_fruit");
            soundEffectsToLoad.Add("power_pellet");
            soundEffectsToLoad.Add("eat_ghost");
            soundEffectsToLoad.Add("retreating");
            soundEffectsToLoad.Add("death_1");
            soundEffectsToLoad.Add("death_2");
            soundEffectsToLoad.Add("extend");
            soundEffectsToLoad.Add("intermission");
            soundEffectsToLoad.Add("credit");
        }

        public void LoadContent(ContentManager content) {
            foreach (string name in soundEffectsToLoad) {
                soundEffects.Add(name, content.Load<SoundEffect>("Sounds/" + name));
            }
        }

        public void PlaySoundEffect(string name) {
            SoundEffectInstance effect = soundEffects[name].CreateInstance();
            effect.Play();
            playingSoundEffects.Add(effect);
        }

        public void PlaySingletonSoundEffect(string name) {
            if (!playingSingletonSoundEffects.ContainsKey(name))
                playingSingletonSoundEffects.Add(name, soundEffects[name].CreateInstance());
            if (playingSingletonSoundEffects[name].State != SoundState.Playing)
                playingSingletonSoundEffects[name].Play();
        }

        public void PauseSingletonSoundEffect(string name) {
            playingSingletonSoundEffects[name].Pause();
        }

        public void ResumeSingletonSoundEffect(string name) {
            playingSingletonSoundEffects[name].Play();  
        }

        public void RemoveSingletonSoundEffect(string name) {
            if (playingSingletonSoundEffects.ContainsKey(name)) {
                playingSingletonSoundEffects[name].Stop();
                playingSingletonSoundEffects.Remove(name);
            }
        }

        public void RemoveAllSoundEffect() {
            foreach (var effect in playingSoundEffects) {
                effect.Stop();
            }
            playingSoundEffects.Clear();
        }

        public void RemoveAllSingletonSoundEffect() {
            foreach (var effect in playingSingletonSoundEffects) {
                effect.Value.Stop();
                playingSingletonSoundEffects.Remove(effect.Key);
            }
        }
    }
}
