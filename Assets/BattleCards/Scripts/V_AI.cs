using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
///        AI script for "BattleCards: CCG Adventure Template"
/// 
/// "This script handles all the AI's stats and behaviours"
/// 
/// To clarify some comments, here are the reference for the TERMS:
/// 
///    Our    - Refers to this instance/GameObject (not the main player).
///    Player - Refers to the player.
///    
///            Added: 2016
/// </summary>


public class V_AI : MonoBehaviour {

	public static int health = 30;
	public static int energy = 0;
	[Header("Add cards here!")]
	public V_Card[] aiCards;			// This act as the AI's deck (like the "myDeck" array in V_PlayerHandler)
	[Header("    Delay Per Move:")]
	public float moveDelay = 1f;

	// Internal:
	private float curDelay = 0;
	private V_GameManager gm;
	private int i = 0;

	// Use this for initialization
	void Start () {
		gm = GameObject.FindObjectOfType <V_GameManager>();
	}
	
	// Update is called once per frame
	void Update () {

		// Clamp health:
		health = Mathf.Clamp(health, 0, gm.maxHealth);

		if (V_GameManager.playerTurn == V_GameManager.playerTypes.AI) {
			curDelay += Time.deltaTime;
			if (curDelay >= moveDelay) {
				if (energy > 0) {
					AITurn ();
					curDelay = 0;
				} else {
					AITurn ();
					curDelay = 0;
				}
			}
		}
	}

