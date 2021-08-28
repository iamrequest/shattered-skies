using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHiddenState : BaseState {
    private BossEnemy enemy;
    private int animHashIsHidden;

    protected override void Awake() {
        base.Awake();
        enemy = GetComponentInParent<BossEnemy>();
        animHashIsHidden = Animator.StringToHash("hidden");
    }

    public override void OnStateEnter(BaseState previousState) {
        base.OnStateEnter(previousState);
        enemy.animator.SetBool(animHashIsHidden, true);
    }
    public override void OnStateExit(BaseState nextState) {
        base.OnStateEnter(nextState);
        enemy.animator.SetBool(animHashIsHidden, false);
    }
}
