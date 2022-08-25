using System;
using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Weather;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Guilds;


namespace HealthBarMod
{
    public class EnemyHealthBarMain : MonoBehaviour
    {

        public static EnemyHealthBarMain Instance { get; private set; }

        private static Mod mod;

        DaggerfallEntityBehaviour hitNPC = null;
        


        public int location { get; private set; }
        public int scale { get; private set; }

        HealthBar healthBar;

        public int test;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            Instance = go.AddComponent<EnemyHealthBarMain>();
            go.AddComponent<EntityHitRegister>();

            mod.IsReady = true;
        }



        private void Awake()
        {
            ModSettings settings = mod.GetSettings();
            mod.IsReady = true;

            

        }

        private void Start()
        {
            healthBar = new HealthBar();
            healthBar.loc = 0; //settings.GetValue<int>("Location", "BarLocation");
            //healthBar.scaleSettings = settings.GetValue<int>("Health Bar Size", "BarSize");

            hitNPC = healthBar.hitNPC;
            EntityHitRegister.TargetNPC += OnTargetNPC;
            Debug.Log("This constructor ran");

            DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.Components.Add(healthBar);
        }

        private void Update()
        {
            Debug.Log(DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.Size);
            if (healthBar.hitNPC)
                healthBar.Update();
        }

        private void OnGUI()
        {
            if(healthBar.hitNPC)
                healthBar.Draw();
        }

        public DaggerfallEntityBehaviour OnTargetNPC(DaggerfallEntityBehaviour target)
        {
            healthBar.hitNPC = target;
            return healthBar.hitNPC;
        }
        //This code checks to see if an enemy is hit by the Player.


    }
}
