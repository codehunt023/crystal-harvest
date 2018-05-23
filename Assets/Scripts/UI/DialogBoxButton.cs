﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogBoxButton : MonoBehaviour 
{
    public DialogBox DialogBox;
    public Text Text;
    public Button Button;

    public void SetAction(Action action)
    {
        Button.onClick.RemoveAllListeners();
        if (action != null)
        {
            Button.onClick.AddListener(() => { action(); });
        }
        Button.onClick.AddListener(() => { DialogBox.ButtonClicked(); });

        Button.onClick.AddListener(() => { DialogBox.SoundManager.PlayButtonSound(); });
    }

    public void SetText(string text)
    {
        Text.text = text;
    }
}
