using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game;

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
        if (InputManager.Instance.GetKey(KeyCode.LeftShift))
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
            case (KeyCode.Alpha9):
                scaleOffset -= 1;
                break;
            case (KeyCode.Alpha0):
                scaleOffset += 1;
                break;
        }

        return scaleOffset;
    }

}
