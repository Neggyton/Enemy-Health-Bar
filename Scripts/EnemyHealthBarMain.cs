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

        bool barActive = false;
        ModSettings settings;

        public int location { get; private set; }
        public int scale { get; private set; }

        HealthBar healthBar = null;


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
            mod.IsReady = true;
            
        }

        private void Start()
        {
            settings = mod.GetSettings();

            GameManager.OnEnemySpawn += OnEnemySpawn;
            EntityHitRegister.TargetNPC += OnTargetNPC;

            SaveLoadManager.OnLoad += RaiseOnLoadEvent;
            HealthBar.Kill += OnKill;

        }

        private void Update()
        {
            if (GameManager.Instance.IsPlayingGame())
                if (healthBar.hitNPC)
                {
                    healthBar.Update();
                }
        }

        private void OnGUI()
        {
            if (GameManager.Instance.IsPlayingGame())
            {
                if (healthBar.hitNPC)
                {
                    healthBar.Draw();
                }
            } 
        }


        //This code checks to see if an enemy is hit by the Player.

        public void OnEnemySpawn(GameObject enemy)
        {
            healthBar = new HealthBar();
            Debug.Log(healthBar.Position);
            healthBar.Scale = DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.Scale;
            healthBar.Size = DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.Size;
            healthBar.loc = 0; //settings.GetValue<int>("Location", "BarLocation");
            healthBar.scaleSettings = settings.GetValue<int>("Health Bar Size", "BarSize");

            DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.Components.Add(healthBar);
            GameManager.OnEnemySpawn -= OnEnemySpawn;
        }
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
            healthBar.hitNPC = null;

            DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.Components.Remove(healthBar);
        }

        public void OnKill()
        {
            DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.Components.Remove(healthBar);
        }

    }
}
