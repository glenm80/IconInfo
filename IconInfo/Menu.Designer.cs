namespace MenuApp
{
    partial class Menu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DropHere = new System.Windows.Forms.TextBox();
            this.LinkTarget = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LinkArgument = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.LinkIconPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.LinkStartIn = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.LinkDescription = new System.Windows.Forms.TextBox();
            this.Image1 = new System.Windows.Forms.PictureBox();
            this.Prev = new System.Windows.Forms.Button();
            this.Next = new System.Windows.Forms.Button();
            this.IconNumber = new System.Windows.Forms.Label();
            this.IconSize = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Image1)).BeginInit();
            this.SuspendLayout();
            // 
            // DropHere
            // 
            this.DropHere.Location = new System.Drawing.Point(12, 46);
            this.DropHere.Multiline = true;
            this.DropHere.Name = "DropHere";
            this.DropHere.Size = new System.Drawing.Size(88, 88);
            this.DropHere.TabIndex = 0;
            this.DropHere.Text = "Drop Link Here";
            this.DropHere.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DropHere.DragDrop += new System.Windows.Forms.DragEventHandler(this.item_DragDrop);
            this.DropHere.DragEnter += new System.Windows.Forms.DragEventHandler(this.item_DragEnter);
            this.DropHere.DragOver += new System.Windows.Forms.DragEventHandler(this.item_DragOver);
            // 
            // LinkTarget
            // 
            this.LinkTarget.Location = new System.Drawing.Point(241, 12);
            this.LinkTarget.Name = "LinkTarget";
            this.LinkTarget.Size = new System.Drawing.Size(434, 20);
            this.LinkTarget.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(121, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Link Target";
            // 
            // LinkArgument
            // 
            this.LinkArgument.Location = new System.Drawing.Point(241, 47);
            this.LinkArgument.Name = "LinkArgument";
            this.LinkArgument.Size = new System.Drawing.Size(434, 20);
            this.LinkArgument.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(121, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Link Arguments";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(121, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Link Icon Path";
            // 
            // LinkIconPath
            // 
            this.LinkIconPath.Location = new System.Drawing.Point(241, 118);
            this.LinkIconPath.Name = "LinkIconPath";
            this.LinkIconPath.Size = new System.Drawing.Size(434, 20);
            this.LinkIconPath.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(121, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Link Start In";
            // 
            // LinkStartIn
            // 
            this.LinkStartIn.Location = new System.Drawing.Point(241, 82);
            this.LinkStartIn.Name = "LinkStartIn";
            this.LinkStartIn.Size = new System.Drawing.Size(434, 20);
            this.LinkStartIn.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(121, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Link Description";
            // 
            // LinkDescription
            // 
            this.LinkDescription.Location = new System.Drawing.Point(241, 154);
            this.LinkDescription.Name = "LinkDescription";
            this.LinkDescription.Size = new System.Drawing.Size(434, 20);
            this.LinkDescription.TabIndex = 9;
            // 
            // Image1
            // 
            this.Image1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Image1.Location = new System.Drawing.Point(241, 199);
            this.Image1.Name = "Image1";
            this.Image1.Size = new System.Drawing.Size(120, 120);
            this.Image1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.Image1.TabIndex = 11;
            this.Image1.TabStop = false;
            // 
            // Prev
            // 
            this.Prev.Location = new System.Drawing.Point(205, 247);
            this.Prev.Name = "Prev";
            this.Prev.Size = new System.Drawing.Size(30, 23);
            this.Prev.TabIndex = 12;
            this.Prev.Text = "<";
            this.Prev.UseVisualStyleBackColor = true;
            this.Prev.Click += new System.EventHandler(this.Prev_Click);
            // 
            // Next
            // 
            this.Next.Location = new System.Drawing.Point(367, 247);
            this.Next.Name = "Next";
            this.Next.Size = new System.Drawing.Size(30, 23);
            this.Next.TabIndex = 13;
            this.Next.Text = ">";
            this.Next.UseVisualStyleBackColor = true;
            this.Next.Click += new System.EventHandler(this.Next_Click);
            // 
            // IconNumber
            // 
            this.IconNumber.AutoSize = true;
            this.IconNumber.Location = new System.Drawing.Point(296, 183);
            this.IconNumber.Name = "IconNumber";
            this.IconNumber.Size = new System.Drawing.Size(13, 13);
            this.IconNumber.TabIndex = 14;
            this.IconNumber.Text = "0";
            this.IconNumber.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // IconSize
            // 
            this.IconSize.Location = new System.Drawing.Point(242, 324);
            this.IconSize.Name = "IconSize";
            this.IconSize.Size = new System.Drawing.Size(118, 21);
            this.IconSize.TabIndex = 15;
            this.IconSize.Text = "0x0";
            this.IconSize.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 351);
            this.Controls.Add(this.IconSize);
            this.Controls.Add(this.IconNumber);
            this.Controls.Add(this.Next);
            this.Controls.Add(this.Prev);
            this.Controls.Add(this.Image1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.LinkDescription);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.LinkStartIn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.LinkIconPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.LinkArgument);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LinkTarget);
            this.Controls.Add(this.DropHere);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Menu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Menu_FormClosing);
            this.Load += new System.EventHandler(this.Menu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Image1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox DropHere;
        private System.Windows.Forms.TextBox LinkTarget;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox LinkArgument;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox LinkIconPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox LinkStartIn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox LinkDescription;
        private System.Windows.Forms.PictureBox Image1;
        private System.Windows.Forms.Button Prev;
        private System.Windows.Forms.Button Next;
        private System.Windows.Forms.Label IconNumber;
        private System.Windows.Forms.Label IconSize;
    }
}