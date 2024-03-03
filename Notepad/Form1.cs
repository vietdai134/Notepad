using System.DirectoryServices;
using System.Drawing.Printing;
using System.Reflection;
using System.Windows.Forms;
namespace Notepad
{
    public partial class Form1 : Form
    {

        private Dictionary<TabPage, string> tabFilePaths = new Dictionary<TabPage, string>();
        private bool isStatusBarVisible = true;
        private int currentSearchIndex = -1;
        private List<int> searchResults = new List<int>();
        private findDialog findDialog;
        private List<int> highlightedIndices = new List<int>();
        public Form1()
        {
            InitializeComponent();
            findDialog = new findDialog(this, richTextBox1);
            Image pngImage = Image.FromFile("E:\\notepad.png");
            findDialog.Size = new Size(520, 50);
            // Chuyển đổi hình ảnh PNG thành Icon
            Icon iconFromPng = Icon.FromHandle(((Bitmap)pngImage).GetHicon());

            // Gán Icon cho Form
            this.Icon = iconFromPng;
        }

        private TabPage CloneTabPage(TabPage sourceTabPage)
        {
            TabPage newTabPage = new TabPage(sourceTabPage.Text);


            // Sao chép MenuStrip
            MenuStrip currentMenuStrip = sourceTabPage.Controls.OfType<MenuStrip>().FirstOrDefault();
            if (currentMenuStrip != null)
            {
                MenuStrip newMenuStrip = CloneMenuStrip(currentMenuStrip);
                newTabPage.Controls.Add(newMenuStrip);
            }


            // Sao chép RichTextBox
            RichTextBox currentRichTextBox = sourceTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();
            if (currentRichTextBox != null)
            {
                RichTextBox newRichTextBox = new RichTextBox();
                //newRichTextBox.Rtf = currentRichTextBox.Rtf;
                newRichTextBox.Size = currentRichTextBox.Size;

                newRichTextBox.Location = new Point(0, currentMenuStrip != null ? currentMenuStrip.Bottom : 0);
                newRichTextBox.BorderStyle = currentRichTextBox.BorderStyle;

                newTabPage.Controls.Add(newRichTextBox);
            }

            return newTabPage;

        }

        private MenuStrip CloneMenuStrip(MenuStrip sourceMenuStrip)
        {
            MenuStrip newMenuStrip = new MenuStrip();

            List<ToolStripMenuItem> menuItems = sourceMenuStrip.Items.OfType<ToolStripMenuItem>().ToList();
            foreach (ToolStripMenuItem sourceMenuItem in menuItems)
            {
                ToolStripMenuItem newMenuItem = CloneToolStripMenuItem(sourceMenuItem);
                newMenuStrip.Items.Add(newMenuItem);
            }

            return newMenuStrip;

        }


        private ToolStripMenuItem CloneToolStripMenuItem(ToolStripMenuItem sourceMenuItem)
        {
            ToolStripMenuItem newMenuItem = new ToolStripMenuItem();

            // Sử dụng reflection để sao chép thuộc tính của ToolStripMenuItem
            foreach (PropertyInfo property in typeof(ToolStripMenuItem).GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(newMenuItem, property.GetValue(sourceMenuItem));
                }
            }

            // Sao chép các ToolStripDropDownItems
            List<ToolStripItem> dropDownItems = sourceMenuItem.DropDownItems.OfType<ToolStripItem>().ToList();
            foreach (ToolStripItem sourceItem in dropDownItems)
            {
                if (sourceItem is ToolStripMenuItem)
                {
                    ToolStripMenuItem newSubMenuItem = CloneToolStripMenuItem((ToolStripMenuItem)sourceItem);
                    newMenuItem.DropDownItems.Add(newSubMenuItem);
                }
            }

            return newMenuItem;

        }




        private void AddCloseButtonToTab(TabPage tabPage)
        {
            // Tạo hình ảnh chứa chữ "x"
            Image closeImage = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(closeImage))
            {
                g.DrawString("x", new Font("Arial", 10), Brushes.Red, new PointF(0, 0));
            }

            // Tạo một PictureBox để hiển thị hình ảnh
            PictureBox closeButton = new PictureBox();
            closeButton.Image = closeImage;
            closeButton.Size = new Size(16, 16);

            // Điều chỉnh vị trí để đặt gần hơn với tiêu đề tab
            closeButton.Location = new Point(tabPage.Right - closeButton.Width - 3, 4);

            // Gán sự kiện khi người dùng nhấp vào hình ảnh
            closeButton.Click += (sender, e) =>
            {
                tabControl1.TabPages.Remove(tabPage);
            };

