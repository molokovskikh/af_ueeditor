using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace UEEditor
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmProgress : System.Windows.Forms.Form, IProgressNotifier
	{
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblWait;
		public System.Windows.Forms.ProgressBar Progress;
		private System.ComponentModel.IContainer components;
		private System.Timers.Timer timerProgress;

		public bool Stop = false;
		private System.Windows.Forms.ImageList imgList;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;

		public frmProgress()
		{
			// Required for Windows Form Designer support
			InitializeComponent();

			// TODO: Add any constructor code after InitializeComponent call
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProgress));
			this.btnCancel = new System.Windows.Forms.Button();
			this.imgList = new System.Windows.Forms.ImageList(this.components);
			this.Progress = new System.Windows.Forms.ProgressBar();
			this.lblWait = new System.Windows.Forms.Label();
			this.timerProgress = new System.Timers.Timer();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.timerProgress)).BeginInit();
			this.SuspendLayout();
			//
			// btnCancel
			//
			this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnCancel.ImageIndex = 0;
			this.btnCancel.ImageList = this.imgList;
			this.btnCancel.Location = new System.Drawing.Point(141, 208);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(144, 32);
			this.btnCancel.TabIndex = 0;
			this.btnCancel.Text = "Отмена";
			this.btnCancel.UseVisualStyleBackColor = false;
			//
			// imgList
			//
			this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
			this.imgList.TransparentColor = System.Drawing.Color.Transparent;
			this.imgList.Images.SetKeyName(0, "");
			//
			// Progress
			//
			this.Progress.Location = new System.Drawing.Point(61, 168);
			this.Progress.Name = "Progress";
			this.Progress.Size = new System.Drawing.Size(304, 23);
			this.Progress.TabIndex = 1;
			//
			// lblWait
			//
			this.lblWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblWait.Location = new System.Drawing.Point(69, 24);
			this.lblWait.Name = "lblWait";
			this.lblWait.Size = new System.Drawing.Size(288, 23);
			this.lblWait.TabIndex = 2;
			this.lblWait.Text = "Подождите, идёт применение изменений...";
			//
			// timerProgress
			//
			this.timerProgress.Enabled = true;
			this.timerProgress.SynchronizingObject = this;
			this.timerProgress.Elapsed += new System.Timers.ElapsedEventHandler(this.timerProgress_Elapsed);
			//
			// label1
			//
			this.label1.Location = new System.Drawing.Point(56, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(312, 23);
			this.label1.TabIndex = 3;
			//
			// textBox1
			//
			this.textBox1.Location = new System.Drawing.Point(48, 88);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(336, 64);
			this.textBox1.TabIndex = 4;
			//
			// frmProgress
			//
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Control;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(426, 258);
			this.ControlBox = false;
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblWait);
			this.Controls.Add(this.Progress);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmProgress";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			((System.ComponentModel.ISupportInitialize)(this.timerProgress)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private void timerProgress_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			Progress.Value = ApplyProgress;
			label1.Text = Status;
			textBox1.Text = Error;
			if (Stop)
				this.DialogResult = DialogResult.OK;
		}

		public string Status { get; set; }

		public string Error { get; set; }

		public int ApplyProgress { get; set; }
	}
}