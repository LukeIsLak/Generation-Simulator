using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopulationGraph : MonoBehaviour
{
    public float secondsBetweenIterations;
    public float currSeconds;
    public GraphVisualizer gv;

    public GameObject highNum;

    public float curCount = 0;

    void Update() {
    currSeconds -= 1 * Time.deltaTime;
    highNum.GetComponent<TMPro.TextMeshProUGUI>().text = gv.highestNum.ToString();

    if (currSeconds <= 0) {
      gv.iterations += 1;
      gv.values.Add(curCount);
      gv.updateGraph = true;
      currSeconds = secondsBetweenIterations;
    }
  }
}
