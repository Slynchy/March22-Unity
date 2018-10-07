using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M22
{
    public class SFXScript : MonoBehaviour
    {

        AudioSource src;
        float volume = 1.0f;
        float speed = 1.0f;
        bool fadingIn = false;
        bool fadingOut = false;

        private IEnumerator DestroyOnComplete()
        {
            yield return new WaitForSeconds(src.clip.length);
            Destroy(this.gameObject);
        }
        private IEnumerator FadeIn()
        {
            while (src.volume < volume)
            {
                src.volume += Time.deltaTime * speed;
                yield return null;
            }
            src.volume = volume;
        }
        private IEnumerator FadeOut()
        {
            while (src.volume > 0)
            {
                src.volume -= Time.deltaTime * speed;
                yield return null;
            }
            Destroy(this.gameObject);
        }

        void Awake()
        {
            src = this.GetComponent<AudioSource>();
        }

        public void InitLooped(AudioClip _clip)
        {
            if (fadingIn == false)
            {
                fadingIn = true;
                this.gameObject.name = _clip.name;
                src.clip = _clip;
                src.volume = 0;
                StartCoroutine(FadeIn());
                src.Play();
            }
        }

        public void Init(AudioClip _clip, string _volume, string _speed, bool _looping = false)
        {
            volume = float.Parse(_volume);
            speed = float.Parse(_speed);
            if (_looping == true)
                InitLooped(_clip);
            else
            {
                src.clip = _clip;
                src.Play();
                StartCoroutine(DestroyOnComplete());
            }
        }

        public void Stop(string _speed = "1.0")
        {
            speed = float.Parse(_speed);
            if(fadingOut == false)
            {
                fadingOut = true;
                StartCoroutine(FadeOut());
            }
        }
    }
}