using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MenuMusicPlayer : MonoBehaviour
{
    private static MenuMusicPlayer instance = null;
    public static MenuMusicPlayer Instance
    {
        get { return instance; }
    }
    private AudioSource player;
    public List<AudioClip> clips;
    public int currentSong;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            player = GetComponent<AudioSource>();
            PlayMusic(0);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void PlayMusic(int song)
    {
        if (player.isPlaying && currentSong == song) return;
        currentSong = song;
        player.clip = clips[currentSong];
        player.Play();
    }
    public void StopMusic()
    {
        currentSong = -1;
        player.Stop();
    }
}