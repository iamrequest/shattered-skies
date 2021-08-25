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
    private bool isAttacking;

    public MotorSettings motorSettings;
    public Transform lookatTarget;
    public BaseState giveUpState;
    public float giveUpDelay;
    private float elapsedGiveUpDelay;


    [Tooltip("The delay between entering this state, and chasing the player")]
    public float initialChaseDelay;
    private bool isWaitingForInitialChase;

    [Tooltip("The delay that occurs between reaching the player, attacking, and resuming chase")]
    public float chaseDelay;

    protected override void Awake() {
        base.Awake();
        baseEnemy = GetComponentInParent<BaseEnemy>();
        navMeshAgent = baseEnemy.GetComponent<NavMeshAgent>();
        animator = baseEnemy.GetComponentInChildren<Animator>();


        animHashIsWalking = Animator.StringToHash("isWalking");
        animHashAttack = Animator.StringToHash("attack");
    }

    public override void OnStateEnter(BaseState previousState) {
        base.OnStateEnter(previousState);

        motorSettings.ApplyMotorSettings(navMeshAgent);
        isAttacking = false;
        isWaitingForInitialChase = true;
        elapsedGiveUpDelay = 0f;
        StartCoroutine(BeginChaseAfterDelay());
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
        isAttacking = true;

        yield return new WaitForSeconds(chaseDelay);

        isAttacking = false;
    }
}
