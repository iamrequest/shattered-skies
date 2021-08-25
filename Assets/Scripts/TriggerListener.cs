using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerListener : MonoBehaviour
{
    public UnityEvent onTriggerEnter, onTriggerExit;
    public UnityEvent onPlayerTriggerEnter, onPlayerTriggerExit;

    private void OnTriggerEnter(Collider other) {
        onTriggerEnter.Invoke();

        if (other.CompareTag(Player.Instance.tag)) {
            onPlayerTriggerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other) {
        onTriggerExit.Invoke();

        if (other.CompareTag(Player.Instance.tag)) {
            onPlayerTriggerExit.Invoke();
        }
    }
}
