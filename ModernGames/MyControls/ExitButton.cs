using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGames.MyControls
{
    public class ExitButton : PictureBox
    {
        public ExitButton() : base() 
        {
            this.Click += ExitButtonClick;
            this.Image = Properties.Image.Exit;
            this.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void ExitButtonClick(object? sender, EventArgs e)
        {
            Control control = (sender as Control)!;
            
            while (control.GetType().BaseType != typeof(Form))
            {
                control = control.Parent;
            }

            (control as Form)!.Close();
        }
    }
}
