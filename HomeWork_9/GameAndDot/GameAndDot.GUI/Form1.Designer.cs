namespace GameAndDot.GUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            textBox1 = new TextBox();
            button1 = new Button();
            label2 = new Label();
            usernameLbl = new Label();
            label4 = new Label();
            colorLbl = new Label();
            listBox1 = new ListBox();
            gameField = new Panel();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(144, 185);
            label1.Name = "label1";
            label1.Size = new Size(144, 21);
            label1.TabIndex = 0;
            label1.Text = "Введите username:";
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 12F);
            textBox1.Location = new Point(294, 182);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(157, 29);
            textBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 12F);
            button1.Location = new Point(466, 182);
            button1.Name = "button1";
            button1.Size = new Size(98, 32);
            button1.TabIndex = 2;
            button1.Text = "Войти";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(624, 14);
            label2.Name = "label2";
            label2.Size = new Size(84, 21);
            label2.TabIndex = 3;
            label2.Text = "Username:";
            label2.Visible = false;
            // 
            // usernameLbl
            // 
            usernameLbl.AutoSize = true;
            usernameLbl.Font = new Font("Segoe UI", 12F);
            usernameLbl.Location = new Point(714, 14);
            usernameLbl.Name = "usernameLbl";
            usernameLbl.Size = new Size(52, 21);
            usernameLbl.TabIndex = 4;
            usernameLbl.Text = "label3";
            usernameLbl.Visible = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(624, 44);
            label4.Name = "label4";
            label4.Size = new Size(51, 21);
            label4.TabIndex = 5;
            label4.Text = "Color:";
            label4.Visible = false;
            // 
            // colorLbl
            // 
            colorLbl.AutoSize = true;
            colorLbl.Font = new Font("Segoe UI", 12F);
            colorLbl.Location = new Point(681, 44);
            colorLbl.Name = "colorLbl";
            colorLbl.Size = new Size(91, 21);
            colorLbl.TabIndex = 6;
            colorLbl.Text = "#shdfksjdhf";
            colorLbl.Visible = false;
            // 
            // listBox1
            // 
            listBox1.Font = new Font("Segoe UI", 12F);
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(620, 89);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(211, 340);
            listBox1.TabIndex = 7;
            listBox1.Visible = false;
            // 
            // gameField
            // 
            gameField.BackColor = Color.White;
            gameField.Location = new Point(4, 6);
            gameField.Name = "gameField";
            gameField.Size = new Size(601, 423);
            gameField.TabIndex = 8;
            gameField.Visible = false;
            gameField.MouseClick += gameField_MouseClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(843, 450);
            Controls.Add(gameField);
            Controls.Add(listBox1);
            Controls.Add(colorLbl);
            Controls.Add(label4);
            Controls.Add(usernameLbl);
            Controls.Add(label2);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private Button button1;
        private Label label2;
        private Label usernameLbl;
        private Label label4;
        private Label colorLbl;
        private ListBox listBox1;
        private Panel gameField;
    }
}
