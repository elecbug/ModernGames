namespace ModernGames.MGControls.MGames.Chess.Support
{
    internal class Paint
    {
        private Color color;
        public Color Color { get => color; set { SetPaint(); color = value; } }

        private Bitmap[] units_image;

        public Paint(Color color)
        {
            this.color = color;
            units_image = new Bitmap[Enum.GetValues<Type>().Length];

            units_image[(int)Type.Pawn] = Properties.Unit.Pawn;
            units_image[(int)Type.Rook] = Properties.Unit.Rook;
            units_image[(int)Type.Knight] = Properties.Unit.Knight;
            units_image[(int)Type.Bishop] = Properties.Unit.Bishop;
            units_image[(int)Type.King] = Properties.Unit.King;
            units_image[(int)Type.Queen] = Properties.Unit.Queen;

            SetPaint();
        }

        public Bitmap Painting(Type unit) => units_image[(int)unit];

        private void SetPaint()
        {
            foreach (Bitmap unit in units_image)
            {
                for (int x = 0; x < unit.Width; x++)
                {
                    for (int y = 0; y < unit.Height; y++)
                    {
                        if (unit.GetPixel(x, y).A != 0)
                        {
                            unit.SetPixel(x, y, color);
                        }
                    }
                }
            }
        }
    }
}
