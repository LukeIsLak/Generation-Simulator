using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallData : MonoBehaviour
{
  public MapGenerator mapData;
  public int mapX;
  public int mapY;
  public int surroundingWallCount1 = 8;
  public int surroundingWallCount2 = 16;
  public int surroundingWallCount3 = 32;
  public bool isWater;
  public int i;

  [Header("Water")]
  public Material shallowWater;
  public Material medWater;
  public Material DeepWater;

  [Header("Ground")]
  public Material shallowGround;
  public Material medGround;
  public Material DeepGround;
  private BoxCollider bc;
  void Start()
  {
    this.transform.SetParent(GameObject.FindGameObjectWithTag("MapGen").transform);
    bc = this.GetComponent<BoxCollider>();
    int i = mapData.map[mapX,mapY];
    for (int x = mapX-1; x <= mapX+1; x++) {
      for (int y = mapY-1; y <= mapY+1; y++) {
          if (mapData.map[x,y] != i) {
            surroundingWallCount1 --;
        }
      }
    }
    for (int x = mapX-2; x <= mapX+2; x++) {
      for (int y = mapY-2; y <= mapY+2; y++) {
        if (mapData.map[x,y] != i) {
          surroundingWallCount2 --;
        }
      }
    }
    for (int x = mapX-3; x <= mapX+3; x++) {
      for (int y = mapY-3; y <= mapY+3; y++) {
        if (mapData.map[x,y] != i) {
          surroundingWallCount3 --;
        }
      }
    }
    if (isWater) {
    if (surroundingWallCount3 <= 32) {
      GetComponent<MeshRenderer>().material = DeepWater;
    }
    if (surroundingWallCount2 < 16) {
      GetComponent<MeshRenderer>().material = medWater;
    }
    if (surroundingWallCount1 < 8) {
      GetComponent<MeshRenderer>().material = shallowWater;
    }
    isWater = false;
    gameObject.name = "Water";
  }
    else {
      if (surroundingWallCount3 <= 32) {
        GetComponent<MeshRenderer>().material = DeepGround;
      }
        if (surroundingWallCount2 < 16) {
          GetComponent<MeshRenderer>().material = medGround;
        }
        if (surroundingWallCount1 < 8) {
          GetComponent<MeshRenderer>().material = shallowGround;
          isWater = true;
          GetComponent<BoxCollider>().isTrigger = true;
          gameObject.name = "Sand";
          }
        }
        DestroyComponents();
    }
    public void DestroyComponents() {
      if (isWater && surroundingWallCount1 >= 8) {
      Destroy(bc);
      }
    }
}
