using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer, HitEnemy, HitNothing}

    public RockStates rockStates;

    public int damage;
    
    private Rigidbody rb;
    [Header("Base Settings")]

    public float force;

    public GameObject target;

    private Vector3 direction;

    public GameObject breakEffect;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;
        flyToTarget();
        
    }

    void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }

    public void flyToTarget()
    {
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                hitPlayer(collision);
                break;
            case RockStates.HitEnemy:
                hitEnemy(collision);
                break;
            case RockStates.HitNothing:
                hitNothing(collision);
                break;
        }
    }

    private void hitNothing(Collision collision)
    {

    }

    private void hitEnemy(Collision collision)
    {
        if (collision.gameObject.GetComponent<Golem>())
        {
            var cObj = collision.gameObject.GetComponent<CharacterStats>();
            cObj.takeDamage(damage, cObj);
            Instantiate(breakEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void hitPlayer(Collision collision)
    {
        var cObj = collision.gameObject;
        if (cObj.CompareTag("Player"))
        {
            cObj.GetComponent<NavMeshAgent>().isStopped = true;
            cObj.GetComponent<NavMeshAgent>().velocity = direction * force;
            
            cObj.GetComponent<Animator>().SetTrigger("Dizzy");
            cObj.GetComponent<CharacterStats>().takeDamage(damage, cObj.GetComponent<CharacterStats>());

            rockStates = RockStates.HitNothing;
        }
    }
}
