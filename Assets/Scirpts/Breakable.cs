using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public string sfxName = "doorBroken";
    public int maxHp = 3;
    public int currentHp;
    protected bool getHurtThisFrame;
    protected Split split;
    [SerializeField]protected float soundRadius;

    protected virtual void Start()
    {
        currentHp = maxHp;
        split = GetComponent<Split>();
    }

    protected virtual void Awake()
    {
        MessageCenter.AddListener(OnGameRestart);
    }

    protected virtual void OnDestroy()
    {
        MessageCenter.RemoveListner(OnGameRestart);
    }

    protected void Update()
    {
        if (getHurtThisFrame)
        {
            getHurtThisFrame = false;
        }
    }

    public virtual void GetHurt(Vector3 murderPoint)
    {
        if (!getHurtThisFrame)
        {
            currentHp--;
            getHurtThisFrame = true;
        }

        if (currentHp == 0)
        {
            gameObject.SetActive(false);
            if(split != null) split.Trigger(murderPoint);
            SoundManager.PlayAudio(sfxName);
            // �ƻ�Ч��+�ƻ���Ч
            HearingPoster.PostVoice(transform.position, soundRadius);
        }
    }

    public virtual void GetHurt(Vector3 murderPoint,Vector3 collidePoint)//?????GetHurt??????,????????????????
    {
        if (!getHurtThisFrame)
        {
            currentHp--;
            getHurtThisFrame = true;
        }

        if (currentHp == 0)
        {
            gameObject.SetActive(false);
            if(split != null) split.Trigger(murderPoint);
            SoundManager.PlayAudio(sfxName);
            HearingPoster.PostVoice(transform.position, soundRadius);
            // �ƻ�Ч��+�ƻ���Ч
        }
    }

    public virtual void OnGameRestart(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        gameObject.SetActive(true);
        currentHp = maxHp;
    }
}