﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetDestinationUI : MonoBehaviour
{
    public TMP_Text _DestinationDistance;
    public TMP_Text _DestinationName;
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayTargetInformation(string destinationName, string targetDistance)
    {
        _DestinationName.text = destinationName;
        _DestinationDistance.text = targetDistance + " m";
    }

    public void UpdateDistance(string targetDistance)
    {
        _DestinationDistance.text = targetDistance + " m";
    }
}
