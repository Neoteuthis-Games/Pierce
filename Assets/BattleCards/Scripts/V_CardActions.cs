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
	public bool isSelected = false; //card is currently selected.
	public bool isUsed = false; //card has expended uses per turn.
    public bool isActive = false; //card has actions but out of its turn in war.
    public bool isDelayed = false; //card is delaying its action til the end of war.
	[Header("    Misc:")]
	public Transform curParent;
	public bool isInGame = false;
	public V_GameManager gm;
	[HideInInspector] public V_Card card;

	//private variables:
	private Image Brendrr;
	private Image Srendrr;
    //number of cards allowed in play, we can make public if things modify it.
    private int fieldlimit = 8;
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
        if (isActive)
        {
            card.inactiveEffect.SetActive(true);
        }
        else
        {
            card.inactiveEffect.SetActive(false);
        }
        if (V_GameManager.CardGameState == V_GameManager.currentState.war && card.actions <= 0)
        {
            card.inactiveEffect.SetActive(true);
        }
        else
        {
            card.inactiveEffect.SetActive(false);
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
        if (gameObject.tag == "AIOwned")
        {
            if (gm.aiCurSelected == card)
            {
                card.selectedEffect.SetActive(true);
                isSelected = true;
            }
            else
            {
                card.selectedEffect.SetActive(false);
                isSelected = false;
            }
        }
        /////warloop{
        //while(V_GameManager.cardgamestate == V_GameManager.currentState.war)
        //{

        //}
	}
	
	public void OnDrag(){
		if (isInGame && V_GameManager.playerTurn == V_GameManager.playerTypes.Player && gameObject.tag == "InHand") { //aha!
			//Debug.Log ("Dragging Card...");
			transform.position = Input.mousePosition;
			transform.SetParent (V_GameManager.gameArea.transform);

			// Let's have the zone flashing effect when this card is dragged over a certain zone.

			//       If this card is a CREATURE card and this card is over the Battle Zone:
			if (IsCursorInZone (Input.mousePosition, V_GameManager.battleZone) && card.type == V_Card.cardType.Creature || IsCursorInZone(Input.mousePosition, V_GameManager.battleZone) && card.type == V_Card.cardType.Event)
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
		if (isInGame && V_GameManager.playerTurn == V_GameManager.playerTypes.Player && gameObject.tag == "InHand"){// && V_GameManager.cardgamestate == V_GameManager.currentState.action) {
            //WE NEED TO MODIFY THIS FOR CARDS THAT CAN BE PLAYED OUTSIDE THE ACTION PHASE.
			// If this card is a CREATURE card, then do:
            //ok this area we will need to modify ALOT.
			if (card.type == V_Card.cardType.Creature) {
				if (curParent == V_GameManager.handZone.transform) {
					if (IsCursorInZone (Input.mousePosition, V_GameManager.battleZone)) {
						Debug.Log ("It's a Creature!");
						if (card.energyCost <= V_PlayerHandler.energy) {
							if (gm.battleZoneHandler.transform.childCount < fieldlimit && V_GameManager.CardGameState == V_GameManager.currentState.action) {
								V_PlayerHandler.energy -= card.energyCost;
								transform.SetParent (V_GameManager.battleZone.transform);
								curParent = V_GameManager.battleZone.transform;
								gameObject.tag = "PlayerOwned";
                                PlayEffect();
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
						if (card.energyCost <= V_PlayerHandler.energy && V_GameManager.CardGameState == V_GameManager.currentState.action) {
							V_PlayerHandler.energy -= card.energyCost;
							transform.SetParent (V_GameManager.battleZone.transform);
							curParent = V_GameManager.battleZone.transform;
							gameObject.tag = "PlayerOwned";
							// Do the SPELL effects:
							card.DoEffect ();
                            PlayEffect();
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
            if (V_GameManager.CardGameState == V_GameManager.currentState.war)
            {
                if (isSelected || isActive == false)
                {
                    gm.curSelected = null;
                    isSelected = false;
                }
                else
                {
                    isSelected = true;
                    gm.curSelected = card;
                }
            }
            else
            {
                if (isSelected)
                {
                    gm.curSelected = null;
                    isSelected = false;
                }
                else
                {
                    isSelected = true;
                    gm.curSelected = card;
                }
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
    //USE THIS FOR ATTACKING/USING ACTIONS
	public void Use(V_Card target){
		V_Card thisCard = card;
        thisCard.isAttacking = true;
        Debug.Log ("Player attacked!");

		if (target.gameObject.tag == "AIOwned" && this.tag == "PlayerOwned") {
			if (thisCard.type == V_Card.cardType.Creature && V_GameManager.CardGameState == V_GameManager.currentState.war) {
                AttackEffect(target, null);
                // Enemy damaged effect:
                Text enemy = Instantiate (V_GameManager.sdamageEffect, target.transform) as Text;
				enemy.text = "-" + thisCard.attackDamage;
				// We damaged effect:
				Text us = Instantiate (V_GameManager.sdamageEffect, thisCard.transform) as Text;
				us.text = "-" + target.attackDamage;
                //
                target.health -= thisCard.attackDamage + card.PowerAttack - target.Armor;
				thisCard.health -= target.attackDamage - target.Armor;
                target.cardHealthHandler.text = target.health.ToString();
                thisCard.cardHealthHandler.text = thisCard.health.ToString();
                thisCard.DoEffect ();//AHA THERE IT IS. COMBAT EFFECTS
                thisCard.actions--;
                target.actions--;
				DestroyThisCard ();
				target.cActions.DestroyThisCard ();
				isUsed = true;
			}
		}
		if (target.gameObject.tag == "PlayerOwned" && this.tag == "AIOwned") {
			if (thisCard.type == V_Card.cardType.Creature && V_GameManager.CardGameState == V_GameManager.currentState.war) {
				Debug.Log ("AI attacked!");
                AttackEffect(target, null);
				// Enemy damaged effect:
				Text enemy = Instantiate (V_GameManager.sdamageEffect, target.transform) as Text;
				enemy.text = "-" + thisCard.attackDamage;
				// We damaged effect:
				Text us = Instantiate (V_GameManager.sdamageEffect, thisCard.transform) as Text;
				us.text = "-" + target.attackDamage;
				//
				target.health -= thisCard.attackDamage + card.PowerAttack - target.Armor;
                thisCard.health -= target.attackDamage - target.Armor;
                target.cardHealthHandler.text = target.health.ToString();
                thisCard.cardHealthHandler.text = thisCard.health.ToString();
				thisCard.DoEffect ();//here
                thisCard.actions--;
                target.actions--;
                DestroyThisCard ();
				target.cActions.DestroyThisCard ();
				isUsed = true;
			}
		}
        thisCard.isAttacking = false;
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

    public void UseToPlayer(GameObject target){ //for attacking players.
		V_Card thisCard = card;
        thisCard.isAttacking = true;
		Debug.Log ("AI attacked!");
		if (target.tag == "Player") {
            V_GameManager.CardGameState = V_GameManager.currentState.war; //testing
            if (thisCard.type == V_Card.cardType.Creature && V_GameManager.CardGameState == V_GameManager.currentState.war) {
                AttackEffect(null, target);
				// Enemy damaged effect:
				Text enemy = Instantiate (V_GameManager.sdamageEffect, target.transform) as Text;
				enemy.text = "-" + thisCard.energyCost;
                //
                V_PlayerHandler.health -= thisCard.energyCost; //this can be changed to make people discard from the deck instead.
                for (int i = thisCard.energyCost; i > 0; i--)
                {
                  target.GetComponent<V_PlayerHandler>().TakeDamage();//null???
                }

                thisCard.DoEffect();
                thisCard.actions--;
                isUsed = true;
			}
		}
		if (target.tag == "AIPlayer") {
			if (thisCard.type == V_Card.cardType.Creature && V_GameManager.CardGameState == V_GameManager.currentState.war) {
                AttackEffect(null, target);
                // Enemy damaged effect:
                Text enemy = Instantiate (V_GameManager.sdamageEffect, target.transform) as Text;
				enemy.text = "-" + thisCard.energyCost;
				//
				V_AI.health -= thisCard.energyCost; //same here
                thisCard.DoEffect();
                thisCard.actions--;
                isUsed = true;
			}
		}
        thisCard.isAttacking = false;
	}

	public void DestroyThisCard(){
        //send to graveyard instead
		V_Card thisCard = card;
        if (thisCard.health <= 0 || card.isDestroyed) {
			Instantiate (thisCard.deathEffect, thisCard.transform);
            if (gameObject.tag == "PlayerOwned")
             {
                 gameObject.tag = "InGrave";
                transform.SetParent(V_GameManager.graveZone.transform);
                V_GameManager.graveZone.GetComponent<V_Graveyard>().AddToGraveyard(gameObject);
                 curParent = V_GameManager.graveZone.transform;
                
            }
            else
            {
                if (gm.ActiveCards.Find(x => gameObject) != null)
                {
                    gameObject.tag = "InGrave";
                }
                else
                {
                    Destroy(gameObject, 1f);
                }
            }
        }
    }
    //add all unique play effects here.
    public void PlayEffect()
    {
        if (card.SpecialTiming == V_Card.UniqueEffectType.Play)
        {
            switch (card.SpecialEffect)
            {

                default: break;
            }
        }
    }
    //add all unique attack effects here.
    public void AttackEffect(V_Card target, GameObject targetplayer)
    {
        if (card.SpecialTiming == V_Card.UniqueEffectType.Attack)
        {
            switch (card.SpecialEffect)
            {
                case V_Card.UniqueEffect.WebCrawler_OnAttack:
                    card.WebCrawler(target);
                    break;


                default: break;
            }
        }
    }
}

