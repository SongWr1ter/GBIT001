using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : MonoBehaviour
{
    public GameObject dogPrefab;
    public Transform makeTransform;

    public void MakeDog()
    {
        SoundManager.PlayAudio("makedog");
        Instantiate(dogPrefab, makeTransform.position, transform.rotation, null).GetComponent<simpleFSM>().paramater.isSummoned = true; // ������ɵ�Stage�Restart�󻹻����
    }
}
