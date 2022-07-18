using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject attackTarget;

    private float lastAttackTime;

    private bool isDead;

    private float stopDistance;

    private CharacterStats characterStats;
    

    void Awake() {

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;
    }

    private void OnEnable()
    {
        MouseManager.Instance.onMouseClicked += MoveToTarget;
        MouseManager.Instance.onEnemyClicked += EventAttack;
        GameManager.Instance.RigisterPlayer(characterStats);
    }

    private void OnDisable()
    {
        if(!MouseManager.isInitialized) return;
        MouseManager.Instance.onMouseClicked -= MoveToTarget;
        MouseManager.Instance.onEnemyClicked -= EventAttack;
    } 

    // Start is called before the first frame update
    void Start()
    {
        SaveManager.Instance.loadplayerData();
    }

    // Update is called once per frame
    void Update()
    {
        isDead = characterStats.currentHealth == 0;
        if(isDead) 
            GameManager.Instance.notifyObservers();
        switchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    private void switchAnimation() {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    
    }

    public void MoveToTarget(Vector3 target)
    {


        StopAllCoroutines();
        if (isDead) return;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }
    public void EventAttack(GameObject target){
        if (isDead) return;
        if(target != null) {

            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(moveToAttackTarget());
        }
    }
    IEnumerator moveToAttackTarget() {
        agent.isStopped = false;

        agent.stoppingDistance = characterStats.attackData.attackRange;
        transform.LookAt(attackTarget.transform);
        //TODO: 武器攻击距离
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange){
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        agent.isStopped = true;
        //attack

        if(lastAttackTime < 0) {

            anim.SetTrigger("Attack");
            anim.SetBool("Critical", characterStats.isCritical);
            //设置攻击冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }

    //Animation Event
    void hit() {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>())
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                //TODO: 20f 可以为击打力量的变量
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 70, ForceMode.Impulse);
            }

            
            
             
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.takeDamage(characterStats, targetStats);
        }

      

    }
}
