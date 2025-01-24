using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTrigger : MonoBehaviour
{
    enum ActionType{//one type <=> one action <=> reverse
        Activate,
        Animation
    }
    public List<GameObject> list;
    [SerializeField]private StageID stageID;
    [SerializeField]private ActionType actionType;
    private bool expired = false;
    
    private void Awake() {
        InitList();
        if(actionType == ActionType.Activate){
            BeforeActivate();
        }
        
        MessageCenter.AddListener(OnGameRestart);

        StageObject stage = GetComponentInParent<StageObject>();
        if(stage != null) stageID = stage.myStageID;
        
        #if UNITY_EDITOR

        #else
            if(TryGetComponent(out SpriteRenderer t)){
            t.enabled = false;
            }
        #endif
    }

    private void OnDestroy() {
        MessageCenter.RemoveListner(OnGameRestart);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            if(!expired){
                Expired();
                Action(actionType);
            }
        }
    }
    //main logic
    private void Action(ActionType type){
        switch (type)
        {
            case ActionType.Activate:
                foreach(var o in list){
                    o.SetActive(true);
                }
                break;
            default:
                break;
        }
    }
    private void Reverse (ActionType type) {
        switch (type)
        {
            case ActionType.Activate:
                foreach(var o in list){
                    o.SetActive(false);
                }
                break;
            default:
                break;
        }
    }

    private void Expired(){
        expired = true;
    }

    private void Refresh(){
        expired = false;
    }

    private void BeforeActivate () {
        foreach(var o in list){
            o.SetActive(false);
        }
    }

    private void InitList(){
        if(list.Count == 0){
            while(transform.childCount > 0){
                Transform t = transform.GetChild(transform.childCount - 1);
                list.Add(t.gameObject);
                t.SetParent(transform.parent);
            }
        }
    }

    //event system
    void OnGameRestart(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.AFTER_RESTART) return;
        if(msg.intParam != (int)stageID) return;
        Refresh();
        //reverse according to action type
        Reverse(actionType);
    }

    //gizmos
    private void OnDrawGizmosSelected() {
        foreach(var o in list){
            Debug.DrawLine(transform.position, o.transform.position,Color.red);  
        }
    }
}
