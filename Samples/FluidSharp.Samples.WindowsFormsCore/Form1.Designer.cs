
namespace FluidSharp.Samples.WindowsFormsCore
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
            this.fluidWidgetView1 = new FluidSharp.Views.WindowsForms.FluidWidgetView();
            this.SuspendLayout();
            // 
            // fluidWidgetView1
            // 
            this.fluidWidgetView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fluidWidgetView1.AutoScale = true;
            this.fluidWidgetView1.AutoSizeHeight = false;
            this.fluidWidgetView1.BackColor = System.Drawing.Color.Black;
            this.fluidWidgetView1.Location = new System.Drawing.Point(15, 16);
            this.fluidWidgetView1.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.fluidWidgetView1.Name = "fluidWidgetView1";
            this.fluidWidgetView1.Size = new System.Drawing.Size(1247, 995);
            this.fluidWidgetView1.TabIndex = 0;
            this.fluidWidgetView1.VSync = true;
            this.fluidWidgetView1.WidgetSource = null;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1277, 1027);
            this.Controls.Add(this.fluidWidgetView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Views.WindowsForms.FluidWidgetView fluidWidgetView1;
    }
}

