﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventHandler : MonoBehaviour
{
       public delegate void CardAction();
    public static event CardAction CardPlayed;
    public static event CardAction CardAttacks;
    public static event CardAction CardDestroyed;
    public static event CardAction CardPurged;
    public static event CardAction CardViewed;

    public delegate void CardGameAction();
    public static event CardGameAction EndTurn;
    public static event CardGameAction Interrupt;

    public delegate void DungeonAction();
    public static event DungeonAction MoveUp;
    public static event DungeonAction MoveLeft;
    public static event DungeonAction MoveRight;
    public static event DungeonAction MoveDown;
    public static event DungeonAction Idle;
    public static event DungeonAction Scan;

    public delegate void OtherAction();
    public static event OtherAction Pause;
    public static event OtherAction UnPause;
    public static event OtherAction Scroll;
    public static EventHandler Eventinstance;
    public V_GameManager GameManager;
    // Start is called before the first frame update
    void Start()
    {
         //singleton
        if (Eventinstance == null) { Eventinstance = this; }
        else { Destroy(this); }
        //no kill
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (true)
            if (Input.GetKey(KeyCode.W))
            {
                if (V_GameManager.ActiveState == V_GameManager.Gamestate.Dungeon)
                {
                    MoveUp();
                }
            }
        if (Input.GetKey(KeyCode.S))
        {
            if (V_GameManager.ActiveState == V_GameManager.Gamestate.Dungeon)
            {
                MoveDown();
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (V_GameManager.ActiveState == V_GameManager.Gamestate.Dungeon)
            {
                MoveRight();
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (V_GameManager.ActiveState == V_GameManager.Gamestate.Dungeon)
            {
                MoveLeft();
            }
        }
        if (V_GameManager.ActiveState == V_GameManager.Gamestate.Dungeon && Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.D) == false && Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.S) == false)
        {
            Idle();
        }
       
        if (Input.GetKey(KeyCode.E))
        {
            if (V_GameManager.ActiveState == V_GameManager.Gamestate.Dungeon)
            {
                Scan();
            }
        }
        if (Input.GetKeyUp(KeyCode.P) || Input.GetKeyUp(KeyCode.Escape))
        {

            if (V_GameManager.ActiveState == V_GameManager.Gamestate.Dungeon)
            {
                V_GameManager.ActiveState = V_GameManager.Gamestate.Paused;
                Pause();
            }
            if (V_GameManager.ActiveState == V_GameManager.Gamestate.Paused)
            {
                V_GameManager.ActiveState = V_GameManager.Gamestate.Dungeon;
                UnPause();
            }
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if (V_GameManager.ActiveState == V_GameManager.Gamestate.InGame)
            {
                //pause the card game so players can read cards or respond to an action
                Interrupt();
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (V_GameManager.ActiveState == V_GameManager.Gamestate.Story)
            {
                //go to the next dialogue box or end story scene.
                Scroll();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (V_GameManager.ActiveState == V_GameManager.Gamestate.InGame)
            {
                //will show an enlarged version of the selected card
                CardViewed();
            }

        }
    }
    public void Play()
    {
        CardPlayed();
    }
    public void Attack()
    {
        CardAttacks();
    }
    public void Purge()
    {
        CardPurged();
    }
    public void Destroy()
    {
        CardDestroyed();
    }
    public void End()
    {
        EndTurn();
    }
    public static EventHandler GetInstance() { return Eventinstance; }
}