	/// <summary>
	///                 AITurn Function: The main BRAIN
	/// This function is where the AI's decisions are made. It holds the logics
	/// which make the AI decide its next move. This is tailored to the current setup
	/// so if something's changed, it might affect the AI's performance... 
	///                "Modify at your own RISK!"
	/// </summary>
	public void AITurn(){
		if (!gm.isGameOver) {
			//Set all the variables needed:
			bool attack = Random.Range(0, 6) < 2? true : false;
			V_Card chosenCard = aiCards [Random.Range (0, aiCards.Length)];
			GameObject[] card = GameObject.FindGameObjectsWithTag ("AIOwned");
			GameObject[] target = GameObject.FindGameObjectsWithTag ("PlayerOwned");

			// Logics!
			if (energy <= 0) {
				Debug.Log ("AI plans to attack our base!");
				if (card.Length >= 1) {
					if (card.Length > 0) {
						Debug.Log ("AI will find some enemies...");
						for (int n = 0; n < card.Length; n++) {
							if (card [n].GetComponent<V_Card> ().canBeUsedTo == V_Card.usage.All || card [n].GetComponent<V_Card> ().canBeUsedTo == V_Card.usage.BaseOnly) {
								if (card [n].GetComponent<V_CardActions> ().isUsed == false) {
									Debug.Log ("Can't find enemies so " + card [n].name + " will attack the player base.");
									AttackPlayer (card [n].GetComponent<V_Card> ());
									return;
								}
							} else {
								if (target.Length >= 1) {
									Debug.Log ("AI found some enemies so " + card [n].name + " will attack one of player's cards!");
									UseACard (card [n].GetComponent<V_CardActions> (), target [Random.Range (0, target.Length)].GetComponent<V_Card> ());
									return;
								} else {
									if (n >= card.Length) {
										Debug.Log ("Finished!");
										AIEndTurn ();
									} else {
									
									}
								}
							}
						}
						AIEndTurn ();
					}
				} else {
					Debug.Log ("2");
					AIEndTurn ();
				}
			} else {
				// If the AI chose to deploy a card than to attack:
				if (!attack) {
					// ... and when the AI has enough energy to place the card and the Battle Zone still has slot, do:
					if (V_GameManager.aiBattleZone.transform.childCount < 6 && chosenCard.energyCost <= energy) {
						PlaceCard (chosenCard);
						energy -= chosenCard.energyCost;
						return;
					} 
				/// But if there's no more room for cards then check if our Battle Zone is
				/// not empty and if there are cards in the player's Battle Zone:
				else {
						// If we have any cards in our battleground that we can use then do the following...
						if (card.Length > 0) {
							// if there are cards on the opponent's battleground, attack one of them...
							int index = Random.Range (0, card.Length);
							if (target.Length > 0 && card [index].GetComponent<V_CardActions> ().isUsed == false) {
								if (card [index].GetComponent<V_Card> ().canBeUsedTo == V_Card.usage.All || card [index].GetComponent<V_Card> ().canBeUsedTo == V_Card.usage.CardsOnly)
									UseACard (card [index].GetComponent<V_CardActions> (), target [Random.Range (0, target.Length)].GetComponent<V_Card> ());
								Debug.Log ("AI is attacking one of our cards!");
								return;
							}
						//...but if there is none, then attack the opponent's base instead:
						else {
								if (i <= card.Length) {
									i += 1;
									GameObject cc = card [Random.Range (0, card.Length)];
									if (cc.GetComponent<V_Card> ().canBeUsedTo == V_Card.usage.All || cc.GetComponent<V_Card> ().canBeUsedTo == V_Card.usage.BaseOnly) {
										if (cc.GetComponent<V_CardActions> ().isUsed == false) {
											AttackPlayer (cc.GetComponent<V_Card> ());
											return;
										} else {
											AIEndTurn ();
										}

									} else {
										Debug.Log ("The AI is attempting to find a card to be used against us...");
										AITurn ();
									}
								} else {
									// ...else, end the turn:
									AIEndTurn ();
								}
							}
						}
					// ...but if we don't have one then just end our turn, or deploy a card if we still have some energy:
					else {
							//checking if we still have some energy...
							if (energy <= 0) {
								AIEndTurn ();
							} else {
								AITurn ();
							}
						}
					}
				}
			// ...but if it's the empty slot, use random deployed card:
			else {
					// Check if we already have cards deployed in our Battle Zone, proceed if we have, restart move if we don't:
					if (card.Length > 0) {
						int index = Random.Range (0, card.Length);
						if (target.Length > 0 && card [index].GetComponent<V_CardActions> ().isUsed == false) {
							if (card [index].GetComponent<V_Card> ().canBeUsedTo == V_Card.usage.All || card [index].GetComponent<V_Card> ().canBeUsedTo == V_Card.usage.CardsOnly)
								UseACard (card [index].GetComponent<V_CardActions> (), target [Random.Range (0, target.Length)].GetComponent<V_Card> ());
							Debug.Log ("AI is attacking one of our cards!");
							return;
						} else {
							AITurn ();
						}
					} else {
						AITurn ();
					}
				}
			}
		}
	}

	public void AIEndTurn(){
		gm.ChangeTurn (V_GameManager.playerTypes.AI);
		gm.curSelected = null;
		gm.aiCurSelected = null;
		Debug.Log ("AI is ending his turn...");
	}

	public void AttackPlayer(V_Card card){
		gm.aiCurSelected = card;
		gm.AI_AttackEnemyPlayer ();
		Debug.Log ("AI is attacking our base!");
		i = 0;
	}

	public void UseACard(V_CardActions card, V_Card target){
		card.Use (target);
	}


	// This function allows the AI to deploy cards:
	public void PlaceCard(V_Card card){
		// If the card is a CREATURE card...
		if (card.type == V_Card.cardType.Creature) {
			GameObject obj = Instantiate (card.gameObject, V_GameManager.aiBattleZone.transform) as GameObject;
			obj.tag = "AIOwned";
		} 
		// ... but if it's a SPELL card:
		if (card.type == V_Card.cardType.Event){
			GameObject obj = Instantiate (card.gameObject, V_GameManager.aiSpellZone.transform) as GameObject;
			obj.tag = "AIOwned";

			// and because it's a spell card, do it's effect:
			obj.GetComponent<V_Card> ().DoEffectAI ();
		}
	}


	// RECIEVE EFFECTS (called by other scripts):
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
	public static void EffectAddEnergy (int value){
		energy += value;
	}
}
