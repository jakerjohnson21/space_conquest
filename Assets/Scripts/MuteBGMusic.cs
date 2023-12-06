using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteBGMusic : MonoBehaviour
{

    [SerializeField] private AudioSource backgroundMusic;

    public void Start()
    {
        backgroundMusic = GetComponent<AudioSource>();

    }
    public void MuteGame()
    {
        backgroundMusic.mute = !backgroundMusic.mute;
    }
}
