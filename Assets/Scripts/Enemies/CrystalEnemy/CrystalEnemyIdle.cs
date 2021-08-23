using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Freya;

public class CrystalEnemyIdle : StateMachineBehaviour {
    private BaseEnemy enemy;
    private MultiAimConstraint multiAimConstraint;
    private Transform multiAimConstraintTarget;

    //private int animLayerIndex;

    public float elapsedVisionDuration;
    public float alertVisionDuration;

    [Tooltip("The duration that must elapse before the head aims 100% at the player")]
    public float aimConstraintWeightDuration;
    public float weight;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        enemy = animator.GetComponentInParent<BaseEnemy>();
        multiAimConstraint = animator.GetComponentInChildren<MultiAimConstraint>();
        //animLayerIndex = animator.GetLayerIndex("Head Rotation");

        multiAimConstraint.weight = 0f;
        multiAimConstraintTarget = multiAimConstraint.data.sourceObjects.GetTransform(0);

        elapsedVisionDuration = 0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy.vision.isPlayerInSight()) {
            elapsedVisionDuration += Time.deltaTime;
            multiAimConstraintTarget.position = Player.Instance.cam.transform.position;

            if (elapsedVisionDuration >= alertVisionDuration) {
                // Transition to attack state
                animator.SetBool("isTrackingPlayer", true);
            }
        } else {
            elapsedVisionDuration = Mathf.Clamp01(elapsedVisionDuration - Time.deltaTime / 2);
        }

        // Lerp the weight of the multi-aim constraint depending on how long the 
        //multiAimConstraint.weight = Mathfs.RemapClamped(0f, aimConstraintWeightDuration, 0f, 1f, elapsedVisionDuration);
        //animator.SetLayerWeight(animLayerIndex, Mathfs.RemapClamped(aimConstraintWeightDuration, 0f, 0f, 1f, elapsedVisionDuration));
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
