

using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A copy of vertical progress but for horizontal.
    /// </summary>
    public class HorizontalProgress : BaseScreenComponent
    {
        public Texture2D ProgressTexture;
        Texture2D colorTexture;

        Color32 color;
        float amount = 1.0f;

        public float Amount
        {
            get { return amount; }
            set { amount = Mathf.Clamp01(value); }
        }

        public Color32 Color
        {
            get { return color; }
            set { SetColor(value); }
        }

        public HorizontalProgress()
            : base()
        {
        }

        public HorizontalProgress(Texture2D texture)
            : base()
        {
            ProgressTexture = texture;
        }

        public override void Draw()
        {
            // Create texture once
            if (!colorTexture)
                colorTexture = DaggerfallUI.CreateSolidTexture(UnityEngine.Color.white, 8);

            if (Enabled)
            {
                base.Draw();
                DrawProgress();
            }
        }

        public void SetColor(Color32 color)
        {
            this.color = color;
        }

        void DrawProgress()
        {
            double x = 0.5;
            Rect srcRect = new Rect((float)x - (amount / 2), 0, amount, 1);
            Rect dstRect = Rectangle;
            float scaledAmount = Mathf.Round(dstRect.width / 2 * amount);
            dstRect.x += dstRect.width / 2 - scaledAmount;
            dstRect.width = dstRect.width / 2 + scaledAmount - (dstRect.width / 2 - scaledAmount);

            DaggerfallUI.DrawTextureWithTexCoords(dstRect, ProgressTexture, srcRect, true);
        }
    }
}
