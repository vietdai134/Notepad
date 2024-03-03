using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notepad
{

    public partial class findDialog : Form
    {
        private bool isExpanded = false;
        private bool isDragging = false;
        private Point lastCursor;
        private Point lastForm;
        private Form1 parentForm;
        private RichTextBox richTextBoxInForm1;

        public string GetSearchText()
        {
            return text_Find.Text;
        }
        public string GetReplaceText()
        {
            return text_Replace.Text;
        }
        public findDialog(Form1 form, RichTextBox richTextBox)
        {
            InitializeComponent();
            parentForm = form;
            richTextBoxInForm1 = richTextBox;
        }
        private void findDialog_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursor = Cursor.Position;
                lastForm = this.Location;
            }
        }
        private void findDialog_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentCursor = Cursor.Position;
                this.Location = new Point(lastForm.X + (currentCursor.X - lastCursor.X), lastForm.Y + (currentCursor.Y - lastCursor.Y));
            }
        }
        private void findDialog_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        private void btn_Expand_Click(object sender, EventArgs e)
        {
            if (isExpanded)
            {
                // Đóng rộng form và ẩn TextBox bổ sung
                this.Size = new System.Drawing.Size(520, 50);

            }
            else
            {
                // Mở rộng form và hiển thị TextBox bổ sung
                this.Size = new System.Drawing.Size(520, 100);

            }
            isExpanded = !isExpanded;
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Size = new Size(520, 50);
            this.Close();
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            text_Find.Clear();
            btn_Clear.Enabled = false;
        }

        private void text_Find_TextChanged(object sender, EventArgs e)
        {

            // Kiểm tra nếu TextBox có văn bản, thì hiển thị nút "Clear". Ngược lại, ẩn nút "Clear".
            if (!string.IsNullOrWhiteSpace(text_Find.Text))
            {
                btn_Clear.Enabled = true;
            }
            else
            {
                btn_Clear.Enabled = false;
            }
        }

        private void findDialog_Load(object sender, EventArgs e)
        {
            btn_Clear.Enabled = false;
            btn_Clear_2.Enabled = false;
            this.MouseDown += findDialog_MouseDown;
            this.MouseMove += findDialog_MouseMove;
            this.MouseUp += findDialog_MouseUp;
        }



        private void btn_Clear_2_Click(object sender, EventArgs e)
        {
            text_Replace.Clear();
        }

        private void text_Replace_TextChanged(object sender, EventArgs e)
        {
            // Kiểm tra nếu TextBox có văn bản, thì hiển thị nút "Clear". Ngược lại, ẩn nút "Clear".
            if (!string.IsNullOrWhiteSpace(text_Replace.Text))
            {
                btn_Clear_2.Enabled = true;
            }
            else
            {
                btn_Clear_2.Enabled = false;
            }
        }

        private void find_Text_Click(object sender, EventArgs e)
        {
            string searchText = GetSearchText();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                parentForm.Find(searchText);
            }
            // Truyền searchText cho Form1 để thực hiện tìm kiếm

        }

        private void find_Previous_Click(object sender, EventArgs e)
        {
            string searchText = GetSearchText();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                parentForm.FindPrevious(searchText);
            }
        }

        private void find_Next_Click(object sender, EventArgs e)
        {
            string searchText = GetSearchText();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                parentForm.FindNext(searchText);
            }
        }

        private void btn_Replace_Click(object sender, EventArgs e)
        {
            string searchText = GetSearchText();
            string replaceText = GetReplaceText();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                int startIndex = richTextBoxInForm1.SelectionStart;

                if (startIndex < richTextBoxInForm1.TextLength)
                {
                    int foundIndex = richTextBoxInForm1.Find(searchText, startIndex, RichTextBoxFinds.None);
                    if (foundIndex >= 0)
                    {
                        richTextBoxInForm1.Select(foundIndex, searchText.Length);
                        richTextBoxInForm1.SelectedText = replaceText;

                        // Chuyển vị trí điểm bắt đầu tìm kiếm lên phía sau vị trí mới thay thế
                        startIndex = foundIndex + replaceText.Length;
                        richTextBoxInForm1.Select(startIndex, 0);
                    }
                }
            }
        }


        private void btn_ReplaceAll_Click(object sender, EventArgs e)
        {
            string searchText = GetSearchText();
            string replaceText = GetReplaceText();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                int startIndex = 0;
                int count = 0;

                while (startIndex < richTextBoxInForm1.TextLength)
                {
                    int foundIndex = richTextBoxInForm1.Find(searchText, startIndex, RichTextBoxFinds.None);
                    if (foundIndex >= 0)
                    {
                        richTextBoxInForm1.Select(foundIndex, searchText.Length);
                        richTextBoxInForm1.SelectedText = replaceText;

                        startIndex = foundIndex + replaceText.Length;
                        count++;
                    }
                    else
                    {
                        break; // Thoát vòng lặp nếu không còn kết quả tìm kiếm
                    }
                }

                // Hiển thị số lần thay thế
                MessageBox.Show($"Replaced {count} occurrences.", "Replace All");
            }
        }

    }
}
