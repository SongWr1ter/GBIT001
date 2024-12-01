using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : RoutineTrap
{
    private Collider2D[] colliders;
    [SerializeField]private Sprite spikeOnSprite;
    [SerializeField]private Sprite spikeOffSprite;
    private SpriteRenderer[] spikeRenderers = new SpriteRenderer[4];
    // Start is called before the first frame update
    void Start()
    {
       colliders = GetComponentsInChildren<Collider2D>();
       for (int i = 0; i < colliders.Length; i++)
       {
           spikeRenderers[i] = colliders[i].gameObject.GetComponent<SpriteRenderer>();
       }
    }



    protected override void TrapOn()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            var c = colliders[i];
            c.enabled = true;
            spikeRenderers[i].sprite = spikeOnSprite;
            SoundManager.PlayAudio("spike",transform.position,9f);
        }
    }

    protected override void TrapOff()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            var c = colliders[i];
            c.enabled = false;
            spikeRenderers[i].sprite = spikeOffSprite;
        }
    }
}
