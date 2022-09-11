using System;
using System.Collections.Generic;
using System.Linq;
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

namespace HealthBarMod
{
    public abstract class LookUpTable
    {

        public static Dictionary<string, Texture2D[]> enemyDict = new Dictionary<string, Texture2D[]>();


        public static void TableCreation()
        {
            //Dictionary where Key is the enemy's name and Value is the assigned textures.
            int y = 0;
            int x = 0;
            foreach (MobileTypes i in (MobileTypes[])Enum.GetValues(typeof(MobileTypes)))
            {
                if (i == MobileTypes.None)
                    continue;
                Debug.Log(i);
                enemyDict.Add(Enum.GetName(typeof(MobileTypes), y), TextureAssignment(EnemyBasics.Enemies[x], Convert.ToString(i)));
                y++;
                x++;
                if (y > 42 && y < 128)
                    y = 128;
            }
        }


        private static Texture2D[] TextureAssignment(MobileEnemy npc, string name)
        {
            string healthbar = "healthbar";
            string backdrop = "backdrop";
            string hb = "_hb";
            string bd = "_bd";
            Texture2D[] setTexture = new Texture2D[2];
            string[] setStringBody = new string[2] { healthbar, backdrop };
            string[] setStringEnd = new string[2] { hb, bd };
            var zip = setStringBody.Zip(setStringEnd, (x, y) => new { setStringBody = x, setStringEnd = y });

            int i = 0;
            foreach (var xy in zip)
            {
                SetTexture(xy.setStringEnd, npc, name, out setTexture[i++]);
            }
            return setTexture;
        }

        private static void SetTexture(string ending, MobileEnemy npc, string npcName, out Texture2D texture)
        {
            //system doesn't like the name 'default' for a variable
            string dfault = "default";
            string team = npc.Team.ToString().ToLower() + "_";
            string name = npcName.ToLower();

            if (TextureReplacement.TryImportImage(team + name + ending, true, out texture)) { }
            else if (TextureReplacement.TryImportImage(team + dfault + ending, true, out texture)) { }
            else if (!TextureReplacement.TryImportImage(dfault + ending, true, out texture))
                Debug.LogError("Unable to import a healthbar texture");


        }
    }
}