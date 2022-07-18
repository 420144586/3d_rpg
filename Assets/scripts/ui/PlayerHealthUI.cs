using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthUI : MonoBehaviour
{
   private Text levelText;
   private Image healthSlider;
   private Image expSlider;

   void Awake()
   {
      levelText = transform.GetChild(2).GetComponent<Text>();
      healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
      expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
      
   }

   private void Update()
   {
      levelText.text = "LEVEL " + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");
      updateHealth();
      expUpdate();
      
   }

   void updateHealth()
   {
      float sliderPercent = (float)GameManager.Instance.playerStats.currentHealth / GameManager.Instance.playerStats.maxHealth;
      healthSlider.fillAmount = sliderPercent;
   }

   void expUpdate()
   {
      float sliderPercent = (float)GameManager.Instance.playerStats.characterData.currentExp /
                            GameManager.Instance.playerStats.characterData.baseExp;
      expSlider.fillAmount = sliderPercent;
   }
}
