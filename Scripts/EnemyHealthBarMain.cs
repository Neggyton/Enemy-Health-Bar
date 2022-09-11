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

        private bool advancedSettings;

        public int activated = 0;



        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            Instance = go.AddComponent<EnemyHealthBarMain>();
            go.AddComponent<EntityHitRegister>();

            mod.IsReady = true;

        }

        public EnemyHealthBarMain()
        {
            EntityHitRegister.TargetNPC += OnTargetNPC;
            SaveLoadManager.OnLoad += RaiseOnLoadEvent;
        }

        private void Start()
        {
            settings = mod.GetSettings();
            advancedSettings = settings.GetBool("Advanced Location Positioning and Scaling", "Enabled");

            healthBar = new HealthBar(new Vector2(PlayerPrefs.GetFloat("BarPositionX"), PlayerPrefs.GetFloat("BarPositionY")), PlayerPrefs.GetInt("BarScale"));
            healthBar.scaleSettings = settings.GetValue<int>("Health Bar Size", "BarSize");
            healthBar.AutoSize = AutoSizeModes.None;
            healthBar.VerticalAlignment = VerticalAlignment.Middle;
            healthBar.HorizontalAlignment = HorizontalAlignment.Center;

            barSettings = new HealthBarSettings();

            DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.Components.Add(healthBar);
        }

        private void Update()
        {
            healthBar.Update();

            if (advancedSettings)
                BarSetting();
        }

        private void BarSetting()
        {

            if (Input.GetKeyDown(KeyCode.Backslash))
            {
                switch (activated)
                {
                    case 0:
                        activated++;
                        healthBar.hitNPC = GameManager.Instance.PlayerEntityBehaviour;
                        barSettings.MessageBox();
                        break;
                    case 2:
                        healthBar.hitNPC = null;

                        InputManager.Instance.enabled = true;
                        advancedSettings = !advancedSettings;
                        break;
                }
            }


            if (activated > 0)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    healthBar.offset = new Vector2(0, 2);
                    healthBar.scaleOffset = 0;
                }

                healthBar.offset = barSettings.NewPos(healthBar.offset);
                healthBar.scaleOffset = barSettings.NewScale(healthBar.scaleOffset);
                healthBar.timerReset = true;

                PlayerPrefs.SetFloat("BarPositionX", healthBar.offset.x);
                PlayerPrefs.SetFloat("BarPositionY", healthBar.offset.y);
                PlayerPrefs.SetInt("BarScale", healthBar.scaleOffset);
                PlayerPrefs.Save();

            }

        }

        public void OnTargetNPC(DaggerfallEntityBehaviour target)
        {
            healthBar.timerReset = true;
            if (target != healthBar.hitNPC)
            {
                healthBar.hitNPC = target;

            }

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
