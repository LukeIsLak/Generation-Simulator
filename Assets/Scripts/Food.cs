using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
  public FoodGenerator FG;
  public int x;
  public int y;

  public List<NewCreatureBecauseImFuckingPissed> currentNearby = new List<NewCreatureBecauseImFuckingPissed>();
  public bool isTaken = false;

  void Start() {
    this.transform.SetParent(GameObject.FindGameObjectWithTag("MapGen").transform);
    FG.foodCount ++;
  }
  void OnDestroy() {
    foreach (NewCreatureBecauseImFuckingPissed c in currentNearby) {
      c.possibleFood.Remove(this.gameObject);
    }
    FG.foodCount --;
  }
  private void DestroyThis() {
    Destroy(this.gameObject);
  }
}