            // Đảm bảo rằng hình ảnh hiển thị trên các phần khác của TabPage
            tabPage.Controls.Add(closeButton);
            tabPage.Controls.SetChildIndex(closeButton, 0);

        }



        private void newTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            TabPage newTabPage = CloneTabPage(currentTabPage);
            AddCloseButtonToTab(newTabPage);
            newTabPage.Text = "Untitled";
            tabControl1.TabPages.Add(newTabPage); // Sử dụng Add thay vì Controls.Add
            RichTextBox newRichTextBox = newTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();
            if (newRichTextBox != null)
            {
                newRichTextBox.TextChanged += (s, ev) =>
                {
                    // 3. Trong sự kiện TextChanged, cập nhật tiêu đề của TabPage
                    string text = newRichTextBox.Text;
                    int titleLength = Math.Min(text.Length, 20); // Lấy 20 ký tự đầu tiên
                    string title = text.Substring(0, titleLength);

                    newTabPage.Text = title;
                    int position = newRichTextBox.SelectionStart;
                    int line = newRichTextBox.GetLineFromCharIndex(position) + 1;
                    int column = position - newRichTextBox.GetFirstCharIndexFromLine(line - 1) + 1;

                    // Cập nhật giá trị trên StatusBar
                    lineColumnStatustoolStripStatusLabel1.Text = $"Dòng: {line}, Cột: {column}";

                };
            }
        }



        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Thêm nút đóng vào tab ban đầu
            AddCloseButtonToTab(tabPage1);
            richTextBox1.SelectionChanged += richTextBox1_TextChanged;

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Lưu đường dẫn tệp đã chọn vào biến filePath
                    string filePath = openFileDialog.FileName;

                    // Đọc nội dung từ tệp đã chọn
                    string fileContents = File.ReadAllText(filePath);

                    // Tạo tab mới và sao chép các thành phần từ tab gốc
                    TabPage newTabPage = CloneTabPage(tabControl1.SelectedTab);



                    // Tìm RichTextBox trong tab mới và đặt nội dung từ file
                    RichTextBox newRichTextBox = newTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();
                    if (newRichTextBox != null)
                    {

                        newRichTextBox.Text = fileContents;
                    }
                    newTabPage.Text = Path.GetFileName(filePath);
                    // Thêm nút đóng vào tab mới
                    AddCloseButtonToTab(newTabPage);

                    // Thêm tab mới vào TabControl
                    tabControl1.TabPages.Add(newTabPage);
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có
                    MessageBox.Show("Lỗi khi mở tệp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveTabContent(TabPage tabPage)
        {
            // Lấy đường dẫn tệp cho tab hiện tại từ Dictionary
            if (tabFilePaths.TryGetValue(tabPage, out string filePath))
            {
                RichTextBox richTextBox = tabPage.Controls.OfType<RichTextBox>().FirstOrDefault();
                if (richTextBox != null)
                {
                    string content = richTextBox.Text;
                    File.WriteAllText(filePath, content);
                    tabPage.Text = Path.GetFileName(filePath);
                }
            }
            else
            {
                // Nếu tệp chưa được lưu trước đó, thực hiện hành động "Lưu tệp" (Save As)
                SaveAsTabContent(tabPage);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            SaveTabContent(currentTabPage);
        }

        private void SaveAsTabContent(TabPage tabPage)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            TabPage newTabPage = CloneTabPage(currentTabPage);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.FileName = newTabPage.Text;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                tabFilePaths[tabPage] = filePath; // Lưu đường dẫn tệp vào Dictionary
                SaveTabContent(tabPage);
                tabPage.Text = Path.GetFileName(filePath);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            SaveAsTabContent(currentTabPage);
        }



        private void SaveAllTabs()
        {
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                SaveTabContent(tabPage);
            }
        }

        private void saveAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveAllTabs();
        }

        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tạo một PageSetupDialog để cho phép người dùng tùy chỉnh cài đặt trang in
            PageSetupDialog pageSetupDialog = new PageSetupDialog();

            // Thiết lập cài đặt trang mặc định (nếu cần)
            pageSetupDialog.PageSettings = new PageSettings();

            // Hiển thị hộp thoại cài đặt trang
            if (pageSetupDialog.ShowDialog() == DialogResult.OK)
            {
                // Lưu cài đặt trang đã được chọn (nếu cần)
                PageSettings selectedPageSettings = pageSetupDialog.PageSettings;

                // Bạn có thể sử dụng selectedPageSettings để in trang theo cài đặt này
                // Ví dụ:
                PrintDocument printDocument = new PrintDocument();
                printDocument.DefaultPageSettings = selectedPageSettings;

                // Tiếp tục xử lý in ở đây
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tạo một PrintDialog để cho phép người dùng chọn máy in
            PrintDialog printDialog = new PrintDialog();

            // Tạo một PrintDocument để quản lý việc in
            PrintDocument printDocument = new PrintDocument();

            // Thiết lập sự kiện PrintPage để in nội dung
            printDocument.PrintPage += (s, ev) =>
            {
                // Lấy trang hiện tại trong tabControl
                TabPage currentTabPage = tabControl1.SelectedTab;
                RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();
                if (richTextBox != null)
                {
                    string textToPrint = richTextBox.Text;

                    // In nội dung lên trang
                    ev.Graphics.DrawString(textToPrint, new Font("Arial", 12), Brushes.Black, 20, 20);

                    // Kiểm tra xem còn trang để in không
                    if (textToPrint.Length > 0)
                    {
                        ev.HasMorePages = true;
                    }
                    else
                    {
                        ev.HasMorePages = false;
                    }
                }
            };

            // Khi người dùng chọn máy in và bấm OK
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;

            if (currentTabPage != null)
            {
                // Kiểm tra xem tab có nội dung chưa được lưu
                RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();
                if (richTextBox != null && richTextBox.Modified)
                {
                    // Nếu nội dung tab có thay đổi chưa được lưu, hỏi người dùng có muốn lưu trước khi đóng không
                    DialogResult result = MessageBox.Show("Nội dung trang hiện tại đã thay đổi. Bạn có muốn lưu trước khi đóng không?", "Lưu trước khi đóng", MessageBoxButtons.YesNoCancel);

                    if (result == DialogResult.Yes)
                    {
                        // Lưu nội dung tab trước khi đóng
                        SaveTabContent(currentTabPage);
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        // Hủy việc đóng tab nếu người dùng chọn Cancel
                        return;
                    }
                }

                // Đóng tab hiện tại
                tabControl1.TabPages.Remove(currentTabPage);
            }
        }

        private void closeWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có nội dung tab nào chưa được lưu trong cửa sổ hiện tại
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                RichTextBox richTextBox = tabPage.Controls.OfType<RichTextBox>().FirstOrDefault();
                if (richTextBox != null && richTextBox.Modified)
                {
                    // Nếu có nội dung tab nào đã thay đổi chưa được lưu, hỏi người dùng có muốn lưu trước khi đóng không
                    DialogResult result = MessageBox.Show("Có nội dung tab đã thay đổi. Bạn có muốn lưu trước khi đóng không?", "Lưu trước khi đóng", MessageBoxButtons.YesNoCancel);

                    if (result == DialogResult.Yes)
                    {
                        // Lưu nội dung tab trước khi đóng
                        SaveTabContent(tabPage);
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        // Hủy việc đóng cửa sổ nếu người dùng chọn Cancel
                        return;
                    }
                }
            }

            // Đóng cửa sổ hiện tại (Form)
            this.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // Kiểm tra xem có nội dung tab nào chưa được lưu
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                RichTextBox richTextBox = tabPage.Controls.OfType<RichTextBox>().FirstOrDefault();
                if (richTextBox != null && richTextBox.Modified)
                {
                    // Nếu có nội dung tab nào đã thay đổi chưa được lưu, hỏi người dùng có muốn lưu trước khi thoát không
                    DialogResult result = MessageBox.Show("Có nội dung tab đã thay đổi. Bạn có muốn lưu trước khi thoát không?", "Lưu trước khi thoát", MessageBoxButtons.YesNoCancel);

                    if (result == DialogResult.Yes)
                    {
                        // Lưu nội dung tab trước khi thoát
                        SaveTabContent(tabPage);
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        // Hủy việc thoát ứng dụng nếu người dùng chọn Cancel
                        return;
                    }
                }
            }

            // Đóng ứng dụng Notepad
            Application.Exit();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();

            if (richTextBox != null)
            {
                if (richTextBox.CanUndo)
                {
                    richTextBox.Undo();
                }
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();

            if (richTextBox != null)
            {
                if (richTextBox.SelectionLength > 0)
                {
                    richTextBox.Cut();
                }
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();

            if (richTextBox != null)
            {
                if (richTextBox.SelectionLength > 0)
                {
                    richTextBox.Copy();
                }
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();

            if (richTextBox != null)
            {
                richTextBox.Paste();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();

            if (richTextBox != null)
            {
                richTextBox.SelectedText = string.Empty;
            }
        }

        public void Find(string searchText)
        {
            //searchResults.Clear();
            //currentSearchIndex = -1;

            //int currentIndex = 0;
            //while (currentIndex < richTextBox1.TextLength)
            //{
            //    currentIndex = richTextBox1.Find(searchText, currentIndex, RichTextBoxFinds.None);
            //    if (currentIndex < 0)
            //        break;

            //    searchResults.Add(currentIndex);
            //    currentIndex += searchText.Length;
            //}

            //if (searchResults.Count > 0)
            //{
            //    currentSearchIndex = 0;
            //    richTextBox1.Select(searchResults[currentSearchIndex], searchText.Length);
            //    richTextBox1.ScrollToCaret();
            //}
            int startIndex = currentSearchIndex + 1;
            int index = richTextBox1.Text.IndexOf(searchText, startIndex, StringComparison.CurrentCultureIgnoreCase);

            if (index >= 0)
            {
                // Bỏ highlight dữ liệu cũ
                ClearHighlight();

                // Highlight dữ liệu mới
                HighlightText(searchText, index);
                currentSearchIndex = index;
            }
        }
        public void FindNext(string searchText)
        {
            //if (searchResults.Count == 0)
            //{
            //    Find(searchText);
            //}
            //else if (currentSearchIndex < searchResults.Count - 1)
            //{
            //    currentSearchIndex++;
            //    richTextBox1.Select(searchResults[currentSearchIndex], searchText.Length);
            //    richTextBox1.ScrollToCaret();
            //}
            int startIndex = currentSearchIndex + 1;
            int index = richTextBox1.Text.IndexOf(searchText, startIndex, StringComparison.CurrentCultureIgnoreCase);

            if (index >= 0)
            {
                // Bỏ highlight dữ liệu cũ
                ClearHighlight();

                // Highlight dữ liệu mới
                HighlightText(searchText, index);
                currentSearchIndex = index;
            }
        }

        public void FindPrevious(string searchText)
        {
            //if (searchResults.Count == 0)
            //{
            //    Find(searchText);
            //}
            //else if (currentSearchIndex > 0)
            //{
            //    currentSearchIndex--;
            //    richTextBox1.Select(searchResults[currentSearchIndex], searchText.Length);
            //    richTextBox1.ScrollToCaret();
            //}
            int startIndex = currentSearchIndex - 1;
            int index = richTextBox1.Text.LastIndexOf(searchText, startIndex, StringComparison.CurrentCultureIgnoreCase);

            if (index >= 0)
            {
                // Bỏ highlight dữ liệu cũ
                ClearHighlight();

                // Highlight dữ liệu mới
                HighlightText(searchText, index);
                currentSearchIndex = index;
            }
        }
        private void HighlightText(string searchText, int index)
        {
            richTextBox1.SelectionStart = index;
            richTextBox1.SelectionLength = searchText.Length;
            richTextBox1.SelectionBackColor = Color.Yellow;

            // Lưu vị trí đã highlight
            highlightedIndices.Add(index);
        }

        private void ClearHighlight()
        {
            // Bỏ highlight dữ liệu cũ
            foreach (int index in highlightedIndices)
            {
                richTextBox1.SelectionStart = index;
                richTextBox1.SelectionLength = richTextBox1.Text.Length;
                richTextBox1.SelectionBackColor = richTextBox1.BackColor;
            }

            // Xóa danh sách vị trí đã highlight
            highlightedIndices.Clear();
        }
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //// Tạo hộp thoại tìm kiếm


            //// Hiển thị hộp thoại tìm kiếm
            findDialog.ShowDialog();

        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string searchText = findDialog.GetSearchText();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                FindNext(searchText);
            }
        }

        private void findPreviousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string searchText = findDialog.GetSearchText();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                FindPrevious(searchText);
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            findDialog.ShowDialog();


        }

        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tạo một hộp thoại để nhập số dòng
            InputBox inputBox = new InputBox("Go To Line", "Enter line number:");

            // Hiển thị hộp thoại
            if (inputBox.ShowDialog() == DialogResult.OK)
            {
                int lineNumber;
                if (int.TryParse(inputBox.InputText, out lineNumber))
                {
                    if (lineNumber >= 1 && lineNumber <= richTextBox1.Lines.Length)
                    {
                        // Tính toán vị trí của ký tự đầu tiên trong dòng muốn đến
                        int index = richTextBox1.GetFirstCharIndexFromLine(lineNumber - 1);
                        if (index >= 0)
                        {
                            // Chuyển đến dòng muốn đến
                            richTextBox1.SelectionStart = index;
                            richTextBox1.ScrollToCaret();
                        }
                    }
                    else
                    {
                        MessageBox.Show("The line number is beyond the total number of lines", "Notepad - Goto line");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid line number.", "Go To Line");
                }
            }
        }
        public class InputBox : Form
        {
            private Label label;
            private TextBox textBox;
            private Button okButton;
            private Button cancelButton;

            public string InputText { get { return textBox.Text; } }

            public InputBox(string title, string prompt)
            {
                this.Text = title;
                label = new Label { Text = prompt, AutoSize = true, Left = 10, Top = 10 };
                textBox = new TextBox { Left = 10, Top = 30, Width = 200 };
                okButton = new Button { Text = "OK", Left = 10, Top = 60, Width = 80 };
                cancelButton = new Button { Text = "Cancel", Left = 100, Top = 60, Width = 80 };
                okButton.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };
                cancelButton.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
                Controls.Add(label);
                Controls.Add(textBox);
                Controls.Add(okButton);
                Controls.Add(cancelButton);
                AcceptButton = okButton;
                CancelButton = cancelButton;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                StartPosition = FormStartPosition.CenterParent;
                ShowInTaskbar = false;
                Size = new Size(250, 150);
            }
        }
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();

            if (richTextBox != null)
            {
                richTextBox.SelectAll();
            }
        }

        private void timedateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();

            if (richTextBox != null)
            {
                // Lấy thời gian và ngày hiện tại
                string currentDateTime = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");

                // Chèn thời gian và ngày vào vị trí con trỏ hiện tại
                int selectionStart = richTextBox.SelectionStart;
                richTextBox.Text = richTextBox.Text.Insert(selectionStart, currentDateTime);
                richTextBox.SelectionStart = selectionStart + currentDateTime.Length;
                richTextBox.Focus();
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();

            if (richTextBox != null)
            {
                // Tạo hộp thoại chọn font
                FontDialog fontDialog = new FontDialog();

                // Lấy font hiện tại từ RichTextBox
                fontDialog.Font = richTextBox.Font;

                // Hiển thị hộp thoại chọn font
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    // Cập nhật font trong RichTextBox
                    richTextBox.Font = fontDialog.Font;
                }
            }
        }


        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();

            if (richTextBox != null)
            {
                // Tăng kích thước font
                if (richTextBox.Font.Size < 100)
                {
                    float newSize = richTextBox.Font.Size + 2.0f; // Tăng kích thước lên 2 điểm
                    richTextBox.Font = new Font(richTextBox.Font.FontFamily, newSize);
                }
            }
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();

            if (richTextBox != null)
            {
                // Giảm kích thước font
                if (richTextBox.Font.Size > 6)
                {
                    float newSize = richTextBox.Font.Size - 2.0f; // Giảm kích thước xuống 2 điểm
                    richTextBox.Font = new Font(richTextBox.Font.FontFamily, newSize);
                }
            }
        }

        private void restoreDefaultZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();

            if (richTextBox != null)
            {
                // Khôi phục kích thước font mặc định
                richTextBox.Font = new Font(richTextBox.Font.FontFamily, 12.0f); // Sử dụng 12 điểm làm kích thước mặc định, có thể điều chỉnh theo ý muốn
            }
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (statusStrip1 != null)
            {
                // Đảm bảo trạng thái ban đầu là có thể nhìn thấy StatusBar
                isStatusBarVisible = !isStatusBarVisible;
                statusStrip1.Visible = isStatusBarVisible;
            }
        }

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentTabPage = tabControl1.SelectedTab;
            RichTextBox richTextBox = currentTabPage.Controls.OfType<RichTextBox>().FirstOrDefault();

            if (richTextBox != null)
            {
                // Bật hoặc tắt tính năng tự động gói từ
                richTextBox.WordWrap = !richTextBox.WordWrap;
            }
        }



        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            int position = richTextBox1.SelectionStart;

            // Tính toán dòng và cột dựa trên vị trí con trỏ
            int line = richTextBox1.GetLineFromCharIndex(position) + 1; // Bắt đầu từ 1
            int column = position - richTextBox1.GetFirstCharIndexFromLine(line - 1) + 1; // Bắt đầu từ 1

            // Cập nhật giá trị trên StatusBar
            lineColumnStatustoolStripStatusLabel1.Text = $"Ln {line}, Col {column}";
        }


    }
}