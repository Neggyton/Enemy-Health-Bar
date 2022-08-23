using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Utility;
using HealthBarMod;

namespace HealthBarMod
{
    public class HealthBar : MonoBehaviour
    {


        DaggerfallHUD hud = DaggerfallUI.Instance.DaggerfallHUD;
        MobilePersonNPC villagerNpc = null;
        DaggerfallEntityBehaviour hitNPC = null;
        DaggerfallEntityBehaviour origNPC = null;
        HorizontalProgress Health;
        Panel Back;
        Texture2D healthTexture;
        Texture2D backTexture;
        WorldContext storedContext;

        Vector2 healthSize = new Vector2(512,64);
        Vector2 backSize = new Vector2(512, 64);


        string healthTextName;
        string backTextName;
        private float timer = 15;
        public bool reset = false;
        public bool isVillager { get; private set; }

        static HealthBar instance = null;
        
        Vector2 scaleVec;
        float scale;
        bool unload = false;




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
            

            int scaleSettings = EnemyHealthBarMain.Instance.scale;
            scale = SetScale(scaleSettings);
            Back = new Panel();
            Health = new HorizontalProgress();
            origNPC = EnemyHealthBarMain.Instance.hitNPC;
            hitNPC = origNPC;
            storedContext = GameManager.Instance.PlayerEnterExit.WorldContext;


            if (villagerNpc)
            {
                if (hitNPC.EntityType == EntityTypes.CivilianNPC && !villagerNpc.IsGuard)
                {
                    isVillager = true;
                }
            }
            else
                isVillager = false;



            LoadTextures();
            Setup();

            


            hud.ParentPanel.Components.Add(Back);
            hud.ParentPanel.Components.Add(Health);
        }

        private void Update()
        {
            if (isVillager
                || !hitNPC
                || GameManager.Instance.PlayerDeath.DeathInProgress
                || !hitNPC.gameObject.activeSelf
                || timer <= 0
                || GameManager.Instance.PlayerEntity.IsResting
                || storedContext != GameManager.Instance.PlayerEnterExit.WorldContext
                || (!GameManager.Instance.IsPlayingGame() && GameManager.Instance.StateManager.CurrentState != StateManager.StateTypes.UI))
            {

                hud.ParentPanel.Components.Remove(Health);
                hud.ParentPanel.Components.Remove(Back);
                Destroy(gameObject);
            }

            villagerNpc = EnemyHealthBarMain.Instance.villagerNpc;
            hitNPC = EnemyHealthBarMain.Instance.hitNPC;

            if (hitNPC && origNPC.GetInstanceID() != hitNPC.GetInstanceID())
            {
                LoadTextures();
                Setup();
                origNPC = hitNPC;
            }
            Health.Amount = hitNPC.Entity.CurrentHealthPercent;
            TimeUpdate();
        }

        void LoadTextures()
        {
            if (hitNPC.Entity.Team != MobileTeams.Undead)
            {
                healthTextName = "BloodHP(dags).png";
                backTextName = "BloodHP(dags)_BD.png";
                
            }
            else
            {
                healthTextName = "BloodHP(dags).png";
                backTextName = "BloodHP(dags)_BD.png";
            }
            if (!TextureReplacement.TryImportImage(healthTextName, true, out healthTexture))
            {
                Debug.LogError("TravelOptions: Unable to load the base UI image.");
            }
            if (!TextureReplacement.TryImportImage(backTextName, true, out backTexture))
            {
                Debug.LogError("TravelOptions: Unable to load the base UI image.");
            }

            healthTexture = Crop(healthTexture);
            backTexture = Crop(backTexture);

            Debug.Log(backTexture.height);

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
        void Setup()
        {
            int loc = EnemyHealthBarMain.Instance.location;

            switch (loc)
            {
                case 0:
                    
                    //scale for use with NativePanel

                    SetPanel(Health, healthSize);
                    SetPanel(Back, backSize);
                    Health.ProgressTexture = healthTexture;
                    Back.BackgroundTexture = backTexture;
                    break;

                case 1:
                    SetPanel(Health, healthSize);
                    SetPanel(Back, backSize);
                    Health.ProgressTexture = healthTexture;
                    Back.BackgroundTexture = backTexture;
                    break;
            }
        }

        private Texture2D Crop(Texture2D crop)
        {
            Color[] c = crop.GetPixels(67, 0, 378, 40);
            crop = new Texture2D(378, 40);
            crop.SetPixels(c);
            crop.Apply();
            return crop;
        }

        private float SetScale(int scaleSettings)
        {
            switch (scaleSettings)
            {
                case 0:
                    scale = 8;
                    break;
                case 1:
                    scale = 12;
                    break;
                case 2:
                    scale = 16;
                    break;
                case 3:
                    scale = 20;
                    break;
            }

            return scale;
        }

        private BaseScreenComponent SetPanel(BaseScreenComponent panel, Vector2 size)
        {
            scaleVec = new Vector2(0, scale);

            //scaleVec = new Vector2(hud.HUDCompass.Size.x, 0);  For the compass location. Make a switch case later once you've put in the code for the Mod Settings.

            float x = size.x / size.y;
            scaleVec.x = scaleVec.y * x;
            panel.Scale = hud.NativePanel.LocalScale;
            scaleVec *= panel.Scale;
            size = scaleVec;

            //create the health bar
            panel.Size = size;
            panel.BackgroundColor = Color.clear;
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.None;
            panel.Position = new Vector2(hud.ParentPanel.Size.x - panel.Size.x, hud.ParentPanel.Size.y - panel.Size.y - (10 * hud.NativePanel.Scale.y));

            return panel;
;
        }

    }
}