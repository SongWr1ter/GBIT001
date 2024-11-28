using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RetailerUI : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    private CanvasGroup _canvasGroup;

    [SerializeField] private float speedAddAmount;
    [SerializeField] private float ammoAddAmount;
    // Start is called before the first frame update
    void Start()
    {
        buttons = GetComponentsInChildren<Button>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        MessageCenter.AddListener(PopUp,MESSAGE_TYPE.UI_RETAILER);
    }

    private void OnDisable()
    {
        MessageCenter.RemoveListener(PopUp, MESSAGE_TYPE.UI_RETAILER);
    }

    private void PopUp(CommonMessage message)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    private void PopDown()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public void AddAmmo()
    {
        if (TheShitOfReference.extraAmmoAmount < TheShitOfReference.MAX_EXTRA_AMMO)
        {
            TheShitOfReference.extraAmmoAmount += ammoAddAmount;
        }
        Exit();
    }

    public void AddSpeed()
    {
        if (TheShitOfReference.extraSpeed < TheShitOfReference.MAX_EXTRA_SPEED)
        {
            TheShitOfReference.extraSpeed += speedAddAmount;
        }
        Exit();
    }

    public void AddTimeEnergy()
    {
        if(TimeScaleChange.MAX_TIME_ENERGY < 200f)
            TimeScaleChange.MAX_TIME_ENERGY += 20f;
        Exit();
    }

    public void Exit()
    {
        PopDown();
    }
}
