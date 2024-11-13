using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// music enum tied to sound effect audio.
/// </summary>
public enum MusicAudio
{
    SlotMachineGame,
}

/// <summary>
/// SFX enum tied to sound effect audio.
/// </summary>
public enum SFXAudio
{
    SlimeInReel,
    ButtonPress,
    NotOver50,
    Over50,
    WinMoney,
    SlimeExplode,
}

[Serializable]
public class AudioTypeClipPair<TAudioType> where TAudioType : Enum
{
    [SerializeField] public TAudioType audioTypeSound;
    [SerializeField] public AudioClip audioClip;
}

[CreateAssetMenu(fileName = "AudioData", menuName = "ScriptableObjects/AudioData", order = 1)]
public class AudioData : ScriptableObject
{
    [SerializeField] public List<AudioTypeClipPair<MusicAudio>> audioMusicClipPairs;
    [SerializeField] public List<AudioTypeClipPair<SFXAudio>> audioSFXClipPairs;


    // invoked when AudioData Inspector Changes.
    public event UnityAction<AudioData> AudioDataInspectorChanged;

    /// <summary>
    /// when the inspector is changed this is called.
    /// </summary>
    private void OnValidate()
    {
        AudioDataInspectorChanged?.Invoke(this);
    }
}
