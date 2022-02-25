using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Sound Effect", menuName = "Assets/ScriptableObject/SoundEffect")]
public class SoundEffect_SO : ScriptableObject
{
    [Title("Audio Clips")]
    public List<AudioClip> audioClips;

    [Title("Bools")]
    public bool playRandom;

    public bool mute;
    public bool bypassEffects;
    public bool bypassListenerEffects;
    public bool bypassReverbZones;
    public bool playOnAwake = true;
    public bool loop;

    [Title("Volume & sound modes")]
    [PropertyRange(0, 256)]
    public int priority = 128;
    [PropertyRange(0, 1)]
    public float volume = 1;
    [PropertyRange(-3, 3)]
    public float pitch = 1;
    [PropertyRange(0, 256)]
    public float stereoPan = 0;
    [PropertyRange(0, 1)]
    public float spatialBlend = 0;
    [PropertyRange(0, 1.1)]
    public float reverbZoneMix = 1;

    [Title("3D Settings")]
    [PropertyRange(0, 5)]
    public float dopplerLevel = 1;
    [PropertyRange(0, 360)]
    public float spread = 0;
    public AudioRolloffMode rollOffMode = AudioRolloffMode.Logarithmic;
    public Vector2 minMaxDistance = new Vector2(1, 500);


    public void PlaySFX(AudioSource _audioSource, int _index = 0)
    {
        InitializeSFX(_audioSource);

        if (playRandom)
        {
            int rand = Random.Range(0, audioClips.Count);
            _audioSource.PlayOneShot(audioClips[rand]);
        }
        else
        {
            _audioSource.PlayOneShot(audioClips[_index]);
        }
    }



    /// <summary>
    /// Inirialize the Audiosource with our custom EQ values
    /// </summary>
    /// <param name="_audioSource"></param>
    private void InitializeSFX(AudioSource _audioSource)
    {
        // Set bools
        _audioSource.mute = mute;
        _audioSource.bypassEffects = bypassEffects;
        _audioSource.bypassListenerEffects = bypassListenerEffects;
        _audioSource.bypassReverbZones = bypassReverbZones;
        _audioSource.playOnAwake = playOnAwake;
        _audioSource.loop = loop;

        // Set volume & sound modes
        _audioSource.priority = priority;
        _audioSource.volume = volume;
        _audioSource.pitch = pitch;
        _audioSource.panStereo = stereoPan;
        _audioSource.spatialBlend = spatialBlend;
        _audioSource.reverbZoneMix = reverbZoneMix;

        // Set 3D settings
        _audioSource.dopplerLevel = dopplerLevel;
        _audioSource.spread = spread;
        _audioSource.rolloffMode = rollOffMode;
        _audioSource.minDistance = minMaxDistance.x;
        _audioSource.maxDistance = minMaxDistance.y;
    }
}
