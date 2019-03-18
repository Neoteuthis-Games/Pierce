using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Gamestate { Menu, Overworld, InGame, Gameover, Deckbuilding, Shop, Paused, Story, Dungeon }; //general
    public static Gamestate ActiveState;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
