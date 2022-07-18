using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
   public GameObject healthBarPrefab;

   public Transform healthBarPoint;

   private Image healthSlider;

   private Transform uiBar;

   private Transform cam;

   private CharacterStats currentStats;

   public bool alwaysVisible;

   public float visibleTime;

   private float timeLeft;
   
   

   private void Awake()
   {
      currentStats = GetComponent<CharacterStats>();

      currentStats.updateHealthBarOnAttack += updateHealthBar;
   }

   private void OnEnable()
   {
      cam = Camera.main.transform;
      foreach (Canvas canvas  in FindObjectsOfType<Canvas>())
      {
         //可能找不准
         if (canvas.renderMode == RenderMode.WorldSpace)
         {
            uiBar = Instantiate(healthBarPrefab, canvas.transform).transform;
            healthSlider = uiBar.GetChild(0).GetComponent<Image>();
            visibleTime = 5f;
            uiBar.gameObject.SetActive(alwaysVisible);
            


         }

      } 
   }

   private void updateHealthBar(int currentHealth, int maxHealth)
   {
      if(currentHealth <= 0)
         Destroy(uiBar.gameObject);

      uiBar.gameObject.SetActive(true);
      timeLeft = visibleTime;
      float sliderPercent = (float)currentHealth / maxHealth;
      healthSlider.fillAmount = sliderPercent;
      
      


   }
   

   private void LateUpdate()
   {
      if (uiBar != null)
      {
         uiBar.position = healthBarPoint.position;
         //血条永远面对摄像机
         uiBar.forward = -cam.forward;

         if (timeLeft <= 0 && !alwaysVisible)
         {
            uiBar.gameObject.SetActive(false);
         }
         else
         {
            timeLeft -= Time.deltaTime;
         }



      }
   }
}
 