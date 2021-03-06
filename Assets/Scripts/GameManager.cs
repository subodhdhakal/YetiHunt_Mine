﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject playerInstantiationPoint;
    bool isSpawning = false;

    public event EventHandler GameOverEvent;

    private void OnGameOver()
    {

        if (GameOverEvent != null)
            GameOverEvent(this, EventArgs.Empty);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isSpawning)
            StartCoroutine(SpawnPlayerRoutine());
    }

    IEnumerator SpawnPlayerRoutine()
    {
        isSpawning = true;
        //Instantiate(player, playerInstantiationPoint.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.0f);
        isSpawning = false;
    }
}
