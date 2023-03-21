using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [Header ("Creature Values")]
    public bool isMale;
    public bool isFemale;
    public int creatureID;
    public int generation = 1;

    [Header ("State")]
    public float hunger;
    public float thirst;
    public float age;
    public float reproductionUrge;

    [Header("Minutes")]
    public float maximumLifeExpectancy = 1;

    [Header("Percennamee")]
    public float sexuallyMature;

    [Header("Settings")]
    public bool lookMate = false;
    public bool lookFood = false;
    public bool lookWater = false;
    public bool isMature = false;
    public float hungerTilDeath = 100;
    public float thirstTilDeath = 100;
    public float speed = 1;
    private float startSpeed;
    public float maxSensorRadius = 9;
    public float secondsBetweenActions;
    public bool isAction = false;
    public GameObject currentMate;
    public GameObject currentWater;
    public GameObject currentFood;

    [Header("Genetics")]
    public float mutationChance;
    public float mutationChange;
    public float fitness;

    [Header("Male Components")]
    public float desireability = 100;
    private float desireabilityStart;
    public List<GameObject> rejectedMates = new List<GameObject>();

    [Header("Female Components")]
    public GameObject child;
    public GameObject childPrefab;
    public bool isPregnet;
    //Seconds
    public float pregnancyPeriod = 15;
    public float pregnancyPeriodStart = 15;
    public float timeBetweenPregnancy;

    [Header ("Lists")]
    public List<GameObject> possibleFood = new List<GameObject>();
    public List<GameObject> possibleWater = new List<GameObject>();
    public List<GameObject> possibleMates = new List<GameObject>();
    [Header ("Components")]
    public SphereCollider sensor;
    private Creature CM;
    public MapGenerator m;
    public Pathfinding p;
    public FoodGenerator FG;
    public List<Coord> MovementPath = new List<Coord>();

    void Awake() {
      PlayerPrefs.DeleteAll();
    }

    void Start() {
      //this ensures that all immediate values of note are reset and accurate
      CM = this;
      startSpeed = speed;
      creatureID = PlayerPrefs.GetInt("CreatureCount", 0) + 1;
      PlayerPrefs.SetInt("CreatureCount", creatureID);
      sensor = this.gameObject.GetComponent<SphereCollider>();

      //I need to make sure both males and females don't spawn in pregnent, because nature doesn't work like that
      desireabilityStart = desireability;
      pregnancyPeriod = pregnancyPeriodStart;
    }
    void LateStart() {
      //To ensure that values are not cross contaminated with one creatue
      possibleFood = new List<GameObject>();
      possibleMates = new List<GameObject>();
      possibleWater = new List<GameObject>();
    }
    void Update() {

      //First and foremost, make sure that the creature moves before it does anything else
      if (MovementPath.Count != 0) {
        float step = speed * Time.deltaTime;
        transform.LookAt(MovementPath[0].currentSpot);
        transform.rotation*=Quaternion.Euler(0, -90, 0);
        transform.position = Vector3.MoveTowards(transform.position, MovementPath[0].currentSpot, step);

        //makes sure it doesn't go running in a circle
        if (transform.position == MovementPath[0].currentSpot){
          MovementPath.Remove(MovementPath[0]);
        }
      }

      //if the creature is at it's destination, reset values
      else {
        if (isAction == true) {
          if (lookFood && currentFood != null && !lookWater && !lookMate) {
            hunger -= FG.FoodCost / 100;
            Destroy(currentFood);
            currentFood = null;
            lookFood = false;
            isAction = false;
          }
          if (lookWater && !lookFood && !lookMate) {
          currentWater = null;
          thirst -= FG.FoodCost / 100;
          lookWater = false;
          }
          if (lookMate && !lookWater && !lookFood) {
            if (isFemale) {
              if (transform.position == currentMate.transform.position) {
                isPregnet = true;
                isAction = false;
              }
            }
            else {
              isAction = false;
            }
            lookMate = false;
          }
        }
      }

      //Updates current states
      hunger += Time.deltaTime * 1 / hungerTilDeath;
      thirst += Time.deltaTime * 1 / thirstTilDeath;
      age += Time.deltaTime * 1 / 100;

      //Is it still waiting to do an action
      if (secondsBetweenActions > 0) {
        secondsBetweenActions -= Time.deltaTime;
      }

      //If it's going to do an action, determine what action to do
      if (secondsBetweenActions <= 0 && isAction == false) {
        Debug.Log("Finding Action");
        DetermingAction();
        isAction = true;
      }

      //Checks if creature is Sexually mature
      if (sexuallyMature <= age/(maximumLifeExpectancy / 100 * 60)*100 && isMature == false) {
        Debug.Log("Creature (" + generation + ")" + creatureID + " is Mature");
        isMature = true;
      }

      //Kills creature
      if (hunger >= 1 || thirst >= 1 || age >= maximumLifeExpectancy / 100 * 60) {
        Death(creatureID);
      }

      //Makes older male rabbits less desireable over time
      if (isMale) {
        if (isMature && desireability > 0) {
            reproductionUrge += Time.deltaTime * 1 / 100;
            desireability -= Time.deltaTime * 1 / age;
        }
      }

      //If it is female and pregnent, make sure that there is gestation
      if (isFemale) {
        if (isMature) {
          reproductionUrge += Time.deltaTime * 1 / 100;
        }
        if (isPregnet) {
          pregnancyPeriod -= (Time.deltaTime * 1 / 100) * 100;
          if (pregnancyPeriod <= 0) {
            GenerateChildren(currentMate.GetComponent<Creature>(), this.GetComponent<Creature>());
            isPregnet = false;
          }
        }
      }
    }

  //Determine Action of creature
  public void DetermingAction() {
    Debug.Log("Determined Action");

    //Pick highest need
    if (hunger > thirst && hunger > reproductionUrge) {
      lookFood = true;
      lookWater = false;
      lookMate = false;
    }
    if (thirst > hunger && thirst > reproductionUrge) {
      lookFood = false;
      lookWater = true;
      lookMate = false;
    }
    if (reproductionUrge > hunger && reproductionUrge > thirst) {
      lookFood = false;
      lookWater = false;
      lookMate = true;
    }

    //If actions are tied, pick one at random
    if (hunger == thirst) {
      float option_a = GetRandomAction(hunger, thirst);
      if (hunger == option_a) {
        lookFood = true;
        lookWater = false;
        lookMate = false;
      }
      else {
        lookFood = false;
        lookWater = true;
        lookMate = false;
      }
    }
    if (hunger == reproductionUrge) {
      float option_a = GetRandomAction(hunger, reproductionUrge);
      if (hunger == option_a) {
        lookFood = true;
        lookWater = false;
        lookMate = false;
      }
      else {
        lookFood = false;
        lookWater = false;
        lookMate = true;
      }
    }
    if (thirst == reproductionUrge) {
      float option_a = GetRandomAction(thirst, reproductionUrge);
      if (thirst == option_a) {
        lookFood = false;
        lookWater = true;
        lookMate = false;
      }
      else {
        lookFood = false;
        lookWater = false;
        lookMate = true;
      }
    }
    if (lookFood == true && lookWater == false && lookMate == false) {
      FindFood();
    }
    if (lookFood == false && lookWater == true && lookMate == false) {
      FindWater();
    }
    if (lookFood == true && lookWater == false && lookMate == true) {
      FindMate();
    }
  }
  public void FindWater() {
    Debug.Log(creatureID + " is finding water");
    GameObject closestWater = possibleWater[0];
    float distance = 0;
    foreach (GameObject f in possibleWater) {
      float getDistance = Vector3.Distance(f.transform.position, this.transform.position);
      if (distance > getDistance) {
        closestWater = f;
        distance = getDistance;
      }
      else {
        continue;
      }
    }
    Vector3 newVec = new Vector3 (closestWater.transform.position.x, closestWater.transform.position.y + 1, closestWater.transform.position.z);
    //p.GoFindPath(this.transform.position, closestWater.transform.position, this);
    }
  public void FindFood() {
    Debug.Log(creatureID + " is finding food");
    GameObject closestFood = possibleFood[0];
    float distance = 0;
    foreach (GameObject f in possibleFood) {
      float getDistance = Vector3.Distance(f.transform.position, this.transform.position);
      if (distance > getDistance) {
        closestFood = f;
        distance = getDistance;
      }
      else {
        continue;
      }
    }
    //p.GoFindPath(this.transform.position, closestFood.transform.position, this);
  }
  public void FindMate() {
    Debug.Log(creatureID + " is finding mate");
    if (isMale == true) {
      List<GameObject> possible = new List<GameObject>();
      possible = possibleMates;
      foreach (GameObject c in possible) {
        if (c.GetComponent<Creature>().lookMate || c.GetComponent<Creature>().isPregnet) {
          possible.Remove(c);
        }
      }

    int i = Random.Range(0, possible.Count + 1);
    PossibleMateFound(possible[i]);
    if (currentMate.GetComponent<Creature>() != null) {
    Debug.Log(currentMate.transform.position);
    //p.GoFindPath(this.transform.position, currentMate.transform.position, this);
    }
  }
}

  public void OnTriggerEnter (Collider col) {
    if (col.gameObject.name == "Food") {
      if (!possibleFood.Contains(col.gameObject)) {
        possibleFood.Add(col.gameObject);
      }
    }
    if (col.gameObject.name == "Sand") {
      if (!possibleWater.Contains(col.gameObject)) {
        possibleWater.Add(col.gameObject);
      }
    }
    if (isMature) {
      if (col.gameObject.GetComponent<Creature>().isFemale == true && this.isMale == true) {
        if (!possibleMates.Contains(col.gameObject)) {
        possibleMates.Add(col.gameObject);
        Debug.Log("FoundPossibleMate");
          }
        }
      }
    }
    public void OnTriggerExit(Collider col) {
    if (col.gameObject.name == "Food") {
      if (possibleFood.Contains(col.gameObject)) {
        possibleFood.Remove(col.gameObject);
      }
    }
    if (col.gameObject.name == "Water") {
      if (possibleWater.Contains(col.gameObject)) {
        possibleWater.Remove(col.gameObject);
      }
    }
    }
  //Requests possible mates
  public void PossibleMateFound(GameObject Female) {
    if (Female.GetComponent<Creature>().isMature == true && !rejectedMates.Contains(Female) && currentMate != Female) {
    bool isMate = RequestMate(this.GetComponent<Creature>());

      if(isMate == true) {
        currentMate = Female;
        Female.GetComponent<Creature>().currentMate = this.gameObject;
        Female.GetComponent<Creature>().isPregnet = true;
      }
      else {
        rejectedMates.Add(Female);
      }
    }
  }

  public void GenerateChildren(Creature Male, Creature Female) {
    int cG = (Female.generation <= Male.generation)? Male.generation : Female.generation;
    for (int i = 0; i <= 2; i ++) {
      GameObject child = (GameObject)Instantiate(childPrefab, transform.position = this.transform.position, Quaternion.identity);
      bool gender = (Random.Range(0,2) < 0.5)? true : false;
        if (gender == true) {
          child.GetComponent<Creature>().isMale = true;
          child.GetComponent<Creature>().isFemale = false;
          child.name = "BunnyMale";
        }
        else {
          child.GetComponent<Creature>().isFemale = true;
          child.GetComponent<Creature>().isMale = false;
          child.name = "BunnyFemale";
        }

      //Randomply gives child genes
        child.GetComponent<Creature>().maximumLifeExpectancy = GetGene (Female.maximumLifeExpectancy, Male.maximumLifeExpectancy);
        child.GetComponent<Creature>().sexuallyMature = GetGene (Female.sexuallyMature, Male.sexuallyMature) - Female.pregnancyPeriod/5;
        child.GetComponent<Creature>().hungerTilDeath = GetGene (Female.hungerTilDeath, Male.hungerTilDeath);
        child.GetComponent<Creature>().thirstTilDeath = GetGene (Female.thirstTilDeath, Male.thirstTilDeath);
        child.GetComponent<Creature>().speed = GetGene (Female.speed, Male.speed);
        child.GetComponent<Creature>().generation = cG + 1;
        child.GetComponent<Creature>().age = 0;
        if (child.GetComponent<Creature>().isMale == true) {
          child.GetComponent<Creature>().desireability = GetGene (Male.desireabilityStart, Male.desireabilityStart);
        }
        if (child.GetComponent<Creature>().isFemale == true) {
          child.GetComponent<Creature>().pregnancyPeriodStart = GetGene (Female.pregnancyPeriodStart, Female.pregnancyPeriodStart);
          if (child.GetComponent<Creature>().pregnancyPeriodStart < 0) {
            child.GetComponent<Creature>().pregnancyPeriodStart = Female.pregnancyPeriodStart;
          }
          child.GetComponent<Creature>().timeBetweenPregnancy = GetGene (Female.timeBetweenPregnancy, Female.timeBetweenPregnancy);
        }
    }
    Female.isPregnet = false;
    Female.GetComponent<Creature>().currentMate = null;
    Male.GetComponent<Creature>().currentMate = null;
  }

  // Kills creature
  public void Death(int creatureID) {
    Debug.Log("Oh No! Creature (" + generation + ") " + creatureID + " has died!");
    Destroy(this.gameObject);
  }

  // Get Get the Genes for Children
  public float GetGene (float Female, float Male) {
    float Gene = (Mathf.RoundToInt(Random.Range(0,2)) < 0.5)? Female : Male;
    if (Random.Range(0,2) < mutationChance) {
      Gene += Random.Range(-mutationChange,mutationChange);
    }
    return Gene;
  }

  public float GetRandomAction(float option_a, float option_b) {
    float chance = (Mathf.RoundToInt(Random.Range(0,2)) < 0.5)? option_a : option_b;
    return chance;
  }
  public bool RequestMate(Creature Male) {
      float chance = Random.Range(0, 101);
      if (chance > Male.desireability) {
        return false;
      }
      else {
        return true;
      }
    }
  }
