using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M22
{

    namespace Audio
    {

        public class MusicMaster : MonoBehaviour
        {

            AudioSource musicSrc;

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

            public void ChangeTrack(AudioClip _track)
            {
                musicSrc.Stop();
                musicSrc.time = 0;
                musicSrc.clip = _track;
                musicSrc.Play();
            }

            public void StopMusic()
            {
                musicSrc.Stop();
                musicSrc.time = 0;
            }
            public void StartMusic()
            {
                musicSrc.Play();
            }
        }

    }

}