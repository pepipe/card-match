using UnityEngine;

namespace CardMatch.Utils
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundClip : MonoBehaviour
    {
        [SerializeField] AudioClip Clip;
        
        AudioSource _audioSource;

        void Awake()
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
        }

        void Start()
        {
            _audioSource.clip = Clip;
        }

        public void PlayOneShot()
        {
            _audioSource.PlayOneShot(Clip);
        }

        public void Play()
        {
            _audioSource.Play();
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public void SetLoop(bool loop)
        {
            _audioSource.loop = loop;
        }

        public bool IsPlaying()
        {
            return _audioSource.isPlaying;
        }
    }
}