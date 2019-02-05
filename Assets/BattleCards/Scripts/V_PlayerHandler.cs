using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

/// <summary>
///      PlayerHandler script for "BattleCards: CCG Adventure Template"
/// 
/// "This is the main script for the player controller object. Only one instance of
/// this component per scene is allowed."
/// 
/// To clarify some comments, here are the reference for the TERMS:
/// 
///    Our    - Refers to this instance/GameObject (not the main player).
///    Player - Refers to the player.
///    
///            Added: 2016
/// </summary>

public class V_PlayerHandler : MonoBehaviour {

	[HideInInspector] public V_Card[] myDeck;		// Holds the physical reference of card prefabs to be instantiated in game.

	public static int selectedDeck;
	public static V_DeckEditor.Deck[] deckData;

	// The following variables are to be managed by the V_GameManager script...
	public static int health = 0;
	public static int energy = 0;
	public static bool isInGame = false;

	// Internal:
	[HideInInspector] public V_GameManager gm;
	V_CardCollections dtbase;

	void Awake(){
		DontDestroyOnLoad (gameObject);
	}

	// Update is called once per frame
	void Update () {
		// *When in main menu:
		if (!isInGame) {
			// Find the card collections in main menu if we haven't cached it yet:
			if (!dtbase) {
				dtbase = GameObject.FindObjectOfType <V_CardCollections> ();
			}

			// Load cards in deck:
			if (deckData != null) {
				myDeck = new V_Card[deckData [selectedDeck].cards.Length];
				for (int i = 0; i < deckData[selectedDeck].cards.Length; i++) {
					myDeck [i] = dtbase.gameCards [deckData [selectedDeck].cards[i]];
				}
			}
		} 
		// *When in game:
		else {
			if (!gm) {
				gm = FindObjectOfType<V_GameManager> ();
				return;
			}

			// Clamp health:
			health = Mathf.Clamp(health, 0, gm.maxHealth);
		}
	}

	public void EndTurn(){
		gm.ChangeTurn (V_GameManager.playerTypes.Player);
		gm.curSelected = null;
		gm.aiCurSelected = null;
	}

	public void ReDraw(){
		if (energy >= gm.drawCost && V_GameManager.playerTurn == V_GameManager.playerTypes.Player) {
			V_Card[] picks = new V_Card[] {
				myDeck [Random.Range (0, myDeck.Length)],
				myDeck [Random.Range (0, myDeck.Length)],
				myDeck [Random.Range (0, myDeck.Length)],
				myDeck [Random.Range (0, myDeck.Length)]
			};
			gm.Redraw (picks);
			energy -= gm.drawCost;
		} else {
			// Display error:
			gm.DisplayError (gm.noEnergy);
		}
	}

	public void StartDraw(){
		if (V_GameManager.playerTurn == V_GameManager.playerTypes.Player) { 
			V_Card[] picks = new V_Card[] {
				myDeck [Random.Range (0, myDeck.Length)],
				myDeck [Random.Range (0, myDeck.Length)],
				myDeck [Random.Range (0, myDeck.Length)],
                myDeck [Random.Range (0, myDeck.Length)],
                myDeck [Random.Range (0, myDeck.Length)]
			};
			gm.Redraw (picks);
		}
        V_GameManager.initialsetup = false;
	}

	public void DrawOneCard(){
		V_Card[] picks = new V_Card[] { myDeck [Random.Range (0, myDeck.Length)] };
		gm.DrawACard (picks);
	}

	// RECEIVE EFFECTS (called by other scripts):
	public static void AddEnergy(int value){
		energy += value;
	}
	public static void EffectDamage (int value, GameObject effect, Transform parentZone){
		health -= value;
		GameObject obj = Instantiate (effect, parentZone) as GameObject;
		obj.GetComponent<Text>().text = "-" + value.ToString();
	}
	public static void EffectHeal (int value, GameObject effect, Transform parentZone){
		health += value;
		GameObject obj = Instantiate (effect, parentZone) as GameObject;
		obj.GetComponent<Text>().text = "+" + value.ToString();
	}
}