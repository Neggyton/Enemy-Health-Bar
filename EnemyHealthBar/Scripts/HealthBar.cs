using System;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Utility;
using HealthBarMod;
using Wenzil.Console.Commands;

namespace HealthBarMod
{
    public class HealthBar : MonoBehaviour
    {




        public HorizontalProgress Health;
        public Vector2 baseSize;
        const string baseTextName = "HealthBar2.png";
        Texture2D baseTexture;
        float origPercent;
        float newPercent;
        Panel Back;
        const string backTextName = "Background.png";
        Texture2D backTexture;
        public bool destroy;
        DaggerfallHUD hud;
        float timer = 15;
        public bool reset = false;
        DaggerfallEntity hitNPC = null;

        static HealthBar instance = null;


        public static HealthBar Instance
        {
            get
            {
                if (instance == null)
                {

                    GameObject go = new GameObject();
                    go.name = "HealthBar";
                    instance = go.AddComponent<HealthBar>();

                }


                return instance;
            }
        }





        private void Start()
        {
            hitNPC = EnemyHealthBarMain.Instance.hitNPC.Entity;
            hud = DaggerfallUI.Instance.DaggerfallHUD;
            LoadTextures();

            origPercent = hitNPC.CurrentHealthPercent;
            newPercent = hitNPC.CurrentHealthPercent;


            Health = new HorizontalProgress();
            baseSize = new Vector2(100, 6);
            Back = new Panel();
            destroy = false;
            Health.Size = baseSize;
            Health.BackgroundColor = Color.clear;
            Health.HorizontalAlignment = HorizontalAlignment.Center;
            Health.VerticalAlignment = VerticalAlignment.None;
            

            Health.Position = new Vector2(0, 192);


            Health.ProgressTexture = baseTexture;
            //Health.BackgroundTexture = baseTexture;

            Back.Size = new Vector2(104, 10);
            Back.BackgroundColor = Color.clear;
            Back.HorizontalAlignment = HorizontalAlignment.Center;
            Back.VerticalAlignment = VerticalAlignment.None;
            Back.Position = new Vector2(0, 190);
            Back.BackgroundTexture = backTexture;


            hud.NativePanel.Components.Add(Back);
            hud.NativePanel.Components.Add(Health);
        }

        private void Update()
        {
            Health.Amount = hitNPC.CurrentHealthPercent;
            TimeUpdate();


                newPercent = EnemyHealthBarMain.Instance.hitNPC.Entity.CurrentHealthPercent;

            if (GameManager.Instance.SaveLoadManager.LoadInProgress
                || GameManager.Instance.PlayerDeath.DeathInProgress
                || newPercent <= 0
                || timer <= 0
                || EnemyHealthBarMain.Instance.hitNPC.Entity.WorldContext != GameManager.Instance.PlayerEnterExit.WorldContext
                || GameManager.Instance.PlayerEntity.IsResting)
            {
                hud.NativePanel.Components.Remove(Health);
                hud.NativePanel.Components.Remove(Back);
                Destroy(gameObject);
            }

        }

        void LoadTextures()
        {
            if (!TextureReplacement.TryImportImage(baseTextName, true, out baseTexture))
            {
                Debug.LogError("TravelOptions: Unable to load the base UI image.");
            }
            if (!TextureReplacement.TryImportImage(backTextName, true, out backTexture))
            {
                Debug.LogError("TravelOptions: Unable to load the base UI image.");
            }
        }





        void TimeUpdate()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            if (reset)
            {
                timer = 15;
                reset = false;
            }
        }
    }
}