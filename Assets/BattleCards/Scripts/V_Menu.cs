using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
///      Menu script for "BattleCards: CCG Adventure Template"
/// 
/// "Just a simple menu script for demo..."
/// 
/// To clarify some comments, here are the reference for the TERMS:
/// 
///    Our    - Refers to this instance/GameObject (not the main player).
///    Player - Refers to the player.
///    
///            Added: 2016
/// </summary>

public class V_Menu : MonoBehaviour {

	[Header("    UI For Deck Editor:")]
	public InputField deckTextInput;
	public Text averageCardCostText;
	public V_DeckEditor deckEdit;

	[Header("    UI For Front Menu:")]
	public V_DeckButton deckButtonPrefab;
	public Transform deckButtonsHandler;

	void Awake(){
        // Set the player to Menu Mode;
        GameManager.ActiveState = GameManager.Gamestate.Menu;
	}
	void Update(){

		// Deck name input field:
		if (!deckTextInput.isFocused) {
			deckTextInput.text = deckEdit.decks [deckEdit.selectedDeck].deckName;
		}

		// "Average card cost in deck" text
		float averageCost = 0;
		for (int i = 0; i < deckEdit.decks [deckEdit.selectedDeck].cards.Length; i++) {
			averageCost += deckEdit.cardDatabase.gameCards [deckEdit.decks [deckEdit.selectedDeck].cards [i]].energyCost;
		}
		averageCost = Mathf.RoundToInt(averageCost / deckEdit.decks [deckEdit.selectedDeck].cards.Length);
		averageCardCostText.text = "Average card cost: " + averageCost;
	}

	/// <summary>
	/// Calls "SaveDeckJson" function of V_DeckEditor.
	/// </summary>
	public void SaveDeck(){
		for (int i = 0; i < deckEdit.decks.Length; i++) {
			deckEdit.SaveDeckJson (i);
		}
	}

	public void SetDeckName(InputField input){
		deckEdit.decks [deckEdit.selectedDeck].deckName = input.text;
	}

	public void RefreshDeckList(){
		// Clear:
		foreach (Transform t in deckButtonsHandler) {
			Destroy (t.gameObject);
		}

		// Populate:
		for (int i = 0; i < deckEdit.decks.Length; i++) {
			V_DeckButton dbtn = Instantiate (deckButtonPrefab, deckButtonsHandler);
			dbtn.mainMenu = this;
			dbtn.theDeck = i;
		}
	}

	public void GoTo(string sceneName){
		SceneManager.LoadScene (sceneName);
	}
}
