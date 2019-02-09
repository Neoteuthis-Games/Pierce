using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
///      Card script for "BattleCards: CCG Adventure Template"
/// 
/// "This is the main component for every cards in this game 
///              - the most important actually..."
/// 
/// To clarify some comments, here are the reference for the TERMS:
/// 
///    Our    - Refers to this instance/GameObject (not the main player).
///    Player - Refers to the player.
///    
///            Added: 2016
/// </summary>

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(V_CardActions))]
public class V_Card : MonoBehaviour , IPointerClickHandler {

	public enum cardType { Creature, Spell, Generator, Equipment, Virus, Item, Upgrade, Avatar }; //spells are events. will leave it like this for now.
    public enum cardDomain {None, Animal, Plant, Spirit, Elemental, Fungus, Machine, Nightmare, }; //domains for the cards, generally only creatures will have a domain.
    public enum cardSubDomain {None, Dust, Fire, Water, Lightning, Wind,}; //generally elementals will have subtypes, but others may too.
    public enum cardRank {Common, Uncommon, Rare, UltraRare, Event}; 
	public enum cardEffect {None, DrawXCards, AddEnergy, AddHealth, DamagePlayer, DrawuptoXcards, LowerAllSpeed, }; //so many to add here. this will expand alot...
	public enum cardTarget {None, ToPlayer, ToOpponent};
	public enum usage {All, CardsOnly, BaseOnly, GeneratorsOnly, };
    public enum UniqueEffect {None, WebCrawler_OnAttack, }; //expand this list for unique effects. this will get big...
    public enum UniqueEffectType { None, Constant, Activated, Death, Play, Attack, Convert, Discard, Toss, Purge, Draw, }; 
    [Header("    Card Type:")]
	public cardType type;
    [Header("    Card Domain:")]
    public cardDomain domain;
    public cardSubDomain subdomain;
    [Header("    Card Rank:")]
	public cardRank rank;
	[Header("    Name & Description:")]
	public string cardName = "Glitch";
	public string cardDescription = "Duplicate the 8th item in your inventory 255 times.";

	[Header("    Attributes:")]
	public int attackDamage = 0;
	public int health = 10;
    public int speed = 0;
	public int energyCost = 0;
    [HideInInspector]
    public int baseattackDamage = 10;
    public int basehealth = 20;
    public int basespeed = 30;
    public int baseenergyCost = 1;
    /// <summary>
    /// USE THE mod ONES to add effects that last until the card dies,
    /// the TEMP ones will be wiped at the end of each turn.
    /// </summary>
    public int MODattackDamage = +0;
    public int MODhealth = +0;
    public int MODspeed = +0;
    public int MODenergyCost = +0;
    public int TEMPattackDamage = +0;
    public int TEMPhealth = +0;
    public int TEMPspeed = -0;
    public int TEMPenergyCost = -0;
    public int actions = 1;
    public bool isDestroyed = false; //flip this to true to use any destroy effects, then call destroythiscard in actions.
    public bool isConverted = false; //this will be used to identify if a card is being destroyed from conversion.
    [Space]
    [Header("            Special Attributes:")]
   // public bool UniqueEffect = false;
    public bool Relentless = false; // use this if the creature's speed cannot be reduced.
    public bool klutz = false; //use this if the creature can't hold equipment.
    public bool interceptor = false; //the creature can intercept an attack using its action.
    public bool demolisher = false; // the creature can attack generators while guarded
    public bool clockwork = false; //use this if the card uses gear counters.
    public int PowerAttack = 0; //use this if the card gains power while attacking.
    public int Armor = 0; // use this to reduce damage taken by cards
    public int recoil = 0; //you take damage while attacking with this card.
    public int regenerate = 0; //use this if the creature regains health when you recharge.
    [Space]
    public int GearCounters = 0; //if clockwork, these will count down every end step.
    public int SwarmCounters = 0; //these count down to replace destroy effects.
    public int DeathCounters = 0; //add these for certain effects like boosting attacks etc
    public int LifeCounters = 0; //add these for damage prevention effects
    [Space]
    [Header("                             -Extra Effects-")]
	public cardEffect extraEffect;
	public cardTarget target;
	public int effectValue = 1;
    [Space]
    public cardEffect PlayEffect;
    public cardEffect AttackEffect;
    public cardEffect DeathEffect;
    public cardEffect ActivatedEffect;
    public UniqueEffect SpecialEffect;
    public UniqueEffectType SpecialTiming;
    [HideInInspector]
	public bool autoUse = false;
	[Space]
	[Header("    Usage:")]
	public usage canBeUsedTo; //utilize this to determine when we are able to do the thing with the thing
	[Space]
	[Header("    Parts:")]
	public Sprite graphicImage;
	public Image graphicHandler;
	public Text cardNameHandler;
	public Text cardDescriptionHandler;
	public Text cardLevelHandler;
    public Text cardTypeHandler;
    public Text cardDomainHandler;
    public Text cardDamageHandler;
	public Text cardHealthHandler;
	public Text cardEnergyHandler;
    public Text cardSpeedHandler;
    [Space]
	[Header("    Miscs:")]
	public GameObject selectedEffect;
	public GameObject disabledEffect;
	public GameObject deathEffect;
	[Space]
	public bool placed = false;
    public bool isAttacking = false;
	public float delay = 1;
	private float curDelay = 0;
	[HideInInspector] public V_CardActions cActions;
	[HideInInspector] public V_CardViewer viewer;

