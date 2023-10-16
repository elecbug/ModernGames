namespace ModernGames.MGControls.MGames.Chess.Support
{
    internal class Promotion
    {
        public static Type DialogBox(string title, string promptText,
            string button1 = "Queen", string button2 = "Knight", string button3 = "Rook", string button4 = "Bishop")
        {
            Form form = new Form();
            Label label = new Label();
            Button button_1 = new Button();
            Button button_2 = new Button();
            Button button_3 = new Button();
            Button button_4 = new Button();

            int buttonStartPos = 5; //Standard two button position

            form.Text = title;

            // Label
            label.Text = promptText;
            label.SetBounds(5, 20, 400, 13);
            button_1.Text = button1;
            button_2.Text = button2;
            button_3.Text = button3;
            button_4.Text = button4;
            button_1.DialogResult = DialogResult.OK;
            button_2.DialogResult = DialogResult.Cancel;
            button_3.DialogResult = DialogResult.Yes;
            button_4.DialogResult = DialogResult.Abort;

            button_1.SetBounds(buttonStartPos, 72, 75, 25);
            button_2.SetBounds(buttonStartPos + 80, 72, 75, 25);
            button_3.SetBounds(buttonStartPos + 2 * 80, 72, 75, 25);
            button_4.SetBounds(buttonStartPos + 3 * 80, 72, 75, 25);

            label.AutoSize = true;
            button_1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_4.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(325, 107);
            form.Controls.AddRange(new Control[] { label, button_1, button_2, button_3, button_4 });

            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.ControlBox = false;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            DialogResult dialogResult = form.ShowDialog();
            switch (dialogResult)
            {
                case DialogResult.OK: return Type.Queen;
                case DialogResult.Cancel: return Type.Knight;
                case DialogResult.Yes: return Type.Rook;
                case DialogResult.Abort: return Type.Bishop;
                default: return Type.Pawn;
            }
        }
    }
}
