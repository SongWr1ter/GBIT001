using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetailerGO : MonoBehaviour
{
    private bool _isLevelClear;
    private GameObject _pointer;

    private void OnEnable()
    {
        MessageCenter.AddListener(OnLevelClear);
        _pointer = transform.GetChild(0).gameObject;
    }

    private void OnDisable()
    {
        MessageCenter.RemoveListner(OnLevelClear);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isLevelClear)
            {
                MessageCenter.SendMessage(new CommonMessage(),MESSAGE_TYPE.UI_RETAILER);
                _isLevelClear = false;
                _pointer.SetActive(false);
            }
        }
    }

    private void OnLevelClear(CommonMessage message)
    {
        if(message.Mid != (int)MESSAGE_TYPE.LEVEL_CLEAR) return;
        _isLevelClear = true;
        _pointer.SetActive(true);
    }
}
