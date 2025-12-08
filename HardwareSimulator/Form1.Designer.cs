namespace HardwareSimulator
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
			groupBox1 = new GroupBox();
			button1 = new Button();
			comboBox1 = new ComboBox();
			groupBox2 = new GroupBox();
			listBox1 = new ListBox();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(button1);
			groupBox1.Controls.Add(comboBox1);
			groupBox1.Location = new Point(12, 12);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(329, 81);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "RFID Reader";
			// 
			// button1
			// 
			button1.Location = new Point(194, 28);
			button1.Name = "button1";
			button1.Size = new Size(112, 34);
			button1.TabIndex = 1;
			button1.Text = "Scan";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// comboBox1
			// 
			comboBox1.FormattingEnabled = true;
			comboBox1.Location = new Point(6, 30);
			comboBox1.Name = "comboBox1";
			comboBox1.Size = new Size(182, 33);
			comboBox1.TabIndex = 0;
			comboBox1.KeyPress += comboBox1_KeyPress;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(listBox1);
			groupBox2.Location = new Point(12, 99);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new Size(329, 228);
			groupBox2.TabIndex = 1;
			groupBox2.TabStop = false;
			groupBox2.Text = "Drawer Open Events";
			// 
			// listBox1
			// 
			listBox1.FormattingEnabled = true;
			listBox1.Location = new Point(6, 30);
			listBox1.Name = "listBox1";
			listBox1.Size = new Size(300, 179);
			listBox1.TabIndex = 0;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(10F, 25F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(353, 339);
			Controls.Add(groupBox2);
			Controls.Add(groupBox1);
			MaximizeBox = false;
			Name = "Form1";
			Text = "Hardware Simulator";
			TopMost = true;
			groupBox1.ResumeLayout(false);
			groupBox2.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private GroupBox groupBox1;
		private ComboBox comboBox1;
		private Button button1;
		private GroupBox groupBox2;
		private ListBox listBox1;
	}
}
