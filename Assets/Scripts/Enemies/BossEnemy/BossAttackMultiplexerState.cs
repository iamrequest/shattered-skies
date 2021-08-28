using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackMultiplexerState : BaseState {
    public BossRainAttackState rainAttack;


    protected override void Awake() {
        base.Awake();
    }

    public override void OnStateEnter(BaseState previousState) {
        base.OnStateEnter(previousState);
    }
    public override void OnStateExit(BaseState previousState) {
        base.OnStateEnter(previousState);
    }

    public void GetNextAttack() {
        if (rainAttack.isSpawningProjectiles) {
        } else {
        }
    }
}
