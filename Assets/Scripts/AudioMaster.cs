﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M22
{

    public class AudioMaster : MonoBehaviour
    {
        static Dictionary<string, AudioClip> loadedAudio = new Dictionary<string, AudioClip>();

        static AudioSource musicSrc;

        // Use this for initialization
        void Start()
        {
            musicSrc = this.GetComponent<AudioSource>();
            if (musicSrc == null)
                Debug.Log("AudioSource for music not attached to camera!");
        }

        // Update is called once per frame
        void Update()
        {
        }

        static public bool PlaySting(string name)
        {
            AudioClip sfx;
            loadedAudio.TryGetValue(name, out sfx);
            if (sfx == null)
                return false;
            musicSrc.PlayOneShot(sfx);
            return true;
        }

        static public bool LoadMusic(string name)
        {
            string filename = "Music/" + name;
            AudioClip temp = Resources.Load(filename) as AudioClip;
            if (temp != null)
            {
                loadedAudio.Add(name, temp);
                return true;
            }
            else
            {
                Debug.LogError("Failed to load audio: " + filename);
                return false;
            }
        }

        static public bool LoadSting(string name)
        {
            string filename = "SFX/" + name;
            AudioClip temp = Resources.Load(filename) as AudioClip;
            if (temp != null)
            {
                loadedAudio.Add(name, temp);
                return true;
            }
            else
            {
                Debug.LogError("Failed to load audio: " + filename);
                return false;
            }
        }

        static public void ChangeTrack(AudioClip _track)
        {
            musicSrc.Stop();
            musicSrc.time = 0;
            musicSrc.clip = _track;
            musicSrc.Play();
        }

        static public void ChangeTrack(string _track)
        {
            musicSrc.Stop();
            musicSrc.time = 0;
            AudioClip track;
            loadedAudio.TryGetValue(_track, out track);
            musicSrc.clip = track;
            musicSrc.Play();
        }

        static public void StopMusic()
        {
            musicSrc.Stop();
            musicSrc.time = 0;
        }
        static public void StartMusic()
        {
            musicSrc.Play();
        }
    }

}