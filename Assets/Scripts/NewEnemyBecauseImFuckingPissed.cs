using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NewEnemyBecauseImFuckingPissed : MonoBehaviour {

    [Header ("Creature Values")]
    public bool isMale;
    public bool isFemale;
    public int creatureID;
    public int generation = 1;

    [Header ("State")]
    public float hunger;
    public float thirst;
    public float reproductionUrge;

    [Header("Increase Rates")]
    public float hungerRate = 1f;
    public float thirstRate = 0.5f;
    public float repRate = 0.3f;

    [Header("Minutes")]
    public float maximumLifeExpectancy = 120;
    public float currentLifeExpectancy;

    [Header("Percentage")]
    public float sexuallyMature = 10;

    [Header("Current Action")]
    public bool lookFood = false;
    public bool lookWater = false;
    public bool isMature = false;
    public bool lookNull = false;
    public bool lookingForMate = false;
    public bool FoundPossibleMate = false;
    public float initialSecondBetweenActions;
    public float secondsBetweenActions;
    public bool isAction = false;

    [Header("Predator Components")]
    public bool foundPrey = false;
    public GameObject currentPrey;

    [Header("Day and Night")]
    public NightAndDay nd;

    [Header("Settings")]
    public float speed = 1;
    public float maxSensorRadius = 9;
    public float hungerTilDeath = 20;
    public float thirstTilDeath = 20;
    public float timeToMaturity = 10;

    [Header("Current Objects")]
    public GameObject currentMate;
    public GameObject currentWater;

    [Header("Genetics")]
    public float mutationChance;
    public float mutationChange;
    public float TilInfluence;
    public float SpeInfluence;
    public float SenInfluence;
    public float SexInfluence;

    [Header("Male Components")]
    public float desireability = 100;
    private float desireabilityStart;
    public List<GameObject> rejectedMates = new List<GameObject>();

    [Header("Female Components")]
    public GameObject child;
    public GameObject childPrefab;
    public bool isPregnet;
    public float pregnancyPeriod = 15;
    public float pregnancyPeriodStart = 15;
    public float maxLitterSize = 6;

    [Header ("Lists")]
    public List<GameObject> possibleWater = new List<GameObject>();
    public List<GameObject> possibleMates = new List<GameObject>();

    [Header ("Components")]
    public SphereCollider sensor;
    private NewEnemyBecauseImFuckingPissed CM;
    public MapGenerator m;
    public Pathfinding p;
    public FoodGenerator FG;
    public List<Coord> MovementPath = new List<Coord>();
    public PopulationGraph pg;
    public GraphDataBase gg;

    private int nullBuffer = 0;

    void Awake() {
      PlayerPrefs.DeleteAll();
    }

    void Start() {
      creatureID = PlayerPrefs.GetInt("CreatureCount", 0) + 1;
      PlayerPrefs.SetInt("CreatureCount", creatureID);
      CM = this;
      sensor = this.gameObject.GetComponent<SphereCollider>();
      desireabilityStart = desireability;
      pregnancyPeriod = pregnancyPeriodStart;
      currentMate = null;
      currentWater = null;
      currentPrey = null;
      isPregnet = false;
      isMature = false;
      timeToMaturity = 0;
      currentLifeExpectancy = maximumLifeExpectancy;
      sensor.radius = maxSensorRadius;
    }

    void LateStart() {
      possibleMates = new List<GameObject>();
      possibleWater = new List<GameObject>();
    }

    void LateUpdate() {
      if (MovementPath.Count != 0) {
        float step = speed * Time.deltaTime;
        transform.LookAt(MovementPath[0].currentSpot);
        transform.rotation*=Quaternion.Euler(0, -90, 0);
        transform.position = Vector3.MoveTowards(transform.position, MovementPath[0].currentSpot, step);
        if (transform.position == MovementPath[0].currentSpot){
          MovementPath.Remove(MovementPath[0]);
        }
      }
      else {
        if(isAction) {
          if (lookFood == true && lookNull == false) {
            hunger = 0;
            lookFood = false;
            isAction = false;
            secondsBetweenActions = initialSecondBetweenActions;
            if (this.transform.position == currentPrey.transform.position) {
            Destroy(currentPrey);
          }
            isAction = false;
            secondsBetweenActions = initialSecondBetweenActions;
          }
          if (lookWater == true && lookNull == false) {
            thirst = 0;
            lookWater = false;
            currentWater = null;
            isAction = false;
            secondsBetweenActions = initialSecondBetweenActions;
          }
          if (lookingForMate == true && FoundPossibleMate == true && lookNull == false && isMale) {
            if ((Mathf.RoundToInt(transform.position.x * 10))/10 == (Mathf.RoundToInt(currentMate.transform.position.x * 10))/10 &&  (Mathf.RoundToInt(transform.position.z * 10))/10 == (Mathf.RoundToInt(currentMate.transform.position.z * 10))/10) {
              reproductionUrge = 0;
              lookingForMate = false;
              FoundPossibleMate = false;
              isAction = false;
              secondsBetweenActions = initialSecondBetweenActions;
              currentMate.GetComponent<NewEnemyBecauseImFuckingPissed>().reproductionUrge = 0f;
              currentMate.GetComponent<NewEnemyBecauseImFuckingPissed>().lookingForMate = false;
              currentMate.GetComponent<NewEnemyBecauseImFuckingPissed>().FoundPossibleMate = false;
              currentMate.GetComponent<NewEnemyBecauseImFuckingPissed>().isAction = false;
              currentMate.GetComponent<NewEnemyBecauseImFuckingPissed>().isPregnet = true;
              currentMate.GetComponent<NewEnemyBecauseImFuckingPissed>().secondsBetweenActions = initialSecondBetweenActions;
              currentMate = null;
            }
            else {
              isAction = false;
              currentMate.GetComponent<NewEnemyBecauseImFuckingPissed>().isAction = false;
              secondsBetweenActions = initialSecondBetweenActions;
              currentMate.GetComponent<NewEnemyBecauseImFuckingPissed>().secondsBetweenActions = initialSecondBetweenActions;
            }
          }
          if (lookNull == true && nullBuffer > 10) {
            lookNull = false;
            nullBuffer = 0;
            isAction = false;
            secondsBetweenActions = initialSecondBetweenActions;
          }
          if (lookNull == true && nullBuffer <= 10) {
            nullBuffer += 1;
          }
        }
      }
    }
    public void Update() {
      if (currentPrey == null && foundPrey == true) {
        currentPrey = null;
        foundPrey = false;
      }
      currentLifeExpectancy -= 1 * Time.deltaTime;
      hunger += hungerRate * Time.deltaTime;
      thirst += thirstRate * Time.deltaTime;

      if (thirst > thirstTilDeath || hunger > hungerTilDeath || currentLifeExpectancy <= 0) {
        Death(creatureID);
      }

      if (timeToMaturity < sexuallyMature && isMature == false) {
        timeToMaturity += 1 * Time.deltaTime;
        isMature = false;
      }
      else {isMature = true;}

      if (isMature) {reproductionUrge += repRate * Time.deltaTime;}

      if (secondsBetweenActions >= 0 && isAction == false) {secondsBetweenActions -= 1 * Time.deltaTime;}
      if (secondsBetweenActions < 0 && isAction == false && foundPrey == false) {
      }
      isAction = true;
      foundPrey = true;
      lookFood = true;

      if (isFemale && isPregnet) {pregnancyPeriod -= 1 * Time.deltaTime;}
      if (pregnancyPeriod < 0) {
        GenerateChildren((currentMate != null)? currentMate.GetComponent<NewEnemyBecauseImFuckingPissed>() : this, this);
        pregnancyPeriod = pregnancyPeriodStart;
        isPregnet = false;
        currentMate = null;
      }
      if (foundPrey == true && lookFood == true && Mathf.RoundToInt(transform.position.x) == transform.position.x && Mathf.RoundToInt(transform.position.z) == transform.position.z) {
        if (lookWater == true || lookingForMate == true) {
          MovementPath = null;
        }
      }
      if (foundPrey == true && currentPrey != null && lookFood == true && isAction) {
        isAction = true;
        lookWater = false;
        lookingForMate = false;
        if (MovementPath.Count == 0) {
          preyFound();
        }

      }
    }

  public void DetermingAction() {
    int index = (thirst >= hunger && thirst >= reproductionUrge) ? 1 : 2;

    if (hunger > thirst && hunger > reproductionUrge){
      lookFood = true;
    }
    if (index == 1){
      if (possibleWater.Count == 0) {
        lookNull = true;
        FindNull();
      }
      else {
      lookWater = true;
      FindWater();
      }
    }
    if (index == 2){
      lookingForMate = true;
      FindMate();
      }
    }

  public void preyFound () {
    Vector3 newPos = currentPrey.transform.position;
    p.GoFindPath(this.transform.position, newPos, null, this, false);
  }

  public void FindWater() {
    //Debug.Log(creatureID + " doesn't want to live in the desert any more");
    GameObject closestWater = possibleWater[0];
        float distance = 0;
    foreach (GameObject x in possibleWater) {
      Vector3 newPos = new Vector3(x.transform.position.x, x.transform.position.y + 1, x.transform.position.z);
        float getDistance = Vector3.Distance(newPos, this.transform.position);
        if (distance > getDistance) {
          closestWater = x;
          distance = getDistance;
        }
        else {
          continue;
        }
      }

    currentWater = closestWater;
    Vector3 updatedPos = new Vector3(closestWater.transform.position.x, closestWater.transform.position.y + 1f, closestWater.transform.position.z);
    p.GoFindPath(this.transform.position, updatedPos, null, this, false);
    }
  public void FindMate() {
    //Debug.Log(creatureID + " desire affection with");
    if(currentMate == null) {
      lookNull = true;
      FindNull();
    }
    else {
      if (isMale) {
        p.GoFindPath(this.transform.position, currentMate.transform.position, null, this, false);
        }
      if (isFemale) {
        if (currentMate.GetComponent<NewEnemyBecauseImFuckingPissed>().currentMate == null) {
          currentMate.GetComponent<NewEnemyBecauseImFuckingPissed>().currentMate = this.gameObject;
        }
        }
      }
    }
  public void FindNull() {
    //Debug.Log(creatureID + " is kinda pathetic, it's got to move around");
    Vector3 newPos = DetermineRandomSpot(this, maxSensorRadius);
    lookWater = false;
    p.GoFindPath(this.transform.position, newPos, null, this, false);
  }

  public void OnTriggerEnter (Collider col) {
    if (col.gameObject.tag == "Rabbit") {
      foundPrey = true;
      if (currentPrey != null) {
      currentPrey = (Vector3.Distance(currentPrey.transform.position, this.transform.position) > Vector3.Distance(col.gameObject.transform.position, this.transform.position))? col.gameObject : currentPrey;
      }
      else {
        currentPrey = col.gameObject;
      }
    }
    if (col.gameObject.name == "Sand") {
      if (!possibleWater.Contains(col.gameObject)) {
        possibleWater.Add(col.gameObject);
      }
    }
    if (isMature) {
      if (col.gameObject.GetComponent<NewEnemyBecauseImFuckingPissed>() != null) {
        if (col.gameObject.GetComponent<NewEnemyBecauseImFuckingPissed>().isFemale == true && this.isMale == true && this.lookingForMate) {
          if (this.isMale == true) {
              PossibleMateFound(col.gameObject);
              if (!rejectedMates.Contains(col.gameObject)) {
                FoundPossibleMate = true;
                col.gameObject.GetComponent<NewEnemyBecauseImFuckingPissed>().FoundPossibleMate = true;

                this.currentMate = col.gameObject;
                col.gameObject.GetComponent<NewEnemyBecauseImFuckingPissed>().currentMate = this.gameObject;
              }
            }
          }
        }
      }
    }
    public void OnTriggerExit(Collider col) {
    if (col.gameObject.name == "Sand") {
      if (possibleWater.Contains(col.gameObject)) {
        possibleWater.Remove(col.gameObject);
      }
    }
    if (col.gameObject.tag == "Rabbit" && currentPrey == col.gameObject) {
      foundPrey = false;
      currentPrey = null;
    }
  }
  //Requests possible mates
  public void PossibleMateFound(GameObject Female) {
    if (Female.GetComponent<NewEnemyBecauseImFuckingPissed>().isMature == true && !rejectedMates.Contains(Female) && currentMate != Female) {
    bool isMate = RequestMate(this.GetComponent<NewEnemyBecauseImFuckingPissed>());

    if (isMate != true) {
        rejectedMates.Add(Female);
      }
    }
  }

  public Vector3 DetermineRandomSpot(NewEnemyBecauseImFuckingPissed c, float range) {
    int x = Mathf.RoundToInt(this.transform.position.x + Random.Range(-range, range));
    int z = Mathf.RoundToInt(this.transform.position.z + Random.Range(-range, range));

    Vector3 newPos = new Vector3(x, transform.position.y, z);
    return newPos;
  }

  public Vector3 DetermineOppositeSpot(NewEnemyBecauseImFuckingPissed c, Vector3 cObj, Vector3 eObj, float range) {
    Vector2 rangeOfCreature = new Vector2(eObj.x - cObj.x, eObj.z - cObj.z);
    Vector2 preferedSpot = new Vector2 ((1*rangeOfCreature.x) + cObj.x, (1*rangeOfCreature.y) + cObj.z);

    Vector3 newPos = new Vector3 (preferedSpot.x, this.transform.position.y, preferedSpot.y);
    Debug.Log(newPos);
    return newPos;
  }

  public void GenerateChildren(NewEnemyBecauseImFuckingPissed Male, NewEnemyBecauseImFuckingPissed Female) {
    int cG = (Female.generation <= Male.generation)? Male.generation : Female.generation;
    int litterSize = Mathf.RoundToInt(Random.Range(0, maxLitterSize));
    for (int i = 0; i < litterSize; i ++) {
      GameObject child = (GameObject)Instantiate(childPrefab, transform.position = this.transform.position, Quaternion.identity);
      bool gender = (Random.Range(0,2) < 0.5)? true : false;
        if (gender == true) {
          child.GetComponent<NewEnemyBecauseImFuckingPissed>().isMale = true;
          child.GetComponent<NewEnemyBecauseImFuckingPissed>().isFemale = false;
          child.name = "BunnyMale";
        }
        else {
          child.GetComponent<NewEnemyBecauseImFuckingPissed>().isFemale = true;
          child.GetComponent<NewEnemyBecauseImFuckingPissed>().isMale = false;
          child.name = "BunnyFemale";
        }

      //Randomply gives child genes

      float newSpeed = GetGene (Female.speed, Male.speed) - GetChange(Male.maxSensorRadius, Female.maxSensorRadius, SenInfluence);
      float newSexMaturity = GetGene (Female.sexuallyMature, Male.sexuallyMature) - GetChange(Male.hungerTilDeath, Female.hungerTilDeath, TilInfluence);
      float newLifeExpectancy = GetGene (Female.maximumLifeExpectancy, Male.maximumLifeExpectancy);
      float newTillDeath = GetGene (Female.hungerTilDeath, Male.hungerTilDeath) - GetChange(Male.speed, Female.speed, SpeInfluence);
      float newSensory = GetGene (Female.maxSensorRadius, Male.maxSensorRadius) - GetChange(Male.sexuallyMature, Female.sexuallyMature, SexInfluence);

      child.GetComponent<NewEnemyBecauseImFuckingPissed>().speed = newSpeed;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().sexuallyMature = newSexMaturity;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().hungerTilDeath = newTillDeath;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().thirstTilDeath = newTillDeath;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().maxSensorRadius = newSensory;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().maximumLifeExpectancy = newLifeExpectancy;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().pg = pg;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().m = m;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().p = p;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().FG = FG;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().nd = nd;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().gg = gg;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().SexInfluence = SexInfluence;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().SenInfluence = SenInfluence;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().TilInfluence = TilInfluence;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().SpeInfluence = SpeInfluence;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().mutationChance = mutationChance;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().mutationChange = mutationChange;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().child = childPrefab;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().childPrefab = childPrefab;
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().generation = cG + 1;

      float newDesirability = GetGene (Male.desireability, Male.desireability) - GetChange(-Male.sexuallyMature, -Female.sexuallyMature, SexInfluence);
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().desireability = newDesirability;
      float newPregnancyPeriod = GetGene (Female.pregnancyPeriodStart, Female.pregnancyPeriodStart) - GetChange(-Male.sexuallyMature, -Female.sexuallyMature, SexInfluence);
      child.GetComponent<NewEnemyBecauseImFuckingPissed>().pregnancyPeriodStart = newPregnancyPeriod;
    }
    Female.isPregnet = false;
    this.isAction = false;
    Female.GetComponent<NewEnemyBecauseImFuckingPissed>().currentMate = null;
    Male.GetComponent<NewEnemyBecauseImFuckingPissed>().currentMate = null;
  }

//affecting value
public float GetChange (float valueMale, float valueFemale, float value) {
  return ((((valueMale+valueFemale)/2) / pregnancyPeriodStart) * value);
}
  // Kills creature
  public void Death(int creatureID) {
    pg.curCount -= 1;
    gg.allListObj.Remove(this.gameObject);
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
  public bool RequestMate(NewEnemyBecauseImFuckingPissed Male) {
      float chance = Random.Range(0, 101);
      if (chance > Male.desireability) {
        return false;
      }
      if (this.currentMate != null) {
        return false;
      }
      else {
        return true;
      }
    }
  }
