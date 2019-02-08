/* Script Created by Mathew Kelly
 * Handles adding cards to the graveyard as well as removing them
 * 
 * Revision
 *      Mathew Kelly 02/08/2019 dd/mm/yyyy
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V_Graveyard : MonoBehaviour
{
    private List<V_Card> graveyardList = new List<V_Card>();
    // This will give you the list of cards in the graveyard if we need it
    // elsewhere but any changes on it must be done through add and remove

    public List<V_Card> GraveyardList
    {
        get { return graveyardList; }
    }

    void AddToGraveyard(V_Card cardAdded)
    {
        graveyardList.Add(cardAdded);
    }

    void RemoveFromGraveYard(V_Card cardToBeRemoved)
    {
        graveyardList.Remove(cardToBeRemoved);
    }

    void MoveCard()
    {
        
    }
}
