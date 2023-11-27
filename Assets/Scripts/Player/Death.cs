using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = this.gameObject;
    }
    private void OnParticleCollision(GameObject other)
    {
        player.SetActive(false);
    }
}
