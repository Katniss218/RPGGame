using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPGGame.Animation
{
    /// <summary>
    /// A simple Animator that can play an arbitrary simple (single-state) animation.
    /// </summary>
    [RequireComponent( typeof( Animator ) )]
    public class GenericAnimator : MonoBehaviour
    {
        const string STATE_NAME = "state";
        const string STATE_NAME_BACKWARDS = "state_backwards";
        const string LAYER_NAME = "layer";

        /// <summary>
        /// Holds the different instantiated AnimatorController state machines for each animation clip.
        /// </summary>
        static Dictionary<AnimationClip, RuntimeAnimatorController> animatorControllers = new Dictionary<AnimationClip, RuntimeAnimatorController>();

        /// <summary>
        /// The clip to assign on start.
        /// </summary>
        [SerializeField] private AnimationClip animationClip;

        /// <summary>
        /// The animation clip that's currently assigned to this animator.
        /// </summary>
        public AnimationClip CurrentClip { get; private set; } = null;

        /// <summary>
        /// The playback speed of the animation from before it was paused. Only valid when paused.
        /// </summary>
        public float PausedPreviousSpeed { get; private set; } = 0.0f;

        /// <summary>
        /// The current normalized time of the animation. Only valid when paused.
        /// </summary>
        public float PausedNormalizedTime { get; private set; } = 0.0f;

        Animator animator;
        bool isPlayingBackwards;

        private static RuntimeAnimatorController GetAnimatorController( AnimationClip clip )
        {
            // For an clip that has already a state machine created - return that state machine.
            if( animatorControllers.TryGetValue( clip, out RuntimeAnimatorController animatorController ) )
            {
                return animatorController;
            }

#warning TODO - this doesn't work in build. - UnityEditor
            // Unity sucks ass, I'll probably use Animation conponent, even though it's "obsolete", because there is no other way.

#if UNITY_EDITOR
            // For a new clip, make a new state machine and save it for later reuse.
            animatorController = new UnityEditor.Animations.AnimatorController()
            {
                layers = new UnityEditor.Animations.AnimatorControllerLayer[1]
                {
                    new UnityEditor.Animations.AnimatorControllerLayer()
                    {
                        name = LAYER_NAME,
                        stateMachine = new UnityEditor.Animations.AnimatorStateMachine()
                        {
                            states = new UnityEditor.Animations.ChildAnimatorState[]
                            {
                                // 2 states, because you can't set the 'animator.speed' to be negative. It has to be done in the state.
                                new UnityEditor.Animations.ChildAnimatorState()
                                {
                                    state = new UnityEditor.Animations.AnimatorState()
                                    {
                                        name = STATE_NAME,
                                        motion = clip,
                                        speed = 1.0f
                                    }
                                },
                                new UnityEditor.Animations.ChildAnimatorState()
                                {
                                    state = new UnityEditor.Animations.AnimatorState()
                                    {
                                        name = STATE_NAME_BACKWARDS,
                                        motion = clip,
                                        speed = -1.0f
                                    }
                                }
                            }
                        }
                    }
                }
            };
#endif

            animatorControllers.Add( clip, animatorController );

            return animatorController;
        }

        /// <summary>
        /// Converts between the time in the state machine and the time on the <see cref="AnimationClip"/>.
        /// </summary>
        private static float GetReversedNormalizedTime( float forwardNormalizedTime )
        {
            // -1 => 2
            // -2 => 3
            // 2 => -1
            // 3 => -2
            return (-forwardNormalizedTime + 1);
        }

        void Start()
        {
            animator = GetComponent<Animator>();

            AssignAnimation( animationClip );
        }

        private void AssignAnimation( AnimationClip clip )
        {
            CurrentClip = clip;

            // in build, this is just 'RuntimeAnimatorController'.
            Type type = animator.runtimeAnimatorController.GetType();
            Main.Instance.text.text += type + "\n";
            foreach( var prop in type.GetProperties() )
            {
                Main.Instance.text.text += " -p " + prop.Name + "\n";
            }
            foreach( var prop in type.GetFields() )
            {
                Main.Instance.text.text += " -f " + prop.Name + "\n";
            }

            animator.runtimeAnimatorController = GetAnimatorController( clip );
            PauseAnimation();
        }

        /// <summary>
        /// Plays a specified animation clip.
        /// </summary>
        /// <param name="clip">The animation clip to play.</param>
        /// <param name="normalizedTime">Where on the animation clip to start playing (0-start, 1-end). Supports values out of the [0-1] range.</param>
        /// <param name="speed">The playback speed (1 - forward, -1 - backwards, interpolated)</param>
        public void PlayAnimation( AnimationClip clip, float normalizedTime, float speed )
        {
            AssignAnimation( clip );

            PlayAnimation( normalizedTime, speed );
        }

        /// <summary>
        /// Plays the already-assigned animation clip.
        /// </summary>
        /// <param name="normalizedTime">Where on the animation clip to start playing (0-start, 1-end). Supports values out of the [0-1] range.</param>
        /// <param name="speed">The playback speed (1 - forward, -1 - backwards, interpolated)</param>
        public void PlayAnimation( float normalizedTime, float speed )
        {
            if( speed == 0 )
            {
                throw new Exception( "Use PauseAnimation() instead." );
            }

            bool isBackwards = false;
            if( speed < 0 )
            {
                isBackwards = true;
                speed = -speed;
            }

            if( isBackwards )
            {
                // Fuckery with normalizedTime to play the time on the animation clip.
                animator.Play( STATE_NAME_BACKWARDS, 0, GetReversedNormalizedTime( normalizedTime ) );
            }
            else
            {
                animator.Play( STATE_NAME, 0, normalizedTime );
            }

            isPlayingBackwards = isBackwards;
            animator.speed = speed;
        }

        /// <summary>
        /// Pauses the animation.
        /// </summary>
        public void PauseAnimation()
        {
            AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo( 0 );

            PausedPreviousSpeed = isPlayingBackwards ? -animator.speed : animator.speed;
            // Fuckery with normalizedTime to save the time on the animation clip instead of the time in the state machine
            PausedNormalizedTime = isPlayingBackwards ? GetReversedNormalizedTime( animationState.normalizedTime ) : animationState.normalizedTime;

            animator.speed = 0.0f; // hopefully this doesn't create performance issues with animation still actually playing but at 0 speed...
        }

        /// <summary>
        /// Resumes the animation with the same parameters as before it was paused.
        /// </summary>
        public void ResumeAnimation()
        {
            animator.speed = isPlayingBackwards ? -PausedPreviousSpeed : PausedPreviousSpeed;
            PausedPreviousSpeed = 0.0f;
        }
    }
}