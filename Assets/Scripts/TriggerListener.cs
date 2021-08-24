using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerListener : MonoBehaviour {
    public UnityEvent onTriggerEnter;
    public UnityEvent onPlayerTriggerEnter;

    private void OnTriggerEnter(Collider other) {
        onTriggerEnter.Invoke();

            if (other.CompareTag(Player.Instance.tag)) {
                onPlayerTriggerEnter.Invoke();
            }
        }
    }
