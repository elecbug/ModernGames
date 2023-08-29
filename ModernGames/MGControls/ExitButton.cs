using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGames.MGControls
{
    public class ExitButton : PictureBox
    {
        public ExitButton() : base() 
        {
            this.Click += ExitButtonClick;
            this.Image = Properties.Image.Exit;
            this.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        /// <summary>
        /// 클릭 시 해당 버튼이 존재하는 최상위 폼 객체를 닫음
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
