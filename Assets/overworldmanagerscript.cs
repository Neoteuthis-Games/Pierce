using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class overworldmanagerscript : MonoBehaviour
{
    public Text locationText;
    public enum LevelType {Dungeon, Duel, Shop, Story};
    //get what the button pressed will load in and preview it to the player.
    public static string LevelName;
    public static LevelType levelToEnter; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        locationText.text = V_GameManager.CurrentLocation;
    }
    public void ActivateArea()
    {
        //ok so for this, activating an area will cause its story to pop up the first time, we need an option to replay story clips.
        //then, there will be a menu akin to the deck selection.
        //this will be the events. once set up, it will play story, then either go into a card game, dungeon, or shop.
        Debug.Log("click");
    }
}
