using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> updateHealthBarOnAttack;
    
    public CharacterData_SO templateData;
    
    public CharacterData_SO characterData;

    public AttackData_SO attackData; 

    [HideInInspector]
    public bool isCritical;

    private void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
    }

    #region Read from Data_SO
    public int maxHealth{
        get {
            if(characterData != null) 
                return characterData.maxHealth;
            else return 0;
        }
        set {
            characterData.maxHealth = value;
        }
    }
    public int currentHealth{
        get {
            if(characterData != null) 
                return characterData.currentHealth;
            else return 0;
        }
        set {
            characterData.currentHealth = value;
        }
    }
    public int baseDefence{
        get {
            if(characterData != null) 
                return characterData.baseDefence;
            else return 0;
        }
        set {
            characterData.baseDefence = value;
        }
    }
    public int currentDefence{
        get {
            if(characterData != null) 
                return characterData.currentDefence;
            else return 0;
        }
        set {
            characterData.currentDefence = value;
        }
    }
    #endregion

    #region Character Combat
    public void takeDamage(CharacterStats attacker, CharacterStats defener){
        int damage = Mathf.Max(attacker.currentDamage() - defener.currentDefence, 0);
        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if(attacker.isCritical) {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        //TODO: 更新ui
        updateHealthBarOnAttack?.Invoke(currentHealth, maxHealth);
        //TODO: 经验获取等
        if(currentHealth <= 0)
            attacker.characterData.updateExp(characterData.killPoint);
    }

    public void takeDamage(int damage, CharacterStats defener)
    {
        int currentDamage = Mathf.Max(damage - defener.currentDefence, 0);
        currentHealth = Mathf.Max(currentHealth - currentDamage, 0);
        updateHealthBarOnAttack?.Invoke(currentHealth, maxHealth);
        
        if(currentHealth <= 0)
            GameManager.Instance.playerStats.characterData.updateExp(characterData.killPoint);
    }

    private int currentDamage() {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        if(isCritical){
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("baoji");
        }

        return (int)coreDamage;
    }
    #endregion
}
 