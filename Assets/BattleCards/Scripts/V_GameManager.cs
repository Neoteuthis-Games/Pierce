using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
///      GameManager script for "BattleCards: CCG Adventure Template"
/// 
/// "This script controls the main game scene. Some important commands
/// are passed here between the Player and the AI."
/// 
/// To clarify some comments, here are the reference for the TERMS:
/// 
///    Our    - Refers to this instance/GameObject (not the main player).
///    Player - Refers to the player.
///    
///            Added: 2016
/// </summary>
 //I will be working with the game manager. Please talk to me before editing this yourself. ~Neo
public class V_GameManager : MonoBehaviour {
	
	// Player types:
	public enum playerTypes {Player, AI}; //it cycles through the enum, so we'll have to just call player2 AI in 2 player mode.
    // Gamestate types
    public enum Gamestate {Menu, Overworld, InGame, Gameover, Deckbuilding, Shop, Paused, Story }; //general
   public enum currentState {begin, recharge, draw, action, war, end}; //incardgame

	[Header("    Game Properties:")]
	public int startingEnergy = 5;			// The starting energy for both players
    public int refreshedEnergy = 5;         // The energy a player is set to during their recharge step
	public int increasingEnergy = 0;        // Energy increased every turn - unneeded.
    public int startingHealth = 30;         // The starting health of a base - unneeded.
    public int maxHealth = 30;              // The maximum health allowed for a base - unneeded.
    public int drawCost = 0;				// Cost of refreshing the hand cards - unneeded.
    public const int freedrawnum = 2;
    public int freedraws = freedrawnum;
	[Space]
	[Header("    UI:")]
	public GameObject DrawBTN;
	public GameObject endTurnBTN;
	public GameObject errorText;
	public Text energyText;
	public Text hpText;
	public Text aiHpText;
	public Text drawCostText;
	public Text healEffect;
	public Text damageEffect;
	public Text opponentsTurnText;
	public Text playersTurnText;
	public GameObject cardViewer;
	[Space]
	public GameObject resultPanel;
	public Sprite victoryBanner;
	public Sprite failedBanner;
	[Space]
	[Header("    Game Elements:")]
	public GameObject aiBattleZoneHandler;
	public GameObject aiSpellZoneHandler;
	public GameObject aiAvatarHandler;
    public GameObject aigraveZoneHandler;
    public GameObject battleZoneHandler;
	public GameObject spellZoneHandler;
    public GameObject graveZoneHandler;
    public GameObject handZoneHandler;
	public GameObject gameAreaHandler;
	public GameObject avatarHandler;
	[Space]
	[Header("    Error Messages:")]
	public string noEnergy;
	public string cantBeUsedToBase;
	public string cantBeUsedToCard;
	public string cantPlaceThere;
	public string battleZoneIsFull;

	[HideInInspector]
	public V_Card curSelected;
	[HideInInspector]
	public V_Card aiCurSelected;

	// Some static variables for other script's reference:
	public static playerTypes playerTurn;
    public static currentState cardgamestate;
	public static Text sdamageEffect;
	public static Text shealEffect;
	public static int curEnergy;
	public static int curHealth;
	public static int sEnergy;
	public static int iEnergy;
	public static GameObject poof;
	public static GameObject battleZone;
	public static GameObject spellZone;
	public static GameObject handZone;
    public static GameObject graveZone;
    public static GameObject gameArea;
	public static GameObject avatarZone;
	public static GameObject aiBattleZone;
	public static GameObject aiSpellZone;
	public static GameObject aiAvatarZone;
    public static GameObject aigraveZone;
    public static bool allowIncreasingEnergy = false;

	[HideInInspector] public bool isGameOver = false;

	// Internal:
	V_PlayerHandler p;	// For accessing non-static variables in player
	V_GameManager gm;	// For accessing non-static variables in game manager

