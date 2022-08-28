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
        public int loc;
        public int scaleOffset = 0;

        public Vector2 offset = new Vector2(0, 10);



        public HealthBar()
            : base()
        {

            Back = new Panel();
            Health = new HorizontalProgress();


            LoadTextures();
            PanelSetup();


        }



        public override void Update()
        {

            if (hitNPC)
            {
                float healthPercent = hitNPC.Entity.CurrentHealth / (float)hitNPC.Entity.MaxHealth;
                Health.Amount = healthPercent;
                if (!init)
                {
                    LoadTextures();
                    PanelSetup();
                    init = true;
                    Components.Add(Back);
                    Components.Add(Health);
                }
          



                TimeUpdate();
                base.Update();
            }

            if (Health.Amount == 0
              || GameManager.Instance.PlayerDeath.DeathInProgress
              || !hitNPC
              || timer <= 0
              || GameManager.Instance.PlayerEntity.IsResting
              || (!GameManager.Instance.IsPlayingGame() && GameManager.Instance.StateManager.CurrentState != StateManager.StateTypes.UI))
            {
                hitNPC = null;
                init = false;
                Components.Remove(Back);
                Components.Remove(Health);

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
            backTexture = Crop(backTexture);
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
            switch (loc)
            {
                case 0:

                    //scale for use with NativePanel

                    SetPanel(Health, healthTexture);
                    SetPanel(Back, backTexture);
                    Health.ProgressTexture = healthTexture;
                    Back.BackgroundTexture = backTexture;
                    break;

                case 1:
                    SetPanel(Health, healthTexture);
                    SetPanel(Back, backTexture);
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