using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagement : MonoBehaviour
{
    [SerializeField] private AudioSource m_soundMenu;
    [SerializeField] private AudioSource m_soundClick;
    [SerializeField] private AudioSource m_soundOpenPanel;
    [SerializeField] private AudioSource m_soundTank;
    [SerializeField] private AudioClip[] m_tankAudio;
    private static SoundManagement s_instance;
    public static SoundManagement Instance {
        get {
            return s_instance;
        }
    }
    private void Awake() {
        if (s_instance != null && s_instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }
        s_instance = this;
        DontDestroyOnLoad(this);
    }
    public void PlaySoundClick() {
        m_soundClick.Play();
    }
    public void PlaySoundOpenPanel() {
        m_soundOpenPanel.Play();
    }
    private int m_indexTankAudioClip = -1;
    public void PlaySoundTank(int index) {
        // 0: idle; 1: run
        if (m_indexTankAudioClip != index) {
            m_indexTankAudioClip = index;
            if (m_soundTank.isPlaying) m_soundTank.Stop();
            m_soundTank.clip = m_tankAudio[index];
            m_soundTank.Play();
        }
        
    }
    public void PlaySoundBackground() {
        StopSoundTank();
        m_soundMenu.Play();
    }
    public void StopSoundBackground() {
        m_soundMenu.Stop();
    }
    public void StopSoundTank() {
        m_soundTank.Stop();
    }
}