	// Use this for initialization
	void Start () {
        //need to add handlers for multiple new zones
		battleZone = battleZoneHandler;
		spellZone = spellZoneHandler;
		handZone = handZoneHandler;
		gameArea = gameAreaHandler;
        graveZone = graveZoneHandler;
		aiBattleZone = aiBattleZoneHandler;
		aiSpellZone = aiSpellZoneHandler;
		aiAvatarZone = aiAvatarHandler;
        aigraveZone = aigraveZoneHandler;
		avatarZone = avatarHandler;
		sdamageEffect = damageEffect;
		shealEffect = healEffect;
		drawCostText.text = drawCost.ToString ();

		// Draw the first hand cards:
		playerTurn = playerTypes.Player;
		p.gm = this;
        //this is what we need to use to set the health to the size of the deck. we can disable it for now.
        ///V_PlayerHandler.health = p.myDeck.Length;
		p.StartDraw ();
	}

	void Awake(){
		// References:
		p = FindObjectOfType<V_PlayerHandler>();
		gm = FindObjectOfType<V_GameManager> ();

		//Set the player to Game Mode:
		V_PlayerHandler.isInGame = true;

		// Set the player's starting life and energy:
		V_PlayerHandler.health = startingHealth;
		V_PlayerHandler.energy = startingEnergy;
		V_AI.health = startingHealth;
		V_AI.energy = startingEnergy;

		sEnergy = startingEnergy;
		iEnergy = increasingEnergy;
	}
	
	// Update is called once per frame
	void Update () {
		curEnergy = V_PlayerHandler.energy;
		curHealth = V_PlayerHandler.health;

		energyText.text = curEnergy.ToString ();
		aiHpText.text = V_AI.health.ToString () + "/" + maxHealth;
		hpText.text = curHealth.ToString () + "/" + maxHealth;

		// Check if all players still have sufficient health, but
		// if one doesn't then call the CallGameResult function:
		if (!isGameOver) {
			// if the player's base died:
			if (V_PlayerHandler.health == 0) {
				isGameOver = true;
				// then AI is the winner:
				CallGameResult (false);
				return;
			}
			// if the AI's base died:
			if (V_AI.health == 0) {
				isGameOver = true;
				// then player is the winner:
				CallGameResult (true);
			}
		}

		opponentsTurnText.gameObject.SetActive (playerTurn == playerTypes.AI && !isGameOver);
		playersTurnText.gameObject.SetActive (playerTurn == playerTypes.Player && !isGameOver);
	}
	// This is called when a player hits the "End Turn" button:
	public void ChangeTurn(playerTypes type){
        //modify this to create the stages of a turn. this is the recharge step. players can draw up to 2 cards and their energy becomes 5. now its talking about the players turn. we will edit this first.
		if (type == playerTypes.AI) {
			if (allowIncreasingEnergy) {
                V_PlayerHandler.energy = 5;
                //V_PlayerHandler.AddEnergy (iEnergy);
            }
			// Draw 1 free card if Hand Zone is'nt full:
			if (handZone.transform.childCount < 7) {//no max hand while drawing, only a discard step, but space is an issue here...
                V_GameManager.cardgamestate = V_GameManager.currentState.draw;
               // p.DrawOneCard();
            }
            if (handZone.transform.childCount < 7)
            {
                //p.DrawOneCard();
            }
            allowIncreasingEnergy = true;
			playerTurn = playerTypes.Player;
			gm.endTurnBTN.SetActive (true);
			gm.DrawBTN.SetActive (true);
			GameObject[] obj = GameObject.FindGameObjectsWithTag ("PlayerOwned");
			foreach (GameObject o in obj) {
				o.GetComponent<V_CardActions> ().isUsed = false;
			}//now its talking about the AI turn. lets ignore this for now.
		} else if (type == playerTypes.Player) {
			if (allowIncreasingEnergy) {
				V_AI.EffectAddEnergy (iEnergy);
			}
			allowIncreasingEnergy = true;
			playerTurn = playerTypes.AI;
			GameObject[] obj = GameObject.FindGameObjectsWithTag ("AIOwned");
			foreach (GameObject o in obj) {
				o.GetComponent<V_CardActions> ().isUsed = false;
			}
		}
	}

