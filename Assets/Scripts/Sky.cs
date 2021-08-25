using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sky : MonoBehaviour {

    private void Update() {
        transform.position = Player.Instance.playerController.transform.position;
    }
}
