using System;
using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;


namespace HealthBarMod
{
    public class EnemyHealthBarMain : MonoBehaviour
    {

        public static EnemyHealthBarMain Instance { get; private set; }

        private static Mod mod;


        public int location { get; private set; }
        public int scale { get; private set; }

        HealthBar healthBar = null;
        ModSettings settings;


        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            Instance = go.AddComponent<EnemyHealthBarMain>();
            go.AddComponent<EntityHitRegister>();

            mod.IsReady = true;
        }

        private void Start()
        {

            mod.IsReady = true;
            settings = mod.GetSettings();

            EntityHitRegister.TargetNPC += OnTargetNPC;
            SaveLoadManager.OnLoad += RaiseOnLoadEvent;

            healthBar = new HealthBar();
            healthBar.Scale = DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.LocalScale;
            healthBar.Size = DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.Size;
            healthBar.loc = 0; //settings.GetValue<int>("Location", "BarLocation");
            healthBar.scaleSettings = settings.GetValue<int>("Health Bar Size", "BarSize");
            healthBar.AutoSize = AutoSizeModes.ScaleToFit;
            healthBar.Parent = DaggerfallUI.Instance.DaggerfallHUD.ParentPanel;

        }

        private void Update()
        {
            if (GameManager.Instance.IsPlayingGame() && healthBar != null)
                if (healthBar.hitNPC)
                    healthBar.Update();
        }

        private void OnGUI()
        {
            if (GameManager.Instance.IsPlayingGame() && healthBar != null)
                if (healthBar.hitNPC)
                    healthBar.Draw();
        }


        //This code checks to see if an enemy is hit by the Player.

        public DaggerfallEntityBehaviour OnTargetNPC(DaggerfallEntityBehaviour target)
        {
            healthBar.timerReset = true;
            if (target != healthBar.hitNPC)
            {
                healthBar.hitNPC = target;
                return healthBar.hitNPC;
            }
            return healthBar.hitNPC;
        }

        public void RaiseOnLoadEvent(SaveData_v1 saveData)
        {
            if (healthBar != null)
                healthBar.hitNPC = null;

        }
    }
}
