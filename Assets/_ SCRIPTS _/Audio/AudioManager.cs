using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Audio
{
    [DisallowMultipleComponent]
    public class AudioManager : MonoBehaviour
    {
        private static List<AudioSource> sources = new List<AudioSource>();

        private static Transform __audioSourceContainer;
        private static Transform audioSourceContainer
        {
            get
            {
                if( __audioSourceContainer == null )
                {
                    __audioSourceContainer = Object.FindObjectOfType<AudioManager>().audioClipContainer;
                }
                return __audioSourceContainer;
            }
        }


        [SerializeField] private Transform audioClipContainer;


        private static void SetClipAndPlay( AudioSource source, TimerHandler timerHandler, AudioClip clip, float volume, float pitch, Vector3 position, Transform target )
        {
            // Sets the source's clip, volume, and pitch.
            // Also sets the timer's duration to the clip's length, as it should never be different.

            source.GetComponent<Follower>().Target = target;
            source.transform.position = position;

            source.volume = volume;
            source.pitch = pitch;
            source.clip = clip;

            timerHandler.duration = clip.length;

            source.Play();
        }

        private static AudioSource CreateSourceAndPlay( AudioClip clip, float volume, float pitch, Vector3 position, Transform target )
        {
            if( audioSourceContainer == null )
            {
                throw new System.Exception( "Can't play sound. There are no AudioManagers added to any GameObject." );
            }

            // Create a new source GameObject to hold the new AudioSource.
            GameObject gameObject = new GameObject( "AudioSource" );
            gameObject.transform.SetParent( audioSourceContainer );
            gameObject.transform.position = position;

            // Add the necessary components.
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = 1.5f;
            audioSource.maxDistance = 15.0f;
            TimerHandler timerHandler = gameObject.AddComponent<TimerHandler>();
            Follower follower = gameObject.AddComponent<Follower>();

            // Setup the timer.
            timerHandler.onTimerEnd.AddListener( () =>
            {
                audioSource.Stop();
                follower.Target = null;
            } );

            // Setup the clip, volume, and pitch.
            SetClipAndPlay( audioSource, timerHandler, clip, volume, pitch, position, target );
            return audioSource;
        }

        private static void PlaySound( AudioClip clip, Vector3 position, Transform target, float volume = 1.0f, float pitch = 1.0f )
        {
            foreach( AudioSource audioSource in sources )
            {
                if( audioSource == null )
                {
                    throw new System.Exception( "Null audio source found." );
                }
                // If the source is currently playing a sound, don't interrupt that, skip it.
                if( audioSource.isPlaying )
                {
                    continue;
                }

                SetClipAndPlay( audioSource, audioSource.GetComponent<TimerHandler>(), clip, volume, pitch, position, target );
                return;
            }

            // If no source GameObject can be reused (every single one is playing at the moment):
            AudioSource newAudioSource = CreateSourceAndPlay( clip, volume, pitch, position, target );

            sources.Add( newAudioSource );
        }

        /// <summary>
        /// Plays a new sound. Can specify the sound, volume and pitch.
        /// </summary>
        public static void PlaySound( AudioClip clip, Vector3 position, float volume = 1.0f, float pitch = 1.0f )
        {
            PlaySound( clip, position, null, volume, pitch );
        }
        
        /// <summary>
        /// Plays a new sound to follow an object. Can specify the sound, volume and pitch.
        /// </summary>
        public static void PlaySound( AudioClip clip, Transform target, float volume = 1.0f, float pitch = 1.0f )
        {
            PlaySound( clip, target.position, target, volume, pitch );
        }

        /// <summary>
        /// Stops all sounds of the specified clip. Null to stop every sound.
        /// </summary>
        public static void StopSounds( AudioClip matchClip = null )
        {
            foreach( AudioSource audioSource in sources )
            {
                if( audioSource == null )
                {
                    throw new System.Exception( "Null audio source found." );
                }

                if( audioSource.isPlaying )
                {
                    if( matchClip == null || audioSource.clip == matchClip )
                    {
                        audioSource.Stop();
                        audioSource.GetComponent<Follower>().Target = null;
                    }
                }
            }
        }
    }
}