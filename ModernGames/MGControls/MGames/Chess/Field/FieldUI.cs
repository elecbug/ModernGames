using ModernGames.MGControls.MGames.Chess.Manager;
using ModernGames.MGControls.MGames.Chess.Support;

namespace ModernGames.MGControls.MGames.Chess.Field
{
    internal class FieldUI : Panel
    {
        private GameManager manager;
        private PictureBox[,] unit_image;
        private Paint color1, color2;
        private FieldData field_data;

        public Color Color1 { get => color1.Color; set => color1.Color = value; }
        public Color Color2 { get => color2.Color; set => color2.Color = value; }

        public FieldUI(GameManager manager, Size size) : base()
        {
            this.manager = manager;
            BackColor = Color.Black;

            Size = size;
            field_data = manager.Data;

            color1 = new Paint(Color.Black);
            color2 = new Paint(Color.Ivory);

            unit_image = new PictureBox[FieldData.MAXIMUM, FieldData.MAXIMUM];

            for (int x = 0; x < FieldData.MAXIMUM; x++)
            {
                for (int y = 0; y < FieldData.MAXIMUM; y++)
                {
                    unit_image[x, y] = new PictureBox()
                    {
                        Parent = this,
                        Visible = true,
                        Size = new Size(Width / FieldData.MAXIMUM, Height / FieldData.MAXIMUM),
                        Location = new Point(x * Width / FieldData.MAXIMUM, y * Height / FieldData.MAXIMUM),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackgroundImageLayout = ImageLayout.Zoom,
                    };

                    unit_image[x, y].Click += ClickCell;
                }
            }

            Resize += ResizeField;

            ResizeField(this, new EventArgs());
            Repainting();
        }

        private void ResizeField(object? sender, EventArgs e)
        {
            for (int x = 0; x < FieldData.MAXIMUM; x++)
            {
                for (int y = 0; y < FieldData.MAXIMUM; y++)
                {
                    unit_image[x, y].Size
                        = new Size(Width / FieldData.MAXIMUM, Height / FieldData.MAXIMUM);
                    unit_image[x, y].Location
                        = new Point(x * Width / FieldData.MAXIMUM, y * Height / FieldData.MAXIMUM);
                }
            }
        }
        private void Repainting()
        {
            for (int x = 0; x < FieldData.MAXIMUM; x++)
            {
                for (int y = 0; y < FieldData.MAXIMUM; y++)
                {
                    if (field_data.Unit(x, y) != null)
                    {
                        unit_image[x, y].BackgroundImage = field_data.Unit(x, y)!.Team == Team.First
                            ? color1.Painting(field_data.Unit(x, y)!.Type)
                            : color2.Painting(field_data.Unit(x, y)!.Type);
                    }
                    else
                    {
                        unit_image[x, y].BackgroundImage = null;
                    }
                    if (field_data.AttackPoints.Contains(new Point(x, y)))
                    {
                        unit_image[x, y].BackColor = Color.Pink;
                    }
                    else if (field_data.MovePoints.Contains(new Point(x, y)))
                    {
                        unit_image[x, y].BackColor = Color.LightGreen;
                    }
                    else
                    {
                        unit_image[x, y].BackColor = (x + y) % 2 == 0 ? Color.LightGray : Color.DarkGray;
                    }
                }
            }
            if (field_data.Choosed != null)
            {
                unit_image[field_data.Choosed.Location.X, field_data.Choosed.Location.Y]!.BackColor
                    = Color.LightBlue;
            }
        }

        public void ClickCell(object? sender, EventArgs e)
        {
            int x = 0, y = 0;

            for (x = 0; x < FieldData.MAXIMUM; x++)
            {
                for (y = 0; y < FieldData.MAXIMUM; y++)
                {
                    if (sender == unit_image[x, y])
                    {
                        goto outside;
                    }
                }
            }

        outside:
            field_data.ControlUnit(x, y);

            Repainting();
        }
    }
}

// 캐슬링(체크 탈출용은 안된다는 듯?), 프로모션, 체크, 체크메이트, 스테일메이트
// 남은 유닛들 만들고 체크 & 스테일 검사 시스템 구축