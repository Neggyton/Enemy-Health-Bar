using System;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Utility.AssetInjection;
using System.IO;
using DaggerfallWorkshop.Utility;

public class LookUpTable{

    Texture2D value;
    Dictionary<string, HashSet<Texture2D>> enemyDict = new Dictionary<string, HashSet<Texture2D>>();
    public LookUpTable()
    {
        int y = 0;
        foreach (MobileTypes i in (MobileTypes[]) Enum.GetValues(typeof(MobileTypes))){
            enemyDict.Add(Enum.GetName(typeof(MobileTypes), y), TextureFinder(EnemyBasics.Enemies[y]));
            y++;
        }

        
        //enemyDictReverse = 
        //enemyDictReverse = enemyDict.ToDictionary(x => x.Value.ToString(), x => x.Key);
        //Debug.Log(enemyDictReverse["Rat"]);
    }


    public HashSet<Texture2D> TextureFinder(MobileEnemy npc)
    {
        Debug.Log(npc.Team);
        return null;
    }
}
