using System;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallConnect;

namespace HealthBarMod
{
    public class HealthBar : Panel
    {


        public DaggerfallEntityBehaviour hitNPC = null;
        HorizontalProgress Health;
        Panel Back;
        Texture2D healthTexture = Texture2D.whiteTexture;
        Texture2D backTexture = Texture2D.whiteTexture;

        bool init = false;
        string healthTextName;
        string backTextName;
        private float timer = 15;
        public bool timerReset = false;


        Vector2 scaleVec;
        float scale;
        public int scaleSettings;
        public int scaleOffset = 0;

        public Vector2 offset = new Vector2(0,2);



        public HealthBar(Vector2 offsetLoad, int scaleOffsetLoad)
            : base()
        {
            Back = new Panel();
            Health = new HorizontalProgress();


            LoadTextures();
            PanelSetup();

            offset = offsetLoad;
            scaleOffset = scaleOffsetLoad;


            Components.Add(Back);
            Components.Add(Health);
        }



        public override void Update()
        {
            if (!hitNPC)
                return;

            float healthPercent = hitNPC.Entity.CurrentHealth / (float)hitNPC.Entity.MaxHealth;
            Health.Amount = healthPercent;
            if (!init)
            {
                Size = DaggerfallUI.Instance.DaggerfallHUD.ParentPanel.Size;
                LoadTextures();
                PanelSetup();
                init = true;
            }
            TimeUpdate();
            base.Update();

            if (Health.Amount == 0
              || GameManager.Instance.PlayerDeath.DeathInProgress
              || !hitNPC
              || timer <= 0
              || GameManager.Instance.PlayerEntity.IsResting
              || (!GameManager.Instance.IsPlayingGame() && GameManager.Instance.StateManager.CurrentState != StateManager.StateTypes.UI))
            {
                hitNPC = null;
                init = false;
            }
        }
        public override void Draw()
        {
            if (!hitNPC)
                return;

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
        public void PanelSetup()
        {
            SetPanel(Health, healthTexture);
            SetPanel(Back, backTexture);
            Health.ProgressTexture = healthTexture;
            Back.BackgroundTexture = backTexture;
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

        private BaseScreenComponent SetPanel(BaseScreenComponent panel,Texture2D texture)
        {
            

            DaggerfallHUD hud = DaggerfallUI.Instance.DaggerfallHUD;
            scale = SetScale(scaleSettings) + scaleOffset;
            scaleVec = new Vector2(0, scale);

            //scale the size of the nativepanel to the equivalent size of the parentpanel.
            float textureRatio = texture.width / texture.height;
            scaleVec.x = scaleVec.y * textureRatio;
            panel.Scale = DaggerfallUI.Instance.DaggerfallHUD.NativePanel.LocalScale;
            panel.Size = scaleVec * panel.Scale;

            Vector2 offsetResize = offset * DaggerfallUI.Instance.DaggerfallHUD.NativePanel.LocalScale;

            //create the health bar
            panel.BackgroundColor = Color.clear;
            panel.HorizontalAlignment = HorizontalAlignment.None;
            panel.VerticalAlignment = VerticalAlignment.None;
            panel.Position = new Vector2((hud.ParentPanel.Size.x/2) - (panel.Size.x/2) + offsetResize.x, hud.ParentPanel.Size.y - panel.Size.y - offsetResize.y);

            return panel;
        }

    }
}