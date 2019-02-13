using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnStateCheck : MonoBehaviour
{
    public Text currentphase;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // public enum currentState { begin, recharge, draw, action, war, end }; //incardgame
  switch (V_GameManager.cardgamestate)
        {
            case V_GameManager.currentState.begin:
                currentphase.text = "begin";
                return;
            case V_GameManager.currentState.recharge:
                currentphase.text = "recharge";
                return;
            case V_GameManager.currentState.draw:
                currentphase.text = "draw";
                return;
            case V_GameManager.currentState.action:
                currentphase.text = "action";
                return;
            case V_GameManager.currentState.war:
                currentphase.text = "war";
                return;
            case V_GameManager.currentState.end:
                currentphase.text = "end";
                return;

        }
            
    }
}
