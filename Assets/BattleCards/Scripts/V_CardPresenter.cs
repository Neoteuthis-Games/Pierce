using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
///      CardPresenter script for "BattleCards: CCG Adventure Template"
/// 
/// "This script is used to present a card for deck customization or cards
/// viewing..."
/// 
/// To clarify some comments, here are the reference for the TERMS:
/// 
///    Our    - Refers to this instance/GameObject (not the main player).
///    Player - Refers to the player.
///    
///            Added: 2016
/// </summary>


public class V_CardPresenter : MonoBehaviour {

	[HideInInspector]
	public int index;
	public Transform cardPreviewHolder;
	[Header(" Mode:")]
	public bool OnDeck = false;     // if this will be used to present cards in 'deck' or cards in inventory/database

	// Internals:
	GameObject toolTip;
	V_DeckEditor deckEditor;
	Image thisImg;

	// Miscs:
	public bool IsCursorInZone(Vector2 cursor, GameObject zone)
	{
		Vector2 localPos = zone.transform.InverseTransformPoint(cursor);
		return ((RectTransform) zone.transform).rect.Contains(localPos);
	}

	// Use this for initialization
	void Start () {
		// Prepare some references:
		toolTip = GameObject.FindGameObjectWithTag ("ToolTip");
		deckEditor = FindObjectOfType<V_DeckEditor> ();
		thisImg = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		// Let's do tooltips!
		if (EventSystem.current.IsPointerOverGameObject ()){
			if (IsCursorInZone (Input.mousePosition, gameObject)) {
				toolTip.SetActive (true);
				toolTip.transform.position = new Vector3 (Input.mousePosition.x +  toolTip.GetComponent<RectTransform>().rect.width, Input.mousePosition.y - toolTip.GetComponent<RectTransform>().rect.height, 0f);
				toolTip.transform.GetChild (0).GetComponent<Text> ().text = V_CardCollections.cards [index].cardDescription;
			}
		} else {
            if(toolTip!=null)
			toolTip.SetActive (false);
		}

		if (V_DeckEditor.selectedCard == this) {
			thisImg.color = Color.cyan;
		} else {
			thisImg.color = Color.gray;
		}
	}

	public void Click(){
		if (OnDeck) {
			if (V_DeckEditor.selectedCard == this) {
				deckEditor.Deselect ();
			} else {
				deckEditor.Select (this);
			}
		} else {
			deckEditor.SetCard (deckEditor.selectedDeck, index);
		}
	}

	public void Refresh(){
		if (cardPreviewHolder) {
			foreach (Transform t in cardPreviewHolder) {
				Destroy (t.gameObject);
			}
			Instantiate (FindObjectOfType<V_CardCollections> ().gameCards [index], cardPreviewHolder);
		}
	}
}
