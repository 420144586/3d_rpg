using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")] 
    public float kickForce = 10;

    public GameObject rockPrefab;

    public Transform handPos;
    

    //animation event
    public void kickOff()
    {
        if (attackTarget != null && transform.isFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
            
            
            targetStats.takeDamage(characterStats, targetStats);
        }
    }
    
    //animation event
    public void throwRock()
    {
        attackTarget = FindObjectOfType<PlayerController>().gameObject;
        Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        var rock = Instantiate(rockPrefab, handPos.position + Vector3.up, randomRotation); //生成函数第一个为生成物，第二个为生成坐标，第三个参数为旋转角度
        rock.GetComponent<Rock>().target = attackTarget;
     
      
    }
}
