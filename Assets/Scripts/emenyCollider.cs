using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class emenyCollider : MonoBehaviour
{
  public NewEnemyBecauseImFuckingPissed ne;
  public void OnTriggerStay (Collider col) {
    if (col.gameObject.tag == "Rabbit") {
      ne.foundPrey = true;
      if (ne.currentPrey != null) {
      ne.currentPrey = (Vector3.Distance(ne.currentPrey.transform.position, ne.transform.position) > Vector3.Distance(col.gameObject.transform.position, ne.transform.position))? col.gameObject : ne.currentPrey;
      }
      else {
        ne.currentPrey = col.gameObject;
      }
    }
    if (col.gameObject.name == "Sand") {
      if (!ne.possibleWater.Contains(col.gameObject)) {
        ne.possibleWater.Add(col.gameObject);
      }
    }
    if (ne.isMature) {
      if (col.gameObject.GetComponent<NewEnemyBecauseImFuckingPissed>() != null) {
        if (col.gameObject.GetComponent<NewEnemyBecauseImFuckingPissed>().isFemale == true && ne.isMale == true && ne.lookingForMate) {
          if (ne.isMale == true) {
              ne.PossibleMateFound(col.gameObject);
              if (!ne.rejectedMates.Contains(col.gameObject)) {
                ne.FoundPossibleMate = true;
                col.gameObject.GetComponent<NewEnemyBecauseImFuckingPissed>().FoundPossibleMate = true;

                ne.currentMate = col.gameObject;
                col.gameObject.GetComponent<NewEnemyBecauseImFuckingPissed>().currentMate = ne.gameObject;
              }
            }
          }
        }
      }
    }
    public void OnTriggerExit(Collider col) {
    if (col.gameObject.name == "Sand") {
      if (ne.possibleWater.Contains(col.gameObject)) {
        ne.possibleWater.Remove(col.gameObject);
      }
    }
    if (col.gameObject.tag == "Rabbit" && ne.currentPrey == col.gameObject) {
      ne.foundPrey = false;
      ne.currentPrey = null;
      ne.isAction = false;
      ne.secondsBetweenActions = 0f;
    }
  }
}
