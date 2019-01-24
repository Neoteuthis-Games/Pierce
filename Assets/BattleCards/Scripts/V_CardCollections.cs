using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
///      GameManager script for "BattleCards: CCG Adventure Template"
/// 
/// "This component stores all the cards in the game."
/// 
/// To clarify some comments, here are the reference for the TERMS:
/// 
///    Our    - Refers to this instance/GameObject (not the main player).
///    Player - Refers to the player.
///    
///            Added: 2016
/// </summary>

public class V_CardCollections : MonoBehaviour {

	[Header("All cards in the game:")]
	public V_Card[] gameCards;

	[Header("UI:")]
	public GameObject cardsListContent;
	public GameObject cardPresenter;

	public static V_Card[] cards;

	void Start(){
		cards = gameCards;
	}
}
