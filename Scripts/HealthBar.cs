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
    public class HealthBar : Panel
    {


        MobilePersonNPC villagerNpc = null;
        public DaggerfallEntityBehaviour hitNPC = null;
        DaggerfallEntityBehaviour origNPC = null;
        HorizontalProgress Health;
        Panel Back;
        Texture2D healthTexture = Texture2D.whiteTexture;
        Texture2D backTexture = Texture2D.whiteTexture;
        WorldContext storedContext;

        Vector2 healthSize = Vector2.zero;
        Vector2 backSize = Vector2.zero;


        string healthTextName;
        string backTextName;
        private float timer = 15;
        public bool reset = false;
        public bool isVillager { get; private set; }

        
        Vector2 scaleVec;
        float scale;
        public int scaleSettings;
        public int loc;



        public HealthBar()
            :base()
        {
            
            Back = new Panel();
            Health = new HorizontalProgress();
            

            if (villagerNpc)
            {
                if (hitNPC.EntityType == EntityTypes.CivilianNPC && !villagerNpc.IsGuard)
                {
                    isVillager = true;
                }
            }
            else
                isVillager = false;
         

        }


        public void Start()
        {
            LoadTextures();
            PanelSetup();


            DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.Components.Add(Back);
            DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.Components.Add(Health);

        }

        public override void Update()
        {
            if (!origNPC || (hitNPC && origNPC.GetInstanceID() != hitNPC.GetInstanceID()))
            {
                LoadTextures();
                PanelSetup();
                origNPC = hitNPC;

                storedContext = GameManager.Instance.PlayerEnterExit.WorldContext;

            }
            Debug.Log(Health.Size);
            Debug.Log(Health.Position);
            if (hitNPC)
            {

                Health.Amount = hitNPC.Entity.CurrentHealthPercent;
            }
            if (isVillager
                || !hitNPC
                || GameManager.Instance.PlayerDeath.DeathInProgress
                || !hitNPC.gameObject.activeSelf
                || timer <= 0
                || GameManager.Instance.PlayerEntity.IsResting
                || storedContext != GameManager.Instance.PlayerEnterExit.WorldContext
                || (!GameManager.Instance.IsPlayingGame() && GameManager.Instance.StateManager.CurrentState != StateManager.StateTypes.UI))
            {
            }



            TimeUpdate();
            base.Update();
        }
        public override void Draw()
        {
                base.Draw();
        }
        void LoadTextures()
        {
            //switch (hitNPC.Entity.Team)
            //{
            //    case MobileTeams.Undead:
            //    case MobileTeams:
            //        healthTextName = "BloodHP(dags).png";
            //        backTextName = "BloodHP(dags)_BD.png";
            //        break;
            //}
            if (!hitNPC || hitNPC.Entity.Team != MobileTeams.Undead)
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
            healthSize = new Vector2(healthTexture.width, healthTexture.height);
            backTexture = Crop(backTexture);
            backSize = new Vector2(backTexture.width, backTexture.height);



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
        void PanelSetup()
        {
            switch (loc)
            {
                case 0:
                    
                    //scale for use with NativePanel

                    SetPanel(Health, healthSize, healthTexture);
                    SetPanel(Back, backSize, backTexture);
                    Health.ProgressTexture = healthTexture;
                    Back.BackgroundTexture = backTexture;
                    break;

                case 1:
                    SetPanel(Health, healthSize, healthTexture);
                    SetPanel(Back, backSize, backTexture);
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
            scaleSettings = 1;
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

        private BaseScreenComponent SetPanel(BaseScreenComponent panel, Vector2 size, Texture2D texture)
        {

            DaggerfallHUD hud = DaggerfallUI.Instance.DaggerfallHUD;
            scale = SetScale(scaleSettings);
            size.x = texture.width;
            size.y = texture.height;
            scaleVec = new Vector2(0, scale);

            //scaleVec = new Vector2(hud.HUDCompass.Size.x, 0);  For the compass location. Make a switch case later once you've put in the code for the Mod Settings.

            float x = size.x / size.y;
            scaleVec.x = scaleVec.y * x;
            panel.Scale = DaggerfallUI.Instance.DaggerfallHUD.NativePanel.LocalScale;
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