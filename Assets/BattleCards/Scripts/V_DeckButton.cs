using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///      DeckButton script for "BattleCards: CCG Adventure Template"
/// 
/// "This is used in deck selection before the game."
/// 
/// To clarify some comments, here are the reference for the TERMS:
/// 
///    Our    - Refers to this instance/GameObject (not the main player).
///    Player - Refers to the player.
///    
///            Added: 2018
/// </summary>

public class V_DeckButton : MonoBehaviour {

	public Text deckNameText;
	public int theDeck;
    public static bool campaignEntry = false;
    public static bool arenaEntry = false;
	[HideInInspector] public V_Menu mainMenu;	// Used for accessing the deck editor...

	public void Start(){
		deckNameText.text = mainMenu.deckEdit.decks [theDeck].deckName;
	}
    public void campaign()
    {
        campaignEntry = true;
    }
    public void Click()
    {
        V_PlayerHandler.selectedDeck = theDeck;
        if (campaignEntry == false)
        {
            V_GameManager.ActiveState = V_GameManager.Gamestate.InGame;
            mainMenu.GoTo("InGame");
        }
        else
        {
            V_GameManager.ActiveState = V_GameManager.Gamestate.Overworld;
            mainMenu.GoTo("Overworld_Airam");
        }
    }
}