	void OnEnable(){
		// References:
		cActions = GetComponent<V_CardActions> ();
		viewer = FindObjectOfType<V_CardViewer> ();
	}

	void Start(){
		// Remove this if you also want the card animations to play in main menu:
		GetComponentInChildren<Animator> ().enabled = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name != "MainMenu"; //gamestate update
        baseattackDamage = attackDamage;
        baseenergyCost = energyCost;
        basehealth = health;
        basespeed = speed;
	}

	// Update is called once per frame
	void Update () {

		// If our V_CardActions component found a game manager then it means we're in a game.    -_- bad!
		// And if it's true then behave like in game:
		if (cActions.gm) {
			if (health <= 0)
            {
				health = 0;
			}
            if(attackDamage <= 0)
            {
                attackDamage = 0;
            }
            if(speed <= 0)
            {
                speed = 0;
            }
            if(energyCost < 0)
            {
                energyCost = 0;
            }
            //stop event cards from staying in play. disable this if we ever have a a lasting event.
			if (type == cardType.Spell && autoUse == false) {
				autoUse = true;
			}
			if (extraEffect == cardEffect.None) {
				target = cardTarget.None;
			}

			//// IN THE BATTLEFIELD:
			if (placed == true) {
				curDelay += Time.deltaTime;
				if (curDelay >= delay){
					curDelay = 0;
					placed = false;
					DoEffect();//KEEP THIS FOR ETB EFFECTS
				}
			}
		}

		// For UI:
		if (graphicImage != null && graphicHandler != null) {
			graphicHandler.sprite = graphicImage;
		}
		if (cardNameHandler != null) {
			cardNameHandler.text = cardName;
		}
		if (cardDescriptionHandler != null) {
			cardDescriptionHandler.text = cardDescription;
		}
		if (cardDamageHandler != null) {
			cardDamageHandler.text = attackDamage.ToString ();
		}
		if (cardHealthHandler != null) {
			cardHealthHandler.text = health.ToString ();
		}
		if (cardEnergyHandler != null) {
			cardEnergyHandler.text = energyCost.ToString ();
		}
        if (cardSpeedHandler != null)
        {
            cardSpeedHandler.text = speed.ToString();
        }
        // Disable scripts (including this) when in main menu:
        ///wait why not just have a gamestate enum in the manager??? 
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name == "MainMenu") {//add gamestate to fix this...
			cActions.enabled = false;
			this.enabled = false;
		}
	}

	public void OnPointerClick (PointerEventData eventData) {

		// View card when right clicked:
		if (eventData.button == PointerEventData.InputButton.Right) {
			viewer.ViewCard (this);
		}

		// Select card when left clicked:
		if (eventData.button == PointerEventData.InputButton.Left) {
			cActions.Select ();
		}
	}

	public void DoEffect(){
		if (this.gameObject.tag == "PlayerOwned") {
            //WHY ISN'T THIS A SWITCH CASE??? ///we can do the thing. the thing with child classes.
			if (extraEffect == cardEffect.AddEnergy) {
				if (target == cardTarget.ToOpponent) {
					V_GameManager.EffectAddEnergy (effectValue);
				} else {
					V_PlayerHandler.AddEnergy (effectValue);
				}
			}
			if (extraEffect == cardEffect.AddHealth) {
				if (target == cardTarget.ToOpponent) {
					V_GameManager.EffectHeal (effectValue);
				} else {
					V_PlayerHandler.EffectHeal (effectValue, V_GameManager.shealEffect.gameObject, V_GameManager.avatarZone.transform);
				}
			}
			if (extraEffect == cardEffect.DamagePlayer) {
				if (target == cardTarget.ToOpponent) {
					V_GameManager.EffectDamage (effectValue);
				} else {
					V_PlayerHandler.EffectDamage (effectValue, V_GameManager.sdamageEffect.gameObject, V_GameManager.avatarZone.transform);
				}
			}
			if (extraEffect == cardEffect.DrawXCards) {
				if (target == cardTarget.ToOpponent) {
					// nothing here yet...
				} else {
					V_PlayerHandler player = FindObjectOfType<V_PlayerHandler> ();
                    //draw x cards
                    for (int i = effectValue; i > 0; i--)
                    {
                        player.DrawOneCard();
                    }
				}
			}
            if (extraEffect == cardEffect.DrawuptoXcards)
            {
                if (target == cardTarget.ToOpponent)
                {
                    // nothing here yet...
                }
                else
                {
                    V_PlayerHandler player = FindObjectOfType<V_PlayerHandler>();
                    //draw up to x cards
                    V_GameManager.effectdraw = effectValue;
                }
            }
            if (extraEffect == cardEffect.LowerAllSpeed)
            {
                //doesn't work :():(:(:(:)(: yet... BUT WHY NOT?????
                GameObject[] obj = GameObject.FindGameObjectsWithTag("AIOwned");
                foreach (GameObject o in obj)
                {
                    if(o.GetComponent<V_Card>().type == cardType.Creature && o.GetComponent<V_Card>().Relentless == false)
                    {
                        o.GetComponent<V_Card>().speed -= effectValue;
                        if (o.GetComponent<V_Card>().speed < 0)
                            o.GetComponent<V_Card>().speed = 0;
                        o.GetComponent<V_Card>().cardSpeedHandler.text = o.GetComponent<V_Card>().speed.ToString();
                    }
                }
                GameObject[] objP = GameObject.FindGameObjectsWithTag("PlayerOwned");
                foreach (GameObject o in objP)
                {
                    if (o.GetComponent<V_Card>().type == cardType.Creature && o.GetComponent<V_Card>().Relentless == false)
                    {
                        o.GetComponent<V_Card>().speed -= effectValue;
                        if (o.GetComponent<V_Card>().speed < 0)
                            o.GetComponent<V_Card>().speed = 0;
                        o.GetComponent<V_Card>().cardSpeedHandler.text = o.GetComponent<V_Card>().speed.ToString();
                    }
                }
            }
        }
		if (type == cardType.Spell) {
			SpellActivate ();
		}
	}

	public void DoEffectAI(){
		if (this.gameObject.tag == "AIOwned") {
			if (extraEffect == cardEffect.AddEnergy) {
				if (target == cardTarget.ToOpponent) {
					V_PlayerHandler.AddEnergy (effectValue);
				} else {
					V_GameManager.EffectAddEnergy (effectValue);
				}
			}
			if (extraEffect == cardEffect.AddHealth) {
				if (target == cardTarget.ToOpponent) {
					V_PlayerHandler.EffectHeal (effectValue, V_GameManager.shealEffect.gameObject, V_GameManager.avatarZone.transform);
				} else {
					V_GameManager.EffectHeal (effectValue);
				}
			}
			if (extraEffect == cardEffect.DamagePlayer) {
				if (target == cardTarget.ToOpponent) {
					V_PlayerHandler.EffectDamage (effectValue, V_GameManager.sdamageEffect.gameObject, V_GameManager.avatarZone.transform);
				} else {
					V_GameManager.EffectDamage (effectValue);
				}
			}
		}
		if (type == cardType.Spell) {
			SpellActivate ();
		}
	}

	public void SpellActivate(){
		Destroy (gameObject, delay); //oh no... ok I need persistance and a graveyard....
		V_GameManager gm = FindObjectOfType<V_GameManager>();
		if (gm.curSelected == this) {
			gm.curSelected = null;
		}
	}

    //public void UniqueEffectActivate()//we may not need this... we'll see...
    //{
    //    switch (SpecialEffect)
    //    {
    //        case UniqueEffect.None:
    //            break;
    //        case UniqueEffect.WebCrawler_OnAttack:
    //           // WebCrawler();
    //            break;
    //        default:
    //            break;
    //    }
    //}
   public void WebCrawler(V_Card target)
    {
        if(target != null) { 
             if(target.speed == 0)
             {
                this.PowerAttack = 50;
             } else
             {
                this.PowerAttack = 0;
             }
        }
    }
}
