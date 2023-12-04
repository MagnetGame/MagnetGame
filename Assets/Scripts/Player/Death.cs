using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    private GameObject player;

    public GameObject DeathUI;

    private void Start()
    {
        player = this.gameObject;
        DeathUI.SetActive(false);
    }
    private void OnParticleCollision(GameObject other)
    {
        player.SetActive(false);
        DeathUI.SetActive(true);
    }
}
