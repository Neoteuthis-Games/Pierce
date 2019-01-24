﻿using UnityEngine;
using System.Collections;
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

	public enum cardType {Creature, Spell};
	public enum cardRank {Common, Uncommon, Unique};
	public enum cardEffect {None, DrawNewCard, AddEnergy, AddHealth, DamagePlayer};
	public enum cardTarget {None, ToUs, ToOpponent};
	public enum usage {All, CardsOnly, BaseOnly,};
	[Header("    Card Type:")]
	public cardType type;
	[Header("    Card Rank:")]
	public cardRank rank;
	[Header("    Name & Description:")]
	public string cardName = "Warrior";
	public string cardDescription = "Active: Deal 1 damage to a card or to the opponent player.";

	[Header("    Attributes:")]
	public int attackDamage = 1;
	public int health = 2;
	public int energyCost = 1;
	[Space]
	[Header("                             -Extra Effects-")]
	public cardEffect extraEffect;
	public cardTarget target;
	public int effectValue = 1;
	[HideInInspector]
	public bool autoUse = false;
	[Space]
	[Header("    Usage:")]
	public usage canBeUsedTo;
	[Space]
	[Header("    Parts:")]
	public Sprite graphicImage;
	public Image graphicHandler;
	public Text cardNameHandler;
	public Text cardDescriptionHandler;
	public Text cardLevelHandler;
	public Text cardDamageHandler;
	public Text cardHealthHandler;
	public Text cardEnergyHandler;
	[Space]
	[Header("    Miscs:")]
	public GameObject selectedEffect;
	public GameObject disabledEffect;
	public GameObject deathEffect;
	[Space]
	public bool placed = false;
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
		GetComponentInChildren<Animator> ().enabled = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name != "MainMenu";
	}

	// Update is called once per frame
	void Update () {

		// If our V_CardActions component found a game manager then it means we're in a game.
		// And if it's true then behave like in game:
		if (cActions.gm) {
			if (health <= 0) {
				health = 0;
			}
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
					DoEffect();
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

		// Disable scripts (including this) when in main menu:
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name == "MainMenu") {
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
			if (extraEffect == cardEffect.DrawNewCard) {
				if (target == cardTarget.ToOpponent) {
					// nothing here yet...
				} else {
					V_PlayerHandler player = FindObjectOfType<V_PlayerHandler> ();
					player.DrawOneCard();
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
		Destroy (gameObject, delay);
		V_GameManager gm = FindObjectOfType<V_GameManager>();
		if (gm.curSelected == this) {
			gm.curSelected = null;
		}
	}
}
