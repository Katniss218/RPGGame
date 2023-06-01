using RPGGame.Mobs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPGGame.Animation
{
    [RequireComponent( typeof( Animator ) )]
    public class HumanoidAnimator : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private MobTargetTracking mobTargetTracking;
        Animator anim;

        void Awake()
        {
            anim = this.GetComponent<Animator>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if( navMeshAgent == null )
            {
                return;
            }

            if( navMeshAgent.velocity.magnitude > 0.01f )
            {
                anim.SetBool( "IsRunning", true );
            }
            else
            {
                anim.SetBool( "IsRunning", false );
            }

            if( mobTargetTracking.Target != null )
            {
                if( Vector3.Distance( mobTargetTracking.Target.position, this.transform.position ) < 2 )
                {
                    anim.SetInteger( "SwingType", 1 );
                }
                else
                {
                    anim.SetInteger( "SwingType", -1 );
                }
            }
        }
    }
}