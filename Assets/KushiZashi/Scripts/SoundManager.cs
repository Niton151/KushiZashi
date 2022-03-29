using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    public AudioClip clock;
    public AudioClip gusa;
    public AudioClip ju;
    public AudioClip match;
    public AudioClip wrong;
    public AudioClip money;
    public AudioClip timeup;
    public AudioClip delete;
    public AudioClip select;
    public AudioClip upgrade;
    public AudioClip unlock;
    public AudioClip timeQuick;

    public AudioSource Audio { get; set; }

    public void FirstInit()
    {
        Audio = GetComponent<AudioSource>();
    }
}
