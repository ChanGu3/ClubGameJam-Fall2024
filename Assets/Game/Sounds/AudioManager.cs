using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // singleton sceneloader
    private static AudioManager audioManager = null;
    private static AudioData audioData = null;
    private static AudioSource audioSourceMusic;
    private static AudioSource audioSourceSFX;

    /// <summary>
    /// called when script is loaded into memory
    /// </summary>
    private void Awake()
    {
        this.SingletonAudioManager();
    }

    /// <summary>
    /// assigns the first existing sceneloader to static sceneloader.
    ///     otherwise deletes any other sceneloader when loading into other scenes.
    /// </summary>
    private void SingletonAudioManager()
    {
        if (audioManager == null)
        {
            audioData = Resources.Load<AudioData>("Audio/AudioData");
            audioData.AudioDataInspectorChanged += OnAudioDataInspectorChanged;
            audioManager = this;
            audioSourceMusic = this.gameObject.AddComponent<AudioSource>();
            audioSourceMusic.volume = 0.35f;
            audioSourceSFX = this.gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// stops music.
    /// </summary>
    public static void StopMusic()
    {
        if (audioSourceMusic.isPlaying)
        {
            audioSourceMusic.Stop();
        }
    }

    /// <summary>
    /// plays  music by MusicAudio enum.
    /// </summary>
    /// <param name="audioMusic"> background audio enum name. </param>
    public static void PlayMusic(MusicAudio audioMusic)
    {
        bool isClipFound = false;
        for (int index = 0; index < audioData.audioMusicClipPairs.Count && !isClipFound; index++)
        {
            if (audioMusic == audioData.audioMusicClipPairs[index].audioTypeSound)
            {
                // whenever current audioclip is the same wanting to be played dont do anything
                if (audioSourceMusic.clip != audioData.audioMusicClipPairs[index].audioClip)
                {
                    // if previous audio is being played stop
                    if (audioSourceMusic.isPlaying)
                    {
                        audioSourceMusic.Stop();
                    }

                    audioSourceMusic.clip = audioData.audioMusicClipPairs[index].audioClip;
                    audioSourceMusic.loop = true;
                    audioSourceMusic.Play();
                }
            }
        }
    }

    /// <summary>
    /// Plays a oneshot of a soundeffect.
    /// </summary>
    /// <param name="audioSFX"></param>
    public static void PlaySFX(SFXAudio audioSFX)
    {
        bool isClipFound = false;
        for (int index = 0; index < audioData.audioSFXClipPairs.Count && !isClipFound; index++)
        {
            if (audioSFX == audioData.audioSFXClipPairs[index].audioTypeSound)
            {
                audioSourceSFX.PlayOneShot(audioData.audioSFXClipPairs[index].audioClip);
            }
        }
    }

    /// <summary>
    ///     when changing the AudioData inspector update the static audio.
    /// </summary>
    /// <param name="sender"> audiodata inspector changed. </param>
    /// <param name="eventArg"> null. </param>
    private static void OnAudioDataInspectorChanged(AudioData sender)
    {
        audioData = sender;
    }
}
