using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class GraphVisualizer : MonoBehaviour {

  public int iterations;
  public List<GameObject> points;

  public List<float> values;

  public GameObject pointPrefab;
  public GameObject widthLine;
  public GameObject parent;

  public GameObject median;
  public GameObject highNum;
  public GameObject lowNum;
  public GameObject variable;

  public float highestNum = 0;
  public float lowestNum = 0;

  public float lowThresh;

  public bool updateGraph = false;

  void LateUpdate() {
    if (updateGraph || points.Count != iterations && iterations <= values.Count) {
      UpdateGraph();
    }
  }

  public void UpdateGraph() {
    Debug.Log("Updating Graph");
    foreach (GameObject p in points) {
      Destroy(p);
    }

    points = new List<GameObject>();

    float spaceBetween = widthLine.GetComponent<RectTransform>().sizeDelta.x / (iterations - 1);

    int medianInt = 0;
    float meadianNum = 0;

    if(highestNum < values.Last() && values.Count != 0) {
      highestNum = values.Last();
    }
    if(lowestNum > values.Last() && values.Count > 1) {
      lowestNum = values.Last();
    }
    if (values.Count == 2) {
      if (values[0] != values[1]) {
      lowestNum = (values[0] < values[1])? values[0] : values[1];
      }
      else {
        lowestNum = values[1] - lowThresh;
      }
    }

    for (int i = 0; i < iterations; i++) {
      if (i < values.Count) {
        GameObject newPoint = Instantiate(pointPrefab, transform.position, Quaternion.identity);
        RectTransform p_RectTransform = newPoint.GetComponent<RectTransform>();
        newPoint.GetComponent<RectTransform>().SetParent(parent.GetComponent<RectTransform>());
        float newY = widthLine.GetComponent<RectTransform>().localPosition.y + ((values[i]-lowestNum) * (200/(highestNum-lowestNum)));
        p_RectTransform.localPosition = new Vector3(((iterations == 1)? 0f : i * spaceBetween) - ((widthLine.GetComponent<RectTransform>().sizeDelta.x/2) - widthLine.GetComponent<RectTransform>().localPosition.x), newY, widthLine.GetComponent<RectTransform>().localPosition.z);
        points.Add(newPoint);
        updateGraph = false;

        medianInt += 1;
        meadianNum += values[i];
    }
  }
    highNum.GetComponent<TMPro.TextMeshProUGUI>().text = highestNum.ToString();
    lowNum.GetComponent<TMPro.TextMeshProUGUI>().text = lowestNum.ToString();
    median.GetComponent<RectTransform>().localPosition = new Vector3(widthLine.GetComponent<RectTransform>().localPosition.x, widthLine.GetComponent<RectTransform>().localPosition.y + (meadianNum/medianInt), widthLine.GetComponent<RectTransform>().localPosition.z);
  }
}
