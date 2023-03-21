using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDefault : MonoBehaviour
{
    public MapGenerator mp;

    void Start() {
      //position the camera to that the map and current values all fit inside of it
      this.transform.position = new Vector3((mp.width / 2) - 5, (mp.width >= mp.height) ?  mp.width : mp.height, 0);
    }
}
