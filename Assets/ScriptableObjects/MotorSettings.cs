using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MotorSettings")]
public class MotorSettings : ScriptableObject {
    public float acceleration;
    public float angularSpeed;
    public float speed;
    public float stoppingDistance;

    public void ApplyMotorSettings(NavMeshAgent agent) {
        agent.acceleration = acceleration;
        agent.angularSpeed = angularSpeed;
        agent.speed = speed;
        agent.stoppingDistance = stoppingDistance;
    }
}
