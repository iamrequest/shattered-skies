using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LizardAttackState : BaseState {
    private BaseEnemy baseEnemy;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private int animHashIsWalking;
    private int animHashAttack;
    private int animHashWalkSpeed;
    private bool isAttacking;

    public MotorSettings motorSettings;
    public Transform lookatTarget;
    public BaseState giveUpState;
    public float giveUpDelay;
    private float elapsedGiveUpDelay;

    [Range(0f, 1f)]
    public float attackFacingAngle;


    [Tooltip("The delay between entering this state, and chasing the player")]
    public float initialChaseDelay;
    private bool isWaitingForInitialChase;

    [Tooltip("The delay that occurs between reaching the player, attacking, and resuming chase")]
    public float chaseDelay;

    [Header("SFX")]
    public AudioClip onPlayerSpottedSFX;
    [Range(0f, .5f)]
    public float playerSpottedPitchRange;
    public AudioClip onAttackSFX;
    [Range(0f, .5f)]
    public float onAttackPitchRange;

    protected override void Awake() {
        base.Awake();
        baseEnemy = GetComponentInParent<BaseEnemy>();
        navMeshAgent = baseEnemy.GetComponent<NavMeshAgent>();
        animator = baseEnemy.GetComponentInChildren<Animator>();


        animHashIsWalking = Animator.StringToHash("isWalking");
        animHashAttack = Animator.StringToHash("attack");
        animHashWalkSpeed = Animator.StringToHash("walkSpeed");
    }

    public override void OnStateEnter(BaseState previousState) {
        base.OnStateEnter(previousState);

        motorSettings.ApplyMotorSettings(navMeshAgent);
        isAttacking = false;
        isWaitingForInitialChase = true;
        elapsedGiveUpDelay = 0f;

        //animator.ResetTrigger(animHashAttack);
        animator.SetFloat(animHashWalkSpeed, 1f);

        // Play SFX
        float sfxPitch = 1 + Random.Range(-playerSpottedPitchRange, playerSpottedPitchRange);
        SFXPlayer.Instance.PlaySFX(onPlayerSpottedSFX, transform.position, sfxPitch, VolumeManager.Instance.lizardPlayerSpotted);

        StartCoroutine(BeginChaseAfterDelay());
    }

    public override void OnStateExit(BaseState previousState) {
        base.OnStateExit(previousState);

        //animator.ResetTrigger(animHashAttack);
        animator.SetFloat(animHashWalkSpeed, 0f);
    }

    public override void OnStateUpdate() {
        if (baseEnemy.vision.isPlayerInSight()) {
            // Just saw the player. Continue the chase.
            elapsedGiveUpDelay = 0f;
            lookatTarget.position = Player.Instance.cam.transform.position;

            // TODO: Consider setting this every x frames
            if (!isWaitingForInitialChase) {
                navMeshAgent.SetDestination(Player.Instance.playerController.transform.position);
                animator.SetBool(animHashIsWalking, true);
            }
        } else {
            // Haven't seen the player, consider giving up
            elapsedGiveUpDelay += Time.deltaTime;

            if (elapsedGiveUpDelay >= giveUpDelay) {
                parentFSM.TransitionTo(giveUpState);
            }
        }

        // If we're close enough, start attacking
        if (!isAttacking) {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
                StartCoroutine(Attack());

                //if (isFacingPlayer()) {
                //} else {
                //    // TODO : Rotate to face player
                //}
            }
        }
    }

    private IEnumerator BeginChaseAfterDelay() {
        yield return new WaitForSeconds(initialChaseDelay);
        isWaitingForInitialChase = false;
    }
    private IEnumerator Attack() {
        animator.SetBool(animHashIsWalking, false);
        animator.SetTrigger(animHashAttack);

        // Play SFX
        float sfxPitch = 1 + Random.Range(-onAttackPitchRange, onAttackPitchRange);
        SFXPlayer.Instance.PlaySFX(onPlayerSpottedSFX, transform.position, sfxPitch, VolumeManager.Instance.lizardAttack);

        isAttacking = true;
        yield return new WaitForSeconds(chaseDelay);
        isAttacking = false;
    }

    private bool isFacingPlayer() {
        // Test if the lizard is facing the player, projected onto the xy plane
        Vector3 enemyToPlayerDir = (Player.Instance.cam.transform.position - baseEnemy.vision.transform.position).normalized;

        float facingDotProduct = Vector3.Dot(Vector3.ProjectOnPlane(baseEnemy.transform.forward, Vector3.up), 
            Vector3.ProjectOnPlane(enemyToPlayerDir, Vector3.up));

        return facingDotProduct > attackFacingAngle;
    }
}
