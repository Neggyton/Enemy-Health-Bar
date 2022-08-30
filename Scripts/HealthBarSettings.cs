using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

public class HealthBarSettings
{
    Vector2 scale = DaggerfallUI.Instance.DaggerfallHUD.NativePanel.Scale;

    public void HealthBarPosition(Panel bar, Vector2 offset)
    {
        bar.Position = bar.Position + offset;
    }

    public Vector2 NewPos(Vector2 newOffset)
    {

        Vector2 multiplier = new Vector2(1,1);
        if (InputManager.Instance.GetKey(KeyCode.LeftShift) || InputManager.Instance.GetKey(KeyCode.RightShift))
            multiplier = 10 * scale;
        switch (InputManager.Instance.GetAnyKeyDown())
        {
            case (KeyCode.UpArrow):
                newOffset.y += multiplier.y;
                break;
            case (KeyCode.DownArrow):
                newOffset.y -= multiplier.y;
                break;
            case (KeyCode.RightArrow):
                newOffset.x += multiplier.x;
                break;
            case (KeyCode.LeftArrow):
                newOffset.x -= multiplier.x;
                break;
        }
        return newOffset;
    }

    public int NewScale(int scaleOffset)
    {

        switch (InputManager.Instance.GetAnyKeyDown())
        {
            case (KeyCode.Minus):
                scaleOffset -= 1;
                break;
            case (KeyCode.Equals):
                scaleOffset += 1;
                break;
        }

        return scaleOffset;
    }

    public void MessageBox()
    {
        DaggerfallMessageBox tempInfoBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
        tempInfoBox.PauseWhileOpen = true;
        tempInfoBox.AllowCancel = false;
        tempInfoBox.ClickAnywhereToClose = true;
        tempInfoBox.ParentPanel.BackgroundColor = Color.clear;
    
    string[] message = new string[] {"Welcome to the Enemy Health Bar settings.", "Use the Arrow keys to move the health bar around.", "You can move the bar around faster by holding down Shift.", "Press the '-' and '=' keys to change the size of the bar.",
            "Press the Delete key to reset the position of the bar.", "Your options will automatically be saved; press the '.' key again to exit this menu.", "Your settings will be loaded even if the Advanced setting is turned off in the mod options."};
        tempInfoBox.SetText(message);
        DaggerfallUI.UIManager.PushWindow(tempInfoBox);

    }
}
