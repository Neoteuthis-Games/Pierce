using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class overworldmanagerscript : MonoBehaviour
{
    public Text locationText;
    [Header("    This Right Here:")]
    public static GameObject MasterScene;
    public GameObject dialoguescene;
    public enum LevelType {Dungeon, Duel, Shop, Story, Item};
    //get what the button pressed will load in and preview it to the player.
    public static string LevelName;
    public static LevelType levelToEnter; 
    // Start is called before the first frame update
    void Start()
    {
        MasterScene = dialoguescene;
    }

    // Update is called once per frame
    void Update()
    {
        locationText.text = V_GameManager.CurrentLocation;
    }
    public static void ActivateArea(string levelname, string leveltag, LevelType leveltype)
    {
        switch (leveltype)
        {
            case LevelType.Duel:
                MasterScene.SetActive(true);
                //finish dialogue
                //load in the custom enemy deck for this encounter. 
                //start encounter
                SceneManager.LoadScene("InGame");
                break;
            case LevelType.Dungeon:
                SceneManager.LoadScene(leveltag);
                break;
            case LevelType.Shop:
               // SceneManager.LoadScene("Shop");
                break;
            case LevelType.Story:
                MasterScene.SetActive(true);
                break;
            case LevelType.Item:
                GameManager.AddToCollection(leveltag);
                //MasterScene.dialog ///i can't remember what I was going to say here, but it needs to say congrats you found X.
                break;
            default:
                break;
                
        }
        //ok so for this, activating an area will cause its story to pop up the first time, we need an option to replay story clips.
        //then, there will be a menu akin to the deck selection.
        //this will be the events. once set up, it will play story, then either go into a card game, dungeon, or shop.
        Debug.Log("click");
    }
}