	/// <summary>
	/// Send damage to the AI!
	/// </summary>
	/// <param name="value">Value.</param>
	public static void EffectDamage (int value){
		V_AI.EffectDamage (value, sdamageEffect.gameObject, aiAvatarZone.transform);
	}
	/// <summary>
	/// Heal the AI...
	/// </summary>
	/// <param name="value">Value.</param>
	public static void EffectHeal (int value){
		V_AI.EffectHeal (value, shealEffect.gameObject, aiAvatarZone.transform);
	}
	/// <summary>
	/// Add increase the energy of the AI!
	/// </summary>
	/// <param name="value">Value.</param>
	public static void EffectAddEnergy (int value){
		V_AI.EffectAddEnergy (value);
	}

	// Called by the Redraw() function in the V_PlayerHandler script:
	public void Redraw(V_Card[] cards){
		RectTransform[] childCards = handZone.transform.GetComponentsInChildren<RectTransform> ();
		foreach (RectTransform t in childCards) {
			if (t.gameObject.tag == "InHand" && t.gameObject != this.gameObject) {
				Destroy (t.gameObject);
			}
		}

		foreach (V_Card card in cards) {
			V_Card curCard = Instantiate (card, handZone.transform);
			curCard.cActions.isInGame = true;

		}
	}

	public void DrawACard(V_Card[] cards){

		foreach (V_Card card in cards) {
			V_Card curCard = Instantiate (card, handZone.transform);
			curCard.cActions.isInGame = true;
		}
	}

	public void AttackPlayer(V_Card card, playerTypes who){
		if (who == playerTypes.Player) {
			GameObject targetPlayer = GameObject.FindGameObjectWithTag ("Player");
			card.cActions.UseToPlayer (targetPlayer);
		}

		if (who == playerTypes.AI) {
			GameObject targetPlayer = GameObject.FindGameObjectWithTag ("AIPlayer");
			card.cActions.UseToPlayer (targetPlayer);
		}
	}

	// This is called by the Player:
	public void AttackEnemyPlayer(){
		if (curSelected != null) {
			if (curSelected.canBeUsedTo == V_Card.usage.All || curSelected.canBeUsedTo == V_Card.usage.BaseOnly) {
				AttackPlayer (curSelected, playerTypes.AI);
				curSelected = null;
			} else {
				// Display error if the selected card is not usable to a player base:
				DisplayError(cantBeUsedToBase);
			}
		}
	}

	// This is called by the AI:
	public void AI_AttackEnemyPlayer(){
		if (aiCurSelected != null) {
			// if the card is usable to enemy bases then attack...
			if (aiCurSelected.canBeUsedTo == V_Card.usage.All || aiCurSelected.canBeUsedTo == V_Card.usage.BaseOnly) {
				AttackPlayer (aiCurSelected, playerTypes.Player);
				aiCurSelected = null;
			// ...else, restart the action:
			} else {
				V_AI ai = GameObject.Find("_AI").GetComponent<V_AI> ();
				ai.AITurn ();
			}
		}
	}

	public void CallGameResult(bool victory){
		resultPanel.SetActive (true);

		// Get the Image component of the first child of resultPanel:
		Image banner = resultPanel.transform.GetChild (0).GetComponent <Image>();
		if (victory == true) {
			banner.sprite = victoryBanner;
		} else {
			banner.sprite = failedBanner;
		}
	}

	public void PlayerRedraw(){
        //change this into drawing up to twice during the draw phase.
        if (V_GameManager.cardgamestate == V_GameManager.currentState.draw)
        {
            p.DrawOneCard();
        }
		//p.ReDraw ();
	}
	public void PlayerEndturn(){
		p.EndTurn ();
	}

    public void ConvertToEnergy()
    {
        if(curSelected != null)
        {
            V_PlayerHandler.energy += curSelected.energyCost;
            Destroy(curSelected.gameObject);
        }
    }
	public void BackToScene(string sceneName){
		SceneManager.LoadScene (sceneName);
	}
	public void DisplayError (string error){
		// Display error if the selected card is not usable to a player base:
		GameObject errorObj = Instantiate (errorText, gameArea.transform) as GameObject;
		errorObj.GetComponent<Text> ().text = error;
	}
}
