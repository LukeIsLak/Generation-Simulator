using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightAndDay : MonoBehaviour
{
  public float timeRate;
  public float nightRate;
  public float curTime;
  public float daytime;
  public bool isDay;
    void Update(){
    if (curTime <= daytime || curTime >= 360 - daytime) {
      curTime += timeRate * Time.deltaTime;
      transform.eulerAngles = new Vector3(curTime, 0, 0);
      isDay = true;
    }
    else {
      curTime += nightRate * Time.deltaTime;
      transform.eulerAngles = new Vector3(curTime, 0, 0);
      isDay = false;
    }
    if (curTime >= 360) {
      curTime = 0;
      }
    }
}
