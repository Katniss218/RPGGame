using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RPGGame.Animation
{
    public class GenericAnimator : MonoBehaviour
    {
        public AnimationClip AnimationClip;

        Animator animator;

        AnimatorOverrideController __animController;

        AnimatorOverrideController AnimController
        {
            get
            {
                if( __animController == null )
                {
                    __animController = new AnimatorOverrideController();
                }
                return __animController;
            }
        }

        /// <summary>
        /// The name of the state. There should be exactly one.
        /// </summary>
        const string STATE_NAME = "state";

        const string LAYER_NAME = "layer";


        void Start()
        {
            //create animation clip
            //  AnimationCurve translateX = AnimationCurve.Linear( 0.0f, 0.0f, 2.0f, 2.0f );

            // AnimationClip animationClip = new AnimationClip();
            // animationClip.SetCurve( "", typeof( Transform ), "localPosition.x", translateX );


            // ---------------------------------
            // Way 1:
            /*

            if( originalStateName == null )
            {
                UnityEditor.Animations.AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

                if( ac == null )
                {
                    throw new Exception( "You need to attach a temporary Animator Controller asset to the Animator component." );
                }

                ac.layers[0].stateMachine.states[0].state.motion = AnimationClip;
            }

            AnimController.runtimeAnimatorController = animator.runtimeAnimatorController;
            animator.runtimeAnimatorController = AnimController;
            */
            // ---------------------------------

            animator = GetComponent<Animator>();

            // This could likely be done with pooling based on the animation clip, to reduce the number of distinct AnimatorControllers.
            // - if so, then the AnimatorController needs to be reassigned when changing the AnimationClip.
            UnityEditor.Animations.AnimatorController ac = new UnityEditor.Animations.AnimatorController();
            ac.layers = new UnityEditor.Animations.AnimatorControllerLayer[1]
            {
                new UnityEditor.Animations.AnimatorControllerLayer()
                {
                    name = LAYER_NAME,
                    stateMachine = new UnityEditor.Animations.AnimatorStateMachine()
                    {
                        states = new UnityEditor.Animations.ChildAnimatorState[1]
                        {
                            new UnityEditor.Animations.ChildAnimatorState()
                            {
                                state = new UnityEditor.Animations.AnimatorState()
                                {
                                    name = STATE_NAME,
                                    motion = AnimationClip
                                }
                            }
                        }
                    }
                }
            };

            animator.runtimeAnimatorController = ac;
            animator.speed = 0.0f;

            // AssignAnimation( AnimationClip );
        }

        /*
        private void AssignAnimation( AnimationClip clip )
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if( clips.Length == 0 )
            {
                throw new Exception( "You haven't configured the animator properly. You need to add a dummy animation clip." );
            }

            string name = clips[0].name;
            AnimController[name] = AnimationClip;
            animator.speed = 0.0f; // hopefully this doesn't create performance issues with animation still actually playing but at 0 speed...
        }
        */

        public void PlayAnimation()
        {
            animator.Play( STATE_NAME, 0, 0 );
            animator.speed = 1.0f;
        }

        public void StopAnimation()
        {
            animator.speed = 0.0f;
        }
    }
}