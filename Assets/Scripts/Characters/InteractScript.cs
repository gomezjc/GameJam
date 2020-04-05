using System;
using Characters;
using UnityEngine;

public class InteractScript : MonoBehaviour
{

   public GameObject interactText;
   private bool isTalking;
   
   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.E) && isTalking)
      {
         Debug.Log("presiono la E");
      }
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.gameObject.CompareTag("Person"))
      {
         PatrolPeople people = other.GetComponentInParent<PatrolPeople>();
         people.stopPath();
         isTalking = true;
         interactText.SetActive(true);
      }
      
   }

   private void OnTriggerExit2D(Collider2D other)
   {
      if (other.gameObject.CompareTag("Person"))
      {
         PatrolPeople people = other.GetComponentInParent<PatrolPeople>();
         people.continuePath();
         isTalking = false;
         interactText.SetActive(false);
      }
   }
}
