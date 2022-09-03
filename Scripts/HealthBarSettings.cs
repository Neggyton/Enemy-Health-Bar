using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

public class HealthBarSettings
{
    Vector2 scale = DaggerfallUI.Instance.DaggerfallHUD.NativePanel.Scale;
    DaggerfallMessageBox tempInfoBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);

    public HealthBarSettings()
    {
        tempInfoBox.OnClose += OnClose;
    }
    public void HealthBarPosition(Panel bar, Vector2 offset)
    {
        bar.Position = bar.Position + offset;
    }

    public Vector2 NewPos(Vector2 newOffset)
    {

        Vector2 multiplier = new Vector2(1,1);
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.W))
                newOffset.y++;
            else if (Input.GetKeyDown(KeyCode.S))
                newOffset.y--;
            if (Input.GetKeyDown(KeyCode.D))
                newOffset.x++;
            else if (Input.GetKeyDown(KeyCode.A))
                newOffset.x--;
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
                newOffset.y++;
            else if (Input.GetKey(KeyCode.S))
                newOffset.y--;
            if (Input.GetKey(KeyCode.D))
                newOffset.x++;
            else if (Input.GetKey(KeyCode.A))
                newOffset.x--;
        }
            
       
        

        return newOffset;
    }

    public int NewScale(int scaleOffset)
    {

        if (Input.GetKeyDown(KeyCode.Q))
            scaleOffset--;
        else if (Input.GetKeyDown(KeyCode.E))
            scaleOffset++;
        

        return scaleOffset;
    }

    public void MessageBox()
    {
        tempInfoBox.PauseWhileOpen = true;
        tempInfoBox.AllowCancel = false;
        tempInfoBox.ClickAnywhereToClose = true;
        tempInfoBox.ParentPanel.BackgroundColor = Color.clear;
    
    string[] message = new string[] {"Welcome to the Enemy Health Bar settings.", "Use WASD to move the health bar around.", "You can move the bar around more precisely by holding down Shift.", "Use Q and E to change the size of the bar.",
            "Press the Spacebar to reset the position of the bar.", "Your options will automatically be saved; press the '.' (Period) key again to exit this menu.", "When finished, uncheck 'Advanced Location Positioning and Scaling' from the Mod Settings page."};
        tempInfoBox.SetText(message);
        DaggerfallUI.UIManager.PushWindow(tempInfoBox);
    }

    public void OnClose()
    {
        InputManager.Instance.enabled = false;
    }
}
