using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///      CardViewer script for "BattleCards: CCG Adventure Template"
/// 
/// "This allows the player to view cards from the game field."
/// 
/// To clarify some comments, here are the reference for the TERMS:
/// 
///    Our    - Refers to this instance/GameObject (not the main player).
///    Player - Refers to the player.
///    
///            Added: 2018
/// </summary>

public class V_CardViewer : MonoBehaviour {

	public CanvasGroup cg;
	public Transform container;
	public float mouseMoveSensitivity;
	public float fadeSpeed;

	// Internal:
	Vector3 initPos;
	Vector3 curPos;
	bool closed = true;
	V_PlayerHandler player;
	V_AI ai;

	// Use this for initialization
	void Start () {

		// References:
		player = FindObjectOfType<V_PlayerHandler>();
		ai = FindObjectOfType<V_AI> ();

		// Initializations:
		initPos = container.localPosition;
	}
	
	// Update is called once per frame
	void Update () {

		// Do a bit of movement with the mouse (because it's cool)
		curPos = new Vector3 (-Input.GetAxis ("Mouse X")*mouseMoveSensitivity + initPos.x, -Input.GetAxis ("Mouse Y")*mouseMoveSensitivity + initPos.y, 0);
		container.localPosition = Vector3.Lerp (container.localPosition, curPos, Time.deltaTime);

		// Do fade when closing and opening:
		cg.alpha = Mathf.MoveTowards(cg.alpha, closed? 0 : 1, Time.deltaTime * fadeSpeed);
		cg.blocksRaycasts = !closed;
	}

	public void ViewCard(V_Card card){
		// Dispose any viewed card first (if there's any):
		foreach (Transform t in container) {
			Destroy (t.gameObject);
		}
		// ...then instantiate a new one:
		V_Card c = null;
		if (card.tag == "AIOwned") {
			for (int i = 0; i < ai.aiCards.Length; i++) {
				if (card.cardName == ai.aiCards [i].cardName) {
					c = Instantiate (ai.aiCards [i], container);
					print ("Found it!");
					break;
				}
			}
		} else {
			for (int i = 0; i < player.myDeck.Length; i++) {
				if (card.cardName == player.myDeck [i].cardName) {
					c = Instantiate (player.myDeck [i], container);
					print ("Found it!");
					break;
				}
			}
		}
		c.cActions.enabled = false;
		c.enabled = false;
		c.tag = "Untagged";
		closed = false;
	}

	public void Close(){
		closed = true;
	}
}
