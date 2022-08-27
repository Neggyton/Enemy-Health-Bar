using System;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Entity;

namespace HealthBarMod
{
    public class HealthBar : Panel
    {


        public DaggerfallEntityBehaviour hitNPC = null;
        HorizontalProgress Health;
        Panel Back;
        Texture2D healthTexture = Texture2D.whiteTexture;
        Texture2D backTexture = Texture2D.whiteTexture;

        Vector2 healthSize = Vector2.zero;
        Vector2 backSize = Vector2.zero;

        bool init = false;
        string healthTextName;
        string backTextName;
        private float timer = 15;
        public bool timerReset = false;
        public const int borderSize = 10;

        
        Vector2 scaleVec;
        float scale;
        public int scaleSettings;
        public int loc;



        public HealthBar()
            :base()
        { 
            
            Back = new Panel();
            Health = new HorizontalProgress();


            LoadTextures();
            PanelSetup();

            SetMargins(Margins.All, borderSize);
            
            Components.Add(Back);
            Components.Add(Health);

        }



        public override void Update()
        {
            
            if (hitNPC)
            {
                float healthPercent = hitNPC.Entity.CurrentHealth / (float) hitNPC.Entity.MaxHealth;
                if (!init)
                {
                    Health.Amount = healthPercent;
                    LoadTextures();
                    PanelSetup();
                    init = true;
                }
                else
                {
                    Health.Amount = healthPercent;
                }
                if (Health.Amount == 0
                    || GameManager.Instance.PlayerDeath.DeathInProgress
                    || !hitNPC.gameObject.activeSelf
                    || timer <= 0
                    || GameManager.Instance.PlayerEntity.IsResting
                    || (!GameManager.Instance.IsPlayingGame() && GameManager.Instance.StateManager.CurrentState != StateManager.StateTypes.UI))
                {
                    hitNPC = null;
                    init = false;
                    OnKill();
                    Components.Remove(this);

                }



                TimeUpdate();
                base.Update();
            }
        }
        public override void Draw()
        {
            base.Draw();
        }
        void LoadTextures()
        {
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
                Debug.LogError("HealthBar: Unable to load the base UI image.");
            }
            if (!TextureReplacement.TryImportImage(backTextName, true, out backTexture))
            {
                Debug.LogError("HealthBar: Unable to load the base UI image.");
            }

            healthTexture = Crop(healthTexture);
            healthSize = new Vector2(healthTexture.width, healthTexture.height);
            backTexture = Crop(backTexture);
            backSize = new Vector2(backTexture.width, backTexture.height);



        }


        void TimeUpdate()
        {
            if (timer > 0)
                timer -= Time.deltaTime;

            if (timerReset)
            {
                timer = 15;
                timerReset = false;
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
            scaleVec = new Vector2(0, scale);

            //scale the size of the nativepanel to the equivalent size of the parentpanel.
            float i = texture.width / texture.height;
            scaleVec.x = scaleVec.y * i;
            panel.Scale = DaggerfallUI.Instance.DaggerfallHUD.NativePanel.LocalScale;
            panel.Size = scaleVec * panel.Scale;

            //create the health bar
            panel.BackgroundColor = Color.clear;
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Bottom;
            panel.Position = new Vector2(hud.ParentPanel.Size.x - panel.Size.x, hud.ParentPanel.Size.y - panel.Size.y - (10 * hud.NativePanel.Scale.y));

            return panel;
;
        }

        public delegate void KillEventHandler();
        public static event KillEventHandler Kill;

        private void OnKill()
        {
            Kill?.Invoke();
        }

    }
}