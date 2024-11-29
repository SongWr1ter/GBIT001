using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryPanel : MonoBehaviour
{
    public List<string> story = new List<string>();
    private TextPanel[] textPanels;
    bool flag = false;
    private Coroutine typingCoroutine; // 存储协程引用
    // Start is called before the first frame update
    void Start()
    {
        textPanels = transform.GetComponentsInChildren<TextPanel>();
        for (int i = 0; i < textPanels.Length; i++)
        {
            textPanels[i].StartTyping(" ");
        }
    }

    private void Update()
    {
        if (flag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                flag = false;
            }
        }
    }

    public void Skip()
    {
        SoundManager.PlayAudio("TypeUI");
        
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            flag = false;
        }
    }

    IEnumerator sequenceShow()
    {
        for (int i = 0; i < story.Count; i++)
        {
            textPanels[i].StartTyping(story[i]);
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    public void ok()
    {
        typingCoroutine = StartCoroutine(sequenceShow());
    }
}
