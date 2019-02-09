/* Script Created by Mathew Kelly
 * Handles adding cards to the graveyard as well as removing them
 * 
 * Revision
 *      Mathew Kelly 02/08/2019 dd/mm/yyyy
 *      Geordon Martin 02/09/2019 dd/mm/yyyy
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V_Graveyard : MonoBehaviour
{
    private List<GameObject> graveyardList = new List<GameObject>();
    // This will give you the list of cards in the graveyard if we need it
    // elsewhere but any changes on it must be done through add and remove
    private bool InfiniteLoop = true;

    public List<GameObject> GraveyardList
    {
        get { return graveyardList; }
    }

    public void AddToGraveyard(GameObject cardAdded)
    {
        graveyardList.Add(cardAdded);
        if(InfiniteLoop == true)
        Cleanup();
    }

    public void RemoveFromGraveYard(GameObject cardToBeRemoved)
    {
        graveyardList.Remove(cardToBeRemoved);
        Cleanup();
    }

   public void MoveCard()
    {
        Cleanup();  
    }

    void Cleanup()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("InGrave");
        foreach (GameObject o in obj)
        {
            InfiniteLoop = false;
           // AddToGraveyard(o);
            o.SetActive(false);
            Debug.Log(o.GetComponent<V_Card>().cardName);
        }
        InfiniteLoop = true;
      Array boople = graveyardList.ToArray();
        int gravelength = boople.Length;
        Debug.Log(gravelength);
        graveyardList[gravelength-1].SetActive(true);
    }
}
