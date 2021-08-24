using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : BaseState {
    private BaseEnemy baseEnemy;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private int animHashIsWalking;

    public BaseState onPlayerSpottedState;
    public List<Transform> patrolPoints;
    public int currentPatrolIndex;

    [Tooltip("The delay before starting the walk to a new point")]
    public float perPointDelay;
    private Coroutine coroutineQueueNextPoint;
    private bool isWaitingAtPoint;


    protected override void Awake() {
        base.Awake();
        baseEnemy = GetComponentInParent<BaseEnemy>();
        navMeshAgent = baseEnemy.GetComponent<NavMeshAgent>();
        animator = baseEnemy.GetComponentInChildren<Animator>();

        animHashIsWalking = Animator.StringToHash("isWalking");
    }

    public override void OnStateEnter(BaseState previousState) {
        base.OnStateEnter(previousState);

        isWaitingAtPoint = false;
        animator.SetBool(animHashIsWalking, true);
        navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    public override void OnStateExit(BaseState previousState) {
        base.OnStateExit(previousState);
        StopCoroutine(coroutineQueueNextPoint);
    }

    public override void OnStateUpdate() {
        base.OnStateFixedUpdate();

        if (baseEnemy.vision.isPlayerInSight()) {
            parentFSM.TransitionTo(onPlayerSpottedState);
        }

        if (patrolPoints.Count > 1) {
            // Don't queue up a new point if the agent is just waiting at some point
            if (!isWaitingAtPoint) {
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
                    coroutineQueueNextPoint = StartCoroutine(QueueNextWaypoint());
                }
            }
        }
    }

    private IEnumerator QueueNextWaypoint() {
        animator.SetBool(animHashIsWalking, false);
        isWaitingAtPoint = true;

        yield return new WaitForSeconds(perPointDelay);

        animator.SetBool(animHashIsWalking, true);
        isWaitingAtPoint = false;

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }
}
