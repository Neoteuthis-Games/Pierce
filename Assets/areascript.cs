using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class areascript : MonoBehaviour
{
    public string mylocation = "TOP SECRET AREA";
    public string locationTag = "Area1";
    public overworldmanagerscript.LevelType mytype = overworldmanagerscript.LevelType.Duel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log("Mouse is over GameObject.");
        V_GameManager.CurrentLocation = mylocation;
    }
   void OnMouseDown()
    {
        overworldmanagerscript.ActivateArea(mylocation, locationTag, mytype);
    }

    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Mouse is no longer on GameObject.");
        V_GameManager.CurrentLocation = "";
    }
}
