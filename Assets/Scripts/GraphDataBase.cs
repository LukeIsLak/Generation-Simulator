using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphDataBase : MonoBehaviour
{
  public List<string> CreatureValuesOfNote;
  public List<float> allLists = new List<float>();
  public List<float> currentAverage = new List<float>();
  public List<List<float>> previousAverages = new List<List<float>>();

  public List<GameObject> allListObj = new List<GameObject>();
  public List<GraphVisualizer> allGraphs = new List<GraphVisualizer>();

  public float secondsBetweenGenerations;
  public float currSecondsBetween;

  public GameObject parent;

  public GameObject GraphPrefab;
  public List<Vector3> graphPosition;

  public void Start() {
  currSecondsBetween = secondsBetweenGenerations;
  for(int i = 0; i < CreatureValuesOfNote.Count; i++) {
    GenerateGraph(CreatureValuesOfNote[i], i);
    }
  }
  public void LateUpdate() {
    currSecondsBetween -= 1 * Time.deltaTime;
    if (currSecondsBetween <= 0) {
      for (int i = 0; i < CreatureValuesOfNote.Count; i++) {
        Debug.Log(i);
        CollectData(CreatureValuesOfNote[i], i);
      }
    currSecondsBetween = secondsBetweenGenerations;
    }

  }

  public void CollectData(string value, int pos) {
    float addMean = 0;
    float count = 0;

    for(int i = 0; i < allListObj.Count; i++) {
      float newVal = (float)typeof(NewCreatureBecauseImFuckingPissed).GetField(value).GetValue(allListObj[i].GetComponent<NewCreatureBecauseImFuckingPissed>());
      addMean += newVal;
      count += 1;
      }
    currentAverage[pos] = (addMean/count);
    allGraphs[pos].GetComponent<GraphVisualizer>().values.Add(currentAverage[pos]);
    allGraphs[pos].GetComponent<GraphVisualizer>().iterations += 1;
    allGraphs[pos].GetComponent<GraphVisualizer>().updateGraph = true;
  }

  public void GenerateGraph(string value, int pos) {
    GameObject newGraph = Instantiate(GraphPrefab, transform.position, Quaternion.identity);
    RectTransform g_RectTransform = newGraph.GetComponent<RectTransform>();
    newGraph.GetComponent<RectTransform>().SetParent(parent.GetComponent<RectTransform>());
    g_RectTransform.localPosition = graphPosition[pos];

    float addMean = 0;
    allLists = null;
    float count = 0;

    for(int i = 0; i < allListObj.Count; i++) {
      float newVal = (float)typeof(NewCreatureBecauseImFuckingPissed).GetField(value).GetValue(allListObj[i].GetComponent<NewCreatureBecauseImFuckingPissed>());
      addMean += newVal;
      count += 1;
      }

    currentAverage.Add(addMean/count);

    newGraph.GetComponent<GraphVisualizer>().values.Add(currentAverage[pos]);
    newGraph.GetComponent<GraphVisualizer>().iterations += 1;
    newGraph.GetComponent<GraphVisualizer>().updateGraph = true;
    newGraph.GetComponent<GraphVisualizer>().variable.GetComponent<TMPro.TextMeshProUGUI>().text = value;
    allGraphs.Add(newGraph.GetComponent<GraphVisualizer>());
  }
}
