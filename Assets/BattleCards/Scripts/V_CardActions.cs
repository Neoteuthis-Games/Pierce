using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
///      CardActions script for "BattleCards: CCG Adventure Template"
/// 
/// "This handles all of THIS card's actions and commands. This script requires
///  the V_Card component in the same GameObject."
/// 
/// To clarify some comments, here are the reference for the TERMS:
/// 
///    Our    - Refers to this instance/GameObject (not the main player).
///    Player - Refers to the player.
///    
///            Added: 2016
/// </summary>

public class V_CardActions : MonoBehaviour {

	[Header("    Card State:")]
	public bool isSelected = false;
	public bool isUsed = false;

	[Header("    Misc:")]
	public Transform curParent;
	public bool isInGame = false;
	public V_GameManager gm;
	[HideInInspector] public V_Card card;

	//private variables:
	private Image Brendrr;
	private Image Srendrr;
    //number of cards allowed in play, we can make public if things modify it.
    private int fieldlimit = 6;
	public bool IsCursorInZone(Vector2 cursor, GameObject zone)
	{
		Vector2 localPos = zone.transform.InverseTransformPoint(cursor);
		return ((RectTransform) zone.transform).rect.Contains(localPos);
	}

	// Use this for initialization
	void Start () {
		// References:
		gm = GameObject.FindObjectOfType<V_GameManager>();
		card = gameObject.GetComponent<V_Card> ();
		if (isInGame) {
			curParent = V_GameManager.handZone.transform;
			Brendrr = V_GameManager.battleZone.GetComponent<Image> ();
			Srendrr = V_GameManager.spellZone.GetComponent<Image> ();
		}
	}

	// Update is called once per frame
	void Update () {
        
		if (isUsed) {
			card.disabledEffect.SetActive (true);
		} else {
			card.disabledEffect.SetActive (false);
		}

		// For Player:
		if (gameObject.tag == "PlayerOwned" || gameObject.tag == "InHand") {
			if (gm.curSelected == card) {
				card.selectedEffect.SetActive (true);
				isSelected = true;
			} else {
				card.selectedEffect.SetActive (false);
				isSelected = false;
			}
		}

		// For AI:
		if (gameObject.tag == "AIOwned") {
			if (gm.aiCurSelected == card) {
				card.selectedEffect.SetActive (true);
				isSelected = true;
			} else {
				card.selectedEffect.SetActive (false);
				isSelected = false;
			}
		}
	}
	
