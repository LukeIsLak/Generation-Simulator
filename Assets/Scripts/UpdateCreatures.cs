using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateCreatures : MonoBehaviour {
  public Slider ageSl;
  public Slider hungerSl;
  public Slider thirstSl;
  public Slider repUrgeSl;

  public GameObject IDNum;
  public GameObject GenNum;
  public GameObject mOrF;

  public NewCreatureBecauseImFuckingPissed CurrentCreature;
  void Update() {
      if (Input.GetMouseButtonDown(0)) {
          RaycastHit hit;
          Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
          if (Physics.Raycast(ray, out hit)) {
            BoxCollider collider = hit.collider as BoxCollider;
            if (hit.collider.gameObject.GetComponent<NewCreatureBecauseImFuckingPissed>() != null && collider != null) {
              CurrentCreature = hit.collider.gameObject.GetComponent<NewCreatureBecauseImFuckingPissed>();
            }
        }
      }
      if (CurrentCreature != null) {
      ageSl.value = CurrentCreature.currentLifeExpectancy/CurrentCreature.maximumLifeExpectancy;
      hungerSl.value = CurrentCreature.hunger/CurrentCreature.hungerTilDeath;
      thirstSl.value = CurrentCreature.thirst/CurrentCreature.thirstTilDeath;
      repUrgeSl.value = CurrentCreature.reproductionUrge/ ((CurrentCreature.hungerTilDeath+CurrentCreature.thirstTilDeath)/2);

      IDNum.GetComponent<TMPro.TextMeshProUGUI>().text = CurrentCreature.creatureID.ToString();
      GenNum.GetComponent<TMPro.TextMeshProUGUI>().text = CurrentCreature.generation.ToString();
      mOrF.GetComponent<TMPro.TextMeshProUGUI>().text = (CurrentCreature.isMale == true)? "Male" : "Female";
    }
  }
}
