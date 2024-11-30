using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamPatch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<simpleFSM>().paramater.deadBodyPrefab =
            TheShitOfReference.AllShitOfReference.DreamDeadBodyReference;
    }
}
