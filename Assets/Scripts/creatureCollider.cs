using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creatureCollider : MonoBehaviour
{
  public NewCreatureBecauseImFuckingPissed cr;

  public void Start() {
    this.gameObject.GetComponent<SphereCollider>().radius = cr.maxSensorRadius;
  }
  public void Update() {
    if (cr.nd.isDay == false) {this.gameObject.GetComponent<SphereCollider>().radius = cr.maxSensorRadius/2;}
    else {this.gameObject.GetComponent<SphereCollider>().radius = cr.maxSensorRadius;}
  }
  public void OnTriggerEnter (Collider col) {
    if (col.gameObject.name == "Food") {
      if (!cr.possibleFood.Contains(col.gameObject) && col.gameObject.GetComponent<Food>().isTaken == false) {
        cr.possibleFood.Add(col.gameObject);
        col.gameObject.GetComponent<Food>().currentNearby.Add(cr);
      }
    }
    if (col.gameObject.name == "Sand") {
      if (!cr.possibleWater.Contains(col.gameObject)) {
        cr.possibleWater.Add(col.gameObject);
      }
    }
    if (col.gameObject.tag == "Enemy") {
      if (col.gameObject.GetComponent<NewEnemyBecauseImFuckingPissed>().currentPrey == cr.gameObject) {
        cr.isBeingHunted = true;
        cr.currentEnemy = col.gameObject;
      }
    }
    if (cr.isMature && cr.isMale && cr.lookingForMate) {
      if (col.gameObject.tag == "Rabbit") {
        Debug.Log("Test");
        if (col.gameObject.GetComponent<NewCreatureBecauseImFuckingPissed>().isFemale == true) {
          Debug.Log("Test4");
          if (cr.isMale == true) {
            Debug.Log("Test2");
              cr.PossibleMateFound(col.gameObject);
              if (!cr.rejectedMates.Contains(col.gameObject)) {
                Debug.Log("Test3");
                cr.FoundPossibleMate = true;
                col.gameObject.GetComponent<NewCreatureBecauseImFuckingPissed>().FoundPossibleMate = true;

                cr.currentMate = col.gameObject;
                col.gameObject.GetComponent<NewCreatureBecauseImFuckingPissed>().currentMate = cr.gameObject;
              }
              else {
                Debug.Log("Test5");
              }
            }
            else {
              Debug.Log("Test6");
            }
          }
          else {
            Debug.Log("Test7");
          }
        }
        else {
          if (!cr.isFemale) {
          Debug.Log("Test8");
        }
        }
      }
    }
    public void OnTriggerExit(Collider col) {
    if (col.gameObject.name == "Food") {
      if (cr.possibleFood.Contains(col.gameObject)) {
        cr.possibleFood.Remove(col.gameObject);
        col.gameObject.GetComponent<Food>().currentNearby.Remove(cr);
      }
    }
    if (col.gameObject.name == "Sand") {
      if (cr.possibleWater.Contains(col.gameObject)) {
        cr.possibleWater.Remove(col.gameObject);
      }
    }
    if (col.gameObject.tag == "Enemy" && cr.currentEnemy == col.gameObject) {
      cr.isBeingHunted = false;
      cr.currentEnemy = null;
      cr.isAction = false;
      cr.secondsBetweenActions = cr.initialSecondBetweenActions;
    }
  }
}
