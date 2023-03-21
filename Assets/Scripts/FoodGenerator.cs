using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGenerator : MonoBehaviour
{
    public int foodCount;
    public int maxFoodCount;
    public MapGenerator m;
    public GameObject Food;
    public float FoodCost;
    private GameObject FoodPrefab;
    public int[,] foodPos;

    void Start() {
      FoodPrefab = Food;
      UpdateFood();
    }

    void Update()
    {
      if (foodCount < maxFoodCount) {
        UpdateFood();
      }
    }
    public void UpdateFood() {
      for (int x = foodCount; x < maxFoodCount; x ++) {
        int posX = Random.Range(0, m.width);
        int posY = Random.Range(0, m.height);

        while (m.map[posX, posY] == 1) {
          posX = Random.Range(0, m.width);
          posY = Random.Range(0, m.height);
        }
        GameObject Food = (GameObject)Instantiate(FoodPrefab, transform.position = new Vector3((float)(-m.width/2 + posX + 0.5f), 0, (float)(-m.height/2 + posY + 0.5f)), Quaternion.identity);
        Food.GetComponent<Food>().x = posX;
        Food.GetComponent<Food>().y = posY;
        Food.name = "Food";
        //amongua
    }
  }
}
