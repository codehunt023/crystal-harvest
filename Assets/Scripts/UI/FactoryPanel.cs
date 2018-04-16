﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class FactoryPanel : MonoBehaviour
{
    public SelectionPanel SelectionPanel;

    public Factory Factory { get; protected set; }

    public ProgressBar ProgressBar;

    List<ResourceIconSlot> inputSlots;
    List<ResourceIconSlot> outputSlots;

    List<GameObject> icons;

    List<int> tempRequiredResources;
    List<int> tempResources;

    public Text TextSubpanel;

    public GameObject RobotIconPrefab;

    void Awake()
    {
        ResourceIconSlot[] slotsArray = this.transform.GetComponentsInChildren<ResourceIconSlot>();

        if (slotsArray == null || slotsArray.Length == 0)
        {
            Debug.Log("Panel nie ma żadnych slotów na zasoby!");
        }

        inputSlots = new List<ResourceIconSlot>();
        outputSlots = new List<ResourceIconSlot>();

        for (int i = 0; i < slotsArray.Length; i++)
        {
            if (slotsArray[i].Type == ResourceIconSlotType.Input)
            {
                inputSlots.Add(slotsArray[i]);
            }
            else if (slotsArray[i].Type == ResourceIconSlotType.Output)
            {
                outputSlots.Add(slotsArray[i]);
            }
        }

        inputSlots.OrderBy(slot => slot.Order);
        outputSlots.OrderBy(slot => slot.Order);

        icons = new List<GameObject>();

        tempRequiredResources = new List<int>();
        tempResources = new List<int>();
    }

    void Update()
    {
        if (Factory == null || Factory.Building.IsDeconstructed)
        {
            SelectionPanel.RemoveSelection();
            return;
        } 

        if (Factory.Halted)
        {
            ProgressBar.SetFillPercentageWithoutText(Factory.GetCompletionPercentage());
            ProgressBar.SetText(GameManager.Instance.GetText("s_halted"));
        }
        else
        {
            ProgressBar.SetFillPercentage(Factory.GetCompletionPercentage());
        }
       
        SelectionPanel.HideResourceIcons(icons);

        if (Factory.Prototype.ConsumedResources != null)
        {
            tempRequiredResources = SelectionPanel.GetResourcesList(Factory.Prototype.ConsumedResources);
            tempRequiredResources.Sort();
            tempResources = SelectionPanel.GetResourcesList(Factory.InputStorage.Resources);
            SelectionPanel.ShowIconsWithRequirements(inputSlots, tempRequiredResources, tempResources, icons);
        }

        if (Factory.Prototype.ProducedResources != null)
        {
            tempRequiredResources = SelectionPanel.GetResourcesList(Factory.Prototype.ProducedResources);    
            tempRequiredResources.Sort();

            tempResources = SelectionPanel.GetResourcesList(Factory.OutputStorage.Resources);           
            foreach (Character c in Factory.OutputStorage.ReservedResources.Keys)
            {
                tempResources.Add(Factory.OutputStorage.ReservedResources[c]);
            }

            SelectionPanel.ShowIconsWithRequirements(outputSlots, tempRequiredResources, tempResources, icons);
        }

        if (Factory.ProducesRobot)
        {
            GameObject icon = SimplePool.Spawn(RobotIconPrefab,
                                           outputSlots[0].transform.position,
                                           Quaternion.identity);
            icon.transform.SetParent(outputSlots[0].transform);
            icons.Add(icon);
        }
    }

    public void SetFactory(Factory f)
    {
        Factory = f;
        if (f != null && f.Building != null)
        {
            TextSubpanel.text = f.Building.Name;
            Update();
        }        
    }    
}
