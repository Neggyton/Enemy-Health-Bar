using System;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterface;


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
        HealthBarSettings barSettings;


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

            barSettings = new HealthBarSettings();

            DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.Components.Add(healthBar);
        }

        private void Update()
        {
            if (GameManager.Instance.IsPlayingGame() && healthBar != null)
                if (healthBar.hitNPC)
                    healthBar.Update();

            if (InputManager.Instance.GetKeyDown(KeyCode.X))
            {
                switch (healthBar.hitNPC != null)
                {
                    case (true):
                        healthBar.hitNPC = null;
                        break;
                    case (false):
                        healthBar.hitNPC = GameManager.Instance.PlayerEntityBehaviour;
                        break;
                }

                Debug.Log(healthBar.hitNPC);
            }

            healthBar.offset = barSettings.NewPos(healthBar.offset);
            healthBar.scaleOffset = barSettings.NewScale(healthBar.scaleOffset);
            Debug.Log(healthBar.scaleOffset);
            healthBar.PanelSetup();
            healthBar.timerReset = true;
        }

        private void OnGUI()
        {
            if (GameManager.Instance.IsPlayingGame() && healthBar != null)
                if (healthBar.hitNPC)
                    healthBar.Draw();
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
            if (healthBar != null)
            {
                healthBar.hitNPC = null;
                healthBar.Update();
            }
        }
    }
}
