using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDHome : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI txtStage;

   private void Start()
   {
      txtStage.text = "Stage " + GameManager.Instance.CurrentStage;
   }

   public void OnbtnPlay()
   {
      ScreenFader.Instance.FadeIn(() =>
      {
         SceneManager.LoadScene("Game");
         ScreenFader.Instance.FadeOut();
      });
   }
}
