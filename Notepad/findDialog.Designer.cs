namespace Notepad
{
    partial class findDialog
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
            btn_ReplaceAll = new Button();
            btn_Expand = new Button();
            btn_Exit = new Button();
            text_Find = new TextBox();
            find_Previous = new Button();
            find_Next = new Button();
            find_Text = new Button();
            btn_Clear = new Button();
            text_Replace = new TextBox();
            btn_Replace = new Button();
            btn_Clear_2 = new Button();
            SuspendLayout();
            // 
            // btn_ReplaceAll
            // 
            btn_ReplaceAll.Location = new Point(413, 58);
            btn_ReplaceAll.Name = "btn_ReplaceAll";
            btn_ReplaceAll.Size = new Size(94, 29);
            btn_ReplaceAll.TabIndex = 1;
            btn_ReplaceAll.Text = "Replace all";
            btn_ReplaceAll.UseVisualStyleBackColor = true;
            btn_ReplaceAll.Click += btn_ReplaceAll_Click;
            // 
            // btn_Expand
            // 
            btn_Expand.Location = new Point(12, 12);
            btn_Expand.Name = "btn_Expand";
            btn_Expand.Size = new Size(37, 26);
            btn_Expand.TabIndex = 0;
            btn_Expand.Text = "V";
            btn_Expand.UseVisualStyleBackColor = true;
            btn_Expand.Click += btn_Expand_Click;
            // 
            // btn_Exit
            // 
            btn_Exit.Location = new Point(475, 11);
            btn_Exit.Name = "btn_Exit";
            btn_Exit.Size = new Size(32, 27);
            btn_Exit.TabIndex = 2;
            btn_Exit.Text = "X";
            btn_Exit.UseVisualStyleBackColor = true;
            btn_Exit.Click += btn_Exit_Click;
            // 
            // text_Find
            // 
            text_Find.Location = new Point(55, 11);
            text_Find.Name = "text_Find";
            text_Find.Size = new Size(275, 27);
            text_Find.TabIndex = 3;
            text_Find.TextChanged += text_Find_TextChanged;
            // 
            // find_Previous
            // 
            find_Previous.Location = new Point(336, 11);
            find_Previous.Name = "find_Previous";
            find_Previous.Size = new Size(38, 27);
            find_Previous.TabIndex = 4;
            find_Previous.Text = "up";
            find_Previous.UseVisualStyleBackColor = true;
            find_Previous.Click += find_Previous_Click;
            // 
            // find_Next
            // 
            find_Next.Location = new Point(380, 11);
            find_Next.Name = "find_Next";
            find_Next.Size = new Size(70, 27);
            find_Next.TabIndex = 5;
            find_Next.Text = "down";
            find_Next.UseVisualStyleBackColor = true;
            find_Next.Click += find_Next_Click;
            // 
            // find_Text
            // 
            find_Text.Location = new Point(282, 12);
            find_Text.Name = "find_Text";
            find_Text.Size = new Size(48, 26);
            find_Text.TabIndex = 7;
            find_Text.Text = "find";
            find_Text.UseVisualStyleBackColor = true;
            find_Text.Click += find_Text_Click;
            // 
            // btn_Clear
            // 
            btn_Clear.FlatAppearance.BorderSize = 0;
            btn_Clear.Location = new Point(268, 12);
            btn_Clear.Name = "btn_Clear";
            btn_Clear.Size = new Size(24, 26);
            btn_Clear.TabIndex = 8;
            btn_Clear.Text = "x";
            btn_Clear.UseVisualStyleBackColor = true;
            btn_Clear.Click += btn_Clear_Click;
            // 
            // text_Replace
            // 
            text_Replace.Location = new Point(55, 60);
            text_Replace.Name = "text_Replace";
            text_Replace.Size = new Size(242, 27);
            text_Replace.TabIndex = 9;
            text_Replace.TextChanged += text_Replace_TextChanged;
            // 
            // btn_Replace
            // 
            btn_Replace.Location = new Point(303, 58);
            btn_Replace.Name = "btn_Replace";
            btn_Replace.Size = new Size(104, 29);
            btn_Replace.TabIndex = 10;
            btn_Replace.Text = "Replace";
            btn_Replace.UseVisualStyleBackColor = true;
            btn_Replace.Click += btn_Replace_Click;
            // 
            // btn_Clear_2
            // 
            btn_Clear_2.Location = new Point(282, 60);
            btn_Clear_2.Name = "btn_Clear_2";
            btn_Clear_2.Size = new Size(15, 27);
            btn_Clear_2.TabIndex = 11;
            btn_Clear_2.Text = "x";
            btn_Clear_2.UseVisualStyleBackColor = true;
            btn_Clear_2.Click += btn_Clear_2_Click;
            // 
            // findDialog
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 50);
            Controls.Add(btn_Clear_2);
            Controls.Add(btn_Replace);
            Controls.Add(text_Replace);
            Controls.Add(btn_Clear);
            Controls.Add(find_Text);
            Controls.Add(find_Next);
            Controls.Add(find_Previous);
            Controls.Add(text_Find);
            Controls.Add(btn_Exit);
            Controls.Add(btn_ReplaceAll);
            Controls.Add(btn_Expand);
            FormBorderStyle = FormBorderStyle.None;
            Name = "findDialog";
            Text = "findDialog";
            Load += findDialog_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btn_ReplaceAll;
        private Button btn_Expand;
        private Button btn_Exit;
        private TextBox text_Find;
        private Button find_Previous;
        private Button find_Next;
        private Button find_Text;
        private Button btn_Clear;
        private TextBox text_Replace;
        private Button btn_Replace;
        private Button btn_Clear_2;
    }
}