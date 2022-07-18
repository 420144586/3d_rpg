using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD}
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour, IEndGameObserver
{

    protected CharacterStats characterStats;

    private EnemyStates enemyStates;
    
    private NavMeshAgent agent;

    private Animator anim;

    private Collider coll;

    [Header("Base Settings")]

    public float sightRadius;

    protected GameObject attackTarget;

    public bool isGuard;
   
    private float speed;

    public float lookAtTime;
    private float remainLookAtTime;

    private float lastAttackTime;

    private Quaternion guardRotation;

    [Header("Patrol State")]

    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;
    public bool isPatrol;


    //bool配合动画的转换
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool isPlayerDead;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }
    void Start() {
        if(isGuard) {
            enemyStates = EnemyStates.GUARD;
        }
        
        if(isPatrol) {
            enemyStates = EnemyStates.PATROL;
            getNewWayPoint();
        }
        //TODO: 场景切换功能加入后删除
        GameManager.Instance.addObserver(this);
    }
    
    void OnEnable()
    {
        //切换场景的时候加回来
        // GameManager.Instance.addObserver(this);

       
    }

    void OnDisable()
    {
        if(!GameManager.isInitialized) return;
        GameManager.Instance.removeObserver(this);   
    }

    void Update() {
        
        isDead = characterStats.currentHealth == 0;
        if (!isPlayerDead)
        {
            switchStates();
            switchAnimation();
            lastAttackTime -= Time.deltaTime;
        }

        
    }

    void switchAnimation() {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    void switchStates() {

        if(isDead) {
            enemyStates = EnemyStates.DEAD;
        
        }
        else if(foundPlayer()){
            enemyStates = EnemyStates.CHASE;  
        }

        switch(enemyStates) {
            case EnemyStates.GUARD:
                guard();
                break;
            case EnemyStates.PATROL:
                patrol();
                break;
            case EnemyStates.CHASE:
                chasePlayer();
                break;
            case EnemyStates.DEAD:
                dead();
                break;
        }
    }

    bool foundPlayer() {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        
        foreach (var target in colliders)
        {
            if(target.CompareTag("Player")) {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    void chasePlayer() {
        isWalk = false;
        isChase = true;
        agent.speed = speed;
        
        if(!foundPlayer()) {
            isFollow = false;
            
            //TODO: 拉脱回到上一个状态
            if(remainLookAtTime > 0){
                agent.destination = transform.position;
                remainLookAtTime -= Time.deltaTime;
            } 
            else if (isGuard) 
                enemyStates = EnemyStates.GUARD;
            else if (isPatrol)
                enemyStates = EnemyStates.PATROL;

            
        }else{
            isFollow = true; 
            agent.isStopped = false;
            //TODO: 追玩家
            agent.destination = attackTarget.transform.position;
        }
        //TODO: 在攻击范围内，要攻击
        //TODO: 配合动画
        if(targetInAttackRange() || targetInSkillRange()){
            isFollow = false;
            agent.isStopped = true;

            if(lastAttackTime < 0) {
                lastAttackTime = characterStats.attackData.coolDown;
                //暴击判断
                characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                //执行攻击
                attack();

            }

        }
    }

    void attack() {
        transform.LookAt(attackTarget.transform);
        if(targetInAttackRange()) {
            //近战动画
            anim.SetTrigger("Attack");
        }
        if(targetInSkillRange()) {
            //技能攻击动画
            anim.SetTrigger("Skill");
        }
    }
    void dead() {
        coll.enabled = false;
        // agent.enabled = false;
        agent.radius = 0;
        Destroy(gameObject, 2f); 
    }
    bool targetInAttackRange() {
        if(attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else 
            return false;
    }
    bool targetInSkillRange() {
        if(attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else 
            return false;
    }
    void patrol() {
        isChase = false;
        agent.speed = speed * 0.5f;
        //判断是否到了随机巡逻的点上
        if(Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance){
            isWalk = false;
            //到了巡逻点看一会再说
            if(remainLookAtTime > 0)
                remainLookAtTime -= Time.deltaTime;
            else
                getNewWayPoint();
        } else {
            isWalk = true;
            agent.destination = wayPoint;
        }
    }

    void guard() {
        isChase = false;
        if(transform.position != guardPos) {
            isWalk = true;
            agent.isStopped = false;
            agent.destination = guardPos;
            if(Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance) {
                isWalk = false;
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.1f);
            }
        }
    }
    void getNewWayPoint() { 
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1)? hit.position : transform.position;
    }

    void OnDrawGizmosSelected()
   {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, sightRadius);
   }

   //Animation Event
   void hit() {
        
        if(attackTarget != null && transform.isFacingTarget(attackTarget.transform)) {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.takeDamage(characterStats, targetStats); 
        }
   }

   public void endGameNotify()
   {
       //所有enemy 在这时的操作
       //获胜动画
       //停止所有移动
       //停止agent
      
       anim.SetBool("Victory", true);
       isPlayerDead = true;
       isChase = false;
       isWalk = false;
       attackTarget = null;
       
       
   }
}
