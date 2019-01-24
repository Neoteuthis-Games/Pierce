using UnityEngine;
using System.Collections;

/// <summary>
///      DeckEditor script for "BattleCards: CCG Adventure Template"
/// 
/// "This script allows the player to customize the decks..."
/// 
/// To clarify some comments, here are the reference for the TERMS:
/// 
///    Our    - Refers to this instance/GameObject (not the main player).
///    Player - Refers to the player.
///    
///            Added: 2016
/// </summary>


public class V_DeckEditor : MonoBehaviour {

	[Header("    Card Presenters:")]
	public V_CardPresenter[] myCards;

	[System.Serializable]
	public class Deck
	{
		public string deckName = "DeckName";
		public int[] cards = new int[0];
	}
	[Header("    Decks:")]
	public Deck[] decks;									// The decks of the player

	[Space]
	public V_CardCollections cardDatabase;

	[Header("    Settings:")]
	public bool allowInfiniteDeckScrolling = false;			// Allow wrapping when scrolling between decks using the "NextDeck()" and "PreviousDeck" functions
	public GameObject cardCollectionList;
	[HideInInspector] public int selectedDeck;				// The deck we're currently viewing
	public static V_CardPresenter selectedCard;				// The card presenter we're currently selecting

	void Start () {
		// load saved decks if there's any, else, make a random one....
		if (PlayerPrefs.HasKey ("deck0")) {
			ReloadDecks ();
		} else {
			for (int i = 0; i < decks.Length; i++) {
				SaveDeckJson (i);
			}
			ReloadDecks ();
		}

		// Card previews for card database (in future updates, an inventory system will be implemented so this will be revised):
		cardDatabase = FindObjectOfType<V_CardCollections>();

		foreach (V_Card card in cardDatabase.gameCards) {
			V_CardPresenter prsntr = Instantiate (cardDatabase.cardPresenter, cardDatabase.cardsListContent.transform).GetComponent<V_CardPresenter>();
			prsntr.index = System.Array.IndexOf (cardDatabase.gameCards, card);
			prsntr.index = cardDatabase.cardsListContent.transform.childCount - 1;
			prsntr.Refresh ();
		}
	}
	
	// LOADING AND SAVING DECKS:
	void LoadDeckJson(int deck){
		decks[deck] = JsonUtility.FromJson <Deck>(PlayerPrefs.GetString("deck" + deck));
		UpdateCardsInDeck (selectedDeck);
		UpdateToPlayer ();
	}
	public void SaveDeckJson(int deck){
		PlayerPrefs.SetString ("deck" + deck, JsonUtility.ToJson (decks [deck]));
		UpdateToPlayer ();
	}

	// REFRESH THE DECKS:
	public void ReloadDecks(){
		for (int i = 0; i < decks.Length; i++) {
			LoadDeckJson (i);
		}
	}

	// CARD SELECTION:
	public void Select(V_CardPresenter card){
		selectedCard = null;
		selectedCard = card;
		cardCollectionList.SetActive (true);
	}
	public void Deselect(){
		selectedCard = null;
		cardCollectionList.SetActive (false);
	}

	// DECK SELECTION (Can be used with uGUI):
	public void NextDeck(){
		selectedDeck++;
		if (selectedDeck >= decks.Length) {
			selectedDeck = allowInfiniteDeckScrolling? 0 : decks.Length - 1;
		}
		// Deselect any card selections:
		Deselect();
		// Update the UI:
		UpdateCardsInDeck (selectedDeck);
	}
	public void PreviousDeck(){
		selectedDeck--;
		if (selectedDeck < 0) {
			selectedDeck = allowInfiniteDeckScrolling? decks.Length - 1 : 0;
		}
		// Deselect any card selections:
		Deselect();
		// Update the UI:
		UpdateCardsInDeck (selectedDeck);
	}

	// OTHER COMMANDS:
	public void SetCard(int deck, int newCardIndex){
		decks[deck].cards[selectedCard.transform.GetSiblingIndex ()] = newCardIndex;
		cardCollectionList.SetActive (false);
		selectedCard = null;
		UpdateCardsInDeck (deck);
		UpdateToPlayer ();
	}
	public void UpdateCardsInDeck (int deck) {
		// update all the card presenters in the deck:
		foreach (V_CardPresenter card in myCards) {
			card.index = decks[deck].cards[System.Array.IndexOf (myCards, card)];
			card.Refresh ();
		}
	}
	public void UpdateToPlayer(){
		V_PlayerHandler.deckData = decks;
	}
}
