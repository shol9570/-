using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class CParticleCollisionSound : MonoBehaviour
{
    public AudioClip[] m_Clips;
    public AudioMixerGroup m_MixerGroup;

    private AudioSource m_AudioSource;

    void Start()
    {
        GameObject soundObj = new GameObject();
        soundObj.transform.SetParent(this.transform);
        m_AudioSource = soundObj.AddComponent(typeof(AudioSource)) as AudioSource;
        m_AudioSource.rolloffMode = AudioRolloffMode.Custom;
        m_AudioSource.spatialBlend = 1f;
        m_AudioSource.minDistance = 0f;
        m_AudioSource.maxDistance = 2f;
        soundObj.hideFlags = HideFlags.HideInHierarchy;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (m_AudioSource.isPlaying) return;
        m_AudioSource.transform.position = other.transform.position;
        m_AudioSource.clip = m_Clips[Random.Range(0, m_Clips.Length)];
        if (m_MixerGroup != null) m_AudioSource.outputAudioMixerGroup = m_MixerGroup;
        m_AudioSource.Play();
    }
}