	public void OnDrag(){
		if (isInGame && V_GameManager.playerTurn == V_GameManager.playerTypes.Player && gameObject.tag != "PlayerOwned") {
			//Debug.Log ("Dragging Card...");
			transform.position = Input.mousePosition;
			transform.SetParent (V_GameManager.gameArea.transform);

			// Let's have the zone flashing effect when this card is dragged over a certain zone.

			//       If this card is a CREATURE card and this card is over the Battle Zone:
			if (IsCursorInZone (Input.mousePosition, V_GameManager.battleZone) && card.type == V_Card.cardType.Creature || IsCursorInZone(Input.mousePosition, V_GameManager.battleZone) && card.type == V_Card.cardType.Spell)
            {
				Brendrr.color = Color.cyan;
			}
			//       But if not, then return the Battle Zone's color to default:
			else 
			{
				Brendrr.color = Color.white;
			}

			//       If this card is a SPELL card and this card is over the Spell Zone:
            ///this doesn't need to be seperate. the battlezone is the battlezone...
			//if (IsCursorInZone (Input.mousePosition, V_GameManager.battleZone) && card.type == V_Card.cardType.Spell) {
			//	Brendrr.color = Color.cyan;
			//}else
			////       But if not, then return the Spell Zone's color to default:
			//{
			//	Brendrr.color = Color.white;
			//}
		}
	}
	public void OnDrop(){

		if (!this.enabled) {
			return;
		}

		//On drop, reset the color of the zones:
		Brendrr.color = Color.white;
		Srendrr.color = Color.white;

		//Conditions for card depoyment:
		if (isInGame && V_GameManager.playerTurn == V_GameManager.playerTypes.Player && gameObject.tag != "PlayerOwned") {

			// If this card is a CREATURE card, then do:
            //ok this area we will need to modify ALOT.
			if (card.type == V_Card.cardType.Creature) {
				if (curParent == V_GameManager.handZone.transform) {
					if (IsCursorInZone (Input.mousePosition, V_GameManager.battleZone)) {
						Debug.Log ("It's a Creature!");
						if (card.energyCost <= V_PlayerHandler.energy) {
							if (gm.battleZoneHandler.transform.childCount < fieldlimit) {
								V_PlayerHandler.energy -= card.energyCost;
								transform.SetParent (V_GameManager.battleZone.transform);
								curParent = V_GameManager.battleZone.transform;
								gameObject.tag = "PlayerOwned";
							} else {
								// Dispay error and revert move:
								gm.DisplayError (gm.battleZoneIsFull);
								transform.SetParent (V_GameManager.handZone.transform);
								curParent = V_GameManager.handZone.transform;
							}
						} else {
							GameObject error = Instantiate (gm.errorText, V_GameManager.gameArea.transform) as GameObject;
							error.GetComponent<Text> ().text = gm.noEnergy;
							transform.SetParent (V_GameManager.handZone.transform);
							curParent = V_GameManager.handZone.transform;
						}
					} else {
						if (IsCursorInZone (Input.mousePosition, V_GameManager.spellZone)) {
							// Display error:
							gm.DisplayError (gm.cantPlaceThere);
						}
						transform.SetParent (V_GameManager.handZone.transform);
						curParent = V_GameManager.handZone.transform;
					}
				} else {
					transform.SetParent (V_GameManager.handZone.transform);
					curParent = V_GameManager.handZone.transform;
				}
		    // But if this card is a SPELL card, then do the spell card specific
		    //    actions when dropped:
			} else {
				if (curParent == V_GameManager.handZone.transform) {
					if (IsCursorInZone (Input.mousePosition, V_GameManager.battleZone)) {
						if (card.energyCost <= V_PlayerHandler.energy) {
							V_PlayerHandler.energy -= card.energyCost;
							transform.SetParent (V_GameManager.battleZone.transform);
							curParent = V_GameManager.battleZone.transform;
							gameObject.tag = "PlayerOwned";
							// Do the SPELL effect:
							card.DoEffect ();
						} else {
							GameObject error = Instantiate (gm.errorText, V_GameManager.gameArea.transform) as GameObject;
							error.GetComponent<Text> ().text = gm.noEnergy;
							transform.SetParent (V_GameManager.handZone.transform);
							curParent = V_GameManager.handZone.transform;
						}
					} else {
						if (IsCursorInZone (Input.mousePosition, V_GameManager.spellZone)) {
							// Display error:
							gm.DisplayError (gm.cantPlaceThere);
						}
						transform.SetParent (V_GameManager.handZone.transform);
						curParent = V_GameManager.handZone.transform;
					}
				} else {
					transform.SetParent (V_GameManager.handZone.transform);
					curParent = V_GameManager.handZone.transform;
				}
			}
		}
	}

	// Player card selecting actions:
	public void Select(){
		if (!isUsed && gameObject.tag == "PlayerOwned" && V_GameManager.playerTurn == V_GameManager.playerTypes.Player) {
			if (isSelected) {
				gm.curSelected = null;
				isSelected = false;
			} else {
				isSelected = true;
				gm.curSelected = card;
			}
		}

		if (gameObject.tag == "AIOwned" && gm.curSelected != null) {
			gm.curSelected.cActions.Use (card);
			gm.curSelected.cActions.isUsed = true;
			gm.curSelected = null;
		}

	}
	/*public void AISelect(){
		if (!isUsed && gameObject.tag == "AIOwned") {
			if (isSelected) {
				gm.aiCurSelected = null;
				isSelected = false;
			} else {
				isSelected = true;
				gm.aiCurSelected = card;

				Debug.Log ("AI has selected a card!");
			}
		}

	}*/

	public void Use(V_Card target){
		V_Card thisCard = card;
		Debug.Log ("AI attacked!");
		if (target.gameObject.tag == "AIOwned" && this.tag == "PlayerOwned") {
			if (thisCard.type == V_Card.cardType.Creature) {
				// Enemy damaged effect:
				Text enemy = Instantiate (V_GameManager.sdamageEffect, target.transform) as Text;
				enemy.text = "-" + thisCard.attackDamage;
				// We damaged effect:
				Text us = Instantiate (V_GameManager.sdamageEffect, thisCard.transform) as Text;
				us.text = "-" + target.attackDamage;
				//
				target.health -= thisCard.attackDamage;
				thisCard.health -= target.attackDamage;
                target.cardHealthHandler.text = target.health.ToString();
                thisCard.cardHealthHandler.text = thisCard.health.ToString();
                thisCard.DoEffect ();
				DestroyThisCard ();
				target.cActions.DestroyThisCard ();
				isUsed = true;
			}
		}
		if (target.gameObject.tag == "PlayerOwned" && this.tag == "AIOwned") {
			if (thisCard.type == V_Card.cardType.Creature) {
				Debug.Log ("AI attacked!");
				// Enemy damaged effect:
				Text enemy = Instantiate (V_GameManager.sdamageEffect, target.transform) as Text;
				enemy.text = "-" + thisCard.attackDamage;
				// We damaged effect:
				Text us = Instantiate (V_GameManager.sdamageEffect, thisCard.transform) as Text;
				us.text = "-" + target.attackDamage;
				//
				target.health -= thisCard.attackDamage;
                thisCard.health -= target.attackDamage;
                target.cardHealthHandler.text = target.health.ToString();
                thisCard.cardHealthHandler.text = thisCard.health.ToString();
				thisCard.DoEffect ();
				DestroyThisCard ();
				target.cActions.DestroyThisCard ();
				isUsed = true;
			}
		}
	}
    ///This will be for once we add gewnerators to the game.
    //public void UsetoGenerator(V_Card target)
    //{
    //    V_Card thisCard = card;
    //    Debug.Log("AI attacked!");
    //    if (target.gameObject.tag == "AIOwned" && this.tag == "PlayerOwned")
    //    {
    //        if (thisCard.type == V_Card.cardType.Creature)
    //        {
    //            // We damaged effect:
    //            Text us = Instantiate(V_GameManager.sdamageEffect, thisCard.transform) as Text;
    //            us.text = "-" + target.attackDamage;
    //            //
    //            target.health -= thisCard.attackDamage;
    //            thisCard.health -= target.attackDamage;
    //            target.cardHealthHandler.text = target.health.ToString();
    //            thisCard.cardHealthHandler.text = thisCard.health.ToString();
    //            thisCard.DoEffect();
    //            DestroyThisCard();
    //            target.cActions.DestroyThisCard();
    //            isUsed = true;
    //        }
    //    }
    //    if (target.gameObject.tag == "PlayerOwned" && this.tag == "AIOwned")
    //    {
    //        if (thisCard.type == V_Card.cardType.Creature)
    //        {
    //            Debug.Log("AI attacked!");
    //            // Enemy damaged effect:
    //            Text enemy = Instantiate(V_GameManager.sdamageEffect, target.transform) as Text;
    //            enemy.text = "-" + thisCard.attackDamage;
    //            //
    //            target.health -= thisCard.attackDamage;
    //            thisCard.health -= target.attackDamage;
    //            target.cardHealthHandler.text = target.health.ToString();
    //            thisCard.cardHealthHandler.text = thisCard.health.ToString();
    //            thisCard.DoEffect();
    //            DestroyThisCard();
    //            target.cActions.DestroyThisCard();
    //            isUsed = true;
    //        }
    //    }
    //}

    public void UseToPlayer(GameObject target){
		V_Card thisCard = card;
		Debug.Log ("AI attacked!");
		if (target.tag == "Player") {
			if (thisCard.type == V_Card.cardType.Creature) {
				// Enemy damaged effect:
				Text enemy = Instantiate (V_GameManager.sdamageEffect, target.transform) as Text;
				enemy.text = "-" + thisCard.energyCost;
                //
                V_PlayerHandler.health -= thisCard.energyCost;
				isUsed = true;
			}
		}
		if (target.tag == "AIPlayer") {
			if (thisCard.type == V_Card.cardType.Creature) {
				// Enemy damaged effect:
				Text enemy = Instantiate (V_GameManager.sdamageEffect, target.transform) as Text;
				enemy.text = "-" + thisCard.energyCost;
				//
				V_AI.health -= thisCard.energyCost;
				isUsed = true;
			}
		}
	}

	public void DestroyThisCard(){
        //send to graveyard instead
		V_Card thisCard = card;
		if (thisCard.health <= 0) {
			Instantiate (thisCard.deathEffect, thisCard.transform);
            if (gameObject.tag == "PlayerOwned")
             {
                 gameObject.tag = "InGrave";
                transform.SetParent(V_GameManager.graveZone.transform);
                 curParent = V_GameManager.graveZone.transform;
                
            }
            else
            {
                Destroy (gameObject, 1f);
            }
        }
    }
}

