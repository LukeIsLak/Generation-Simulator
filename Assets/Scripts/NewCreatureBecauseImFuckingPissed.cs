using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NewCreatureBecauseImFuckingPissed : MonoBehaviour {

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
    public bool isBeingHunted = false;
    public GameObject currentEnemy;

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
    public GameObject currentFood;

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
    public List<GameObject> possibleFood = new List<GameObject>();
    public List<GameObject> possibleWater = new List<GameObject>();
    public List<GameObject> possibleMates = new List<GameObject>();

    [Header ("Components")]
    public SphereCollider sensor;
    private NewCreatureBecauseImFuckingPissed CM;
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
      pg.curCount += 1;
      creatureID = PlayerPrefs.GetInt("CreatureCount", 0) + 1;
      PlayerPrefs.SetInt("CreatureCount", creatureID);
      CM = this;
      sensor = this.gameObject.GetComponent<SphereCollider>();
      desireabilityStart = desireability;
      pregnancyPeriod = pregnancyPeriodStart;
      currentMate = null;
      currentWater = null;
      currentFood = null;
      isPregnet = false;
      isMature = false;
      timeToMaturity = 0;
      currentLifeExpectancy = maximumLifeExpectancy;
      sensor.radius = maxSensorRadius;
      gg.allListObj.Add(this.gameObject);
    }

    void LateStart() {
      possibleFood = new List<GameObject>();
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
            bool test = false;
            float prevHunger = hunger;
            if (currentFood != null) {
              test = true;
              possibleFood.Remove(currentFood);
              Destroy(currentFood);
            }
            hunger = 0;
            lookFood = false;
            currentFood = null;
            isAction = false;
            secondsBetweenActions = initialSecondBetweenActions;
            if (test == false) {hunger = prevHunger;}

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
              currentMate.GetComponent<NewCreatureBecauseImFuckingPissed>().reproductionUrge = 0f;
              currentMate.GetComponent<NewCreatureBecauseImFuckingPissed>().lookingForMate = false;
              currentMate.GetComponent<NewCreatureBecauseImFuckingPissed>().FoundPossibleMate = false;
              currentMate.GetComponent<NewCreatureBecauseImFuckingPissed>().isAction = false;
              currentMate.GetComponent<NewCreatureBecauseImFuckingPissed>().isPregnet = true;
              currentMate.GetComponent<NewCreatureBecauseImFuckingPissed>().secondsBetweenActions = initialSecondBetweenActions;
              currentMate = null;
            }
            else {
              isAction = false;
              currentMate.GetComponent<NewCreatureBecauseImFuckingPissed>().isAction = false;
              secondsBetweenActions = initialSecondBetweenActions;
              currentMate.GetComponent<NewCreatureBecauseImFuckingPissed>().secondsBetweenActions = initialSecondBetweenActions;
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
      if (secondsBetweenActions < 0 && isAction == false) {
        isAction = true;
        DetermingAction();
      }

      if (isFemale && isPregnet) {pregnancyPeriod -= 1 * Time.deltaTime;}
      if (pregnancyPeriod < 0) {
        GenerateChildren((currentMate != null)? currentMate.GetComponent<NewCreatureBecauseImFuckingPissed>() : this, this);
        pregnancyPeriod = pregnancyPeriodStart;
        isPregnet = false;
        currentMate = null;
      }
      if (isBeingHunted == true && Mathf.RoundToInt(transform.position.x) == transform.position.x && Mathf.RoundToInt(transform.position.z) == transform.position.z) {
        if (lookFood == true || lookWater == true || lookingForMate == true) {
          MovementPath = null;
        }
      }
      if (isBeingHunted == true && currentEnemy != null) {
        isAction = true;
        lookFood = false;
        lookWater = false;
        lookingForMate = false;
        if (MovementPath.Count == 0) {
          isHunted();
        }

      }
    }

  public void DetermingAction() {
    int index = (hunger > thirst && hunger > reproductionUrge) ? 0 : (thirst > hunger && thirst > reproductionUrge) ? 1 : 2;

    if (index == 0){
      if (possibleFood.Count == 0) {
        lookNull = true;
        FindNull();
      }
      else {
      lookFood = true;
      FindFood();
      }
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

  public void isHunted () {
    //Debug.Log(creatureID + " is very scared right now");
    currentFood = null;
    currentWater = null;
    Vector3 newPos = DetermineOppositeSpot(this, this.transform.position, currentEnemy.transform.position, maxSensorRadius);
    p.GoFindPath(this.transform.position, newPos, this, null, true);
  }
  public void FindFood() {
    //Debug.Log(creatureID + " is finding munchies");
    GameObject closestFood = possibleFood[0];
        float distance = 0;
    foreach (GameObject x in possibleFood) {
        float getDistance = Vector3.Distance(x.transform.position, this.transform.position);
        if (distance > getDistance && x.GetComponent<Food>().isTaken == false) {
          closestFood = x;
          distance = getDistance;
        }
        else {
          continue;
        }
        if (x == null) {
          possibleFood.Remove(x);
        }
      }
    currentFood = closestFood;
    p.GoFindPath(this.transform.position, currentFood.transform.position, this, null, false);
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
    p.GoFindPath(this.transform.position, updatedPos, this, null, false);
    }
  public void FindMate() {
    //Debug.Log(creatureID + " desire affection with");
    if(currentMate == null) {
      lookNull = true;
      FindNull();
    }
    else {
      if (isMale) {
        p.GoFindPath(this.transform.position, currentMate.transform.position, this, null, false);
        }
      if (isFemale) {
        if (currentMate.GetComponent<NewCreatureBecauseImFuckingPissed>().currentMate == null) {
          currentMate.GetComponent<NewCreatureBecauseImFuckingPissed>().currentMate = this.gameObject;
        }
        }
      }
    }

  public void FindNull() {
    //Debug.Log(creatureID + " is kinda pathetic, it's got to move around");
    Vector3 newPos = DetermineRandomSpot(this, maxSensorRadius);
    lookWater = false;
    p.GoFindPath(this.transform.position, newPos, this, null, false);
  }
  //Requests possible mates
  public void PossibleMateFound(GameObject Female) {
    if (Female.GetComponent<NewCreatureBecauseImFuckingPissed>().isMature == true && !rejectedMates.Contains(Female) && currentMate != Female) {
    bool isMate = RequestMate(this.GetComponent<NewCreatureBecauseImFuckingPissed>());

    if (isMate != true) {
        rejectedMates.Add(Female);
      }
    }
  }

  public Vector3 DetermineRandomSpot(NewCreatureBecauseImFuckingPissed c, float range) {
    int x = Mathf.RoundToInt(this.transform.position.x + Random.Range(-range, range));
    int z = Mathf.RoundToInt(this.transform.position.z + Random.Range(-range, range));

    Vector3 newPos = new Vector3(x, transform.position.y, z);
    return newPos;
  }

  public Vector3 DetermineOppositeSpot(NewCreatureBecauseImFuckingPissed c, Vector3 cObj, Vector3 eObj, float range) {
    Vector2 rangeOfCreature = new Vector2(Mathf.RoundToInt(eObj.x) - Mathf.RoundToInt(cObj.x), Mathf.RoundToInt(eObj.z) - Mathf.RoundToInt(cObj.z));
    Vector2 preferedSpot = new Vector2 ((Mathf.Sign(rangeOfCreature.x)) * ((rangeOfCreature.x == 0)? 0 : -1) + cObj.x, (Mathf.Sign(rangeOfCreature.y)) * ((rangeOfCreature.y == 0)? 0 : -1) + cObj.z);

    Vector3 newPos = new Vector3 (preferedSpot.x, this.transform.position.y, preferedSpot.y);
    return newPos;
  }

  public void GenerateChildren(NewCreatureBecauseImFuckingPissed Male, NewCreatureBecauseImFuckingPissed Female) {
    int cG = (Female.generation <= Male.generation)? Male.generation : Female.generation;
    int litterSize = Mathf.RoundToInt(Random.Range(0, maxLitterSize));
    for (int i = 0; i < litterSize; i ++) {
      GameObject child = (GameObject)Instantiate(childPrefab, transform.position = this.transform.position, Quaternion.identity);
      bool gender = (Random.Range(0,2) < 0.5)? true : false;
        if (gender == true) {
          child.GetComponent<NewCreatureBecauseImFuckingPissed>().isMale = true;
          child.GetComponent<NewCreatureBecauseImFuckingPissed>().isFemale = false;
          child.name = "BunnyMale";
        }
        else {
          child.GetComponent<NewCreatureBecauseImFuckingPissed>().isFemale = true;
          child.GetComponent<NewCreatureBecauseImFuckingPissed>().isMale = false;
          child.name = "BunnyFemale";
        }

      //Randomply gives child genes

      float newSpeed = GetGene (Female.speed, Male.speed) - GetChange(Male.maxSensorRadius, Female.maxSensorRadius, SenInfluence);
      float newSexMaturity = GetGene (Female.sexuallyMature, Male.sexuallyMature) - GetChange(Male.hungerTilDeath, Female.hungerTilDeath, TilInfluence);
      float newLifeExpectancy = GetGene (Female.maximumLifeExpectancy, Male.maximumLifeExpectancy);
      float newTillDeath = GetGene (Female.hungerTilDeath, Male.hungerTilDeath) - GetChange(Male.speed, Female.speed, SpeInfluence);
      float newSensory = GetGene (Female.maxSensorRadius, Male.maxSensorRadius) - GetChange(Male.sexuallyMature, Female.sexuallyMature, SexInfluence);

      child.GetComponent<NewCreatureBecauseImFuckingPissed>().speed = newSpeed;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().sexuallyMature = newSexMaturity;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().hungerTilDeath = newTillDeath;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().thirstTilDeath = newTillDeath;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().maxSensorRadius = newSensory;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().maximumLifeExpectancy = newLifeExpectancy;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().pg = pg;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().m = m;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().p = p;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().FG = FG;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().nd = nd;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().gg = gg;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().SexInfluence = SexInfluence;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().SenInfluence = SenInfluence;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().TilInfluence = TilInfluence;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().SpeInfluence = SpeInfluence;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().mutationChance = mutationChance;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().mutationChange = mutationChange;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().child = childPrefab;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().childPrefab = childPrefab;
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().generation = cG + 1;

      float newDesirability = GetGene (Male.desireability, Male.desireability) - GetChange(-Male.sexuallyMature, -Female.sexuallyMature, SexInfluence);
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().desireability = newDesirability;
      float newPregnancyPeriod = GetGene (Female.pregnancyPeriodStart, Female.pregnancyPeriodStart) - GetChange(-Male.sexuallyMature, -Female.sexuallyMature, SexInfluence);
      child.GetComponent<NewCreatureBecauseImFuckingPissed>().pregnancyPeriodStart = newPregnancyPeriod;
    }
    Female.isPregnet = false;
    this.isAction = false;
    Female.GetComponent<NewCreatureBecauseImFuckingPissed>().currentMate = null;
    Male.GetComponent<NewCreatureBecauseImFuckingPissed>().currentMate = null;
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
  public bool RequestMate(NewCreatureBecauseImFuckingPissed Male) {
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
