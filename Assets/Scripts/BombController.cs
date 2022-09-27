using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField]private float waitForExplode;
    [SerializeField]private float waitForDestroy;
    private Animator animator;
    private bool isActive;
    private int IDIsActive;
    private Transform transformBomb;
    [SerializeField] private float expansiveWaveRange;
    [SerializeField] private LayerMask isDestroyable;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        IDIsActive = Animator.StringToHash("isActive");
        transformBomb = GetComponent<Transform>();
    }

    private void Update()
    {
        waitForExplode -= Time.deltaTime;
        waitForDestroy -= Time.deltaTime;
        if(waitForExplode <= 0 && !isActive) ActivatedBomb();
        if (waitForDestroy <=0) Destroy(gameObject);
    }

    private void ActivatedBomb()
    {
        isActive = true;
        animator.SetBool(IDIsActive, isActive);
        Collider2D[] destroyedObjects = Physics2D.OverlapCircleAll(transformBomb.position,expansiveWaveRange,isDestroyable);
        if (destroyedObjects.Length > 0)
        {
            foreach (var col in destroyedObjects)
            {
                Destroy(col.gameObject);
            }
        }
    }
}
