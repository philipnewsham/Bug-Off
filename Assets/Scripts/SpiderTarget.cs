using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderTarget : MonoBehaviour
{
    [SerializeField] private Spider _spider;
    [SerializeField] private Collider2D _collider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Fly"))
        {
            return;
        }

        _spider.OnTargetHit();
        _collider.enabled = false;
    }
}
