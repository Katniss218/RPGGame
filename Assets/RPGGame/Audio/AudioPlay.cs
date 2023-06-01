using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Audio {
    public class AudioPlay : MonoBehaviour
    {
        public AudioClip clip;

        public void Play()
        {
            AudioManager.PlaySound( clip, this.transform );
        }

        public void Play( AudioClip clip )
        {
            AudioManager.PlaySound( clip, this.transform );
        }
    }
}