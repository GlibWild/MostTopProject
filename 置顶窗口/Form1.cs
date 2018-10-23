using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 置顶窗口
{
    public partial class Form1 : Form
    {
        string fileName = string.Empty;
        string[] PicExtensions = { ".gif", ".jpg", ".jpeg", ".png", ".bmp" };
        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
        }
        void Form1_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            int width = this.Width;
            int height = this.Height;
            Rectangle rect = System.Windows.Forms.SystemInformation.VirtualScreen;
            this.Location = new Point(rect.Width - width, 0);
            this.AllowDrop = true;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.TopMost = true;
        }
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            TranslateToFile(e);
        }

        private object TranslateToFile(DragEventArgs e)
        {
            object obj = null;
            try
            {
                if (obj == null)
                {
                    string format = DataFormats.Text;
                    obj = e.Data.GetData(format, false);
                    if (obj != null)
                    {
                        this.pictureBox1.Visible = false;
                        this.richTextBox1.Visible = true;
                        this.statusStrip1.Visible = true;
                        this.richTextBox1.Text = (String)obj;
                        TextWriter writer = new StreamWriter(Path.Combine(Application.StartupPath, "~tmp.txt"),false);
                        writer.Write((String)obj);
                        writer.Close();
                        FileInfo file = new FileInfo(Path.Combine(Application.StartupPath, "~tmp.txt"));
                        file.Attributes = FileAttributes.Hidden;
                        fileName = file.FullName;
                    }
                }
                if (obj == null)
                {
                    string format = DataFormats.Bitmap;
                    obj = e.Data.GetData(format, false);
                    if (obj != null)
                    {
                        this.pictureBox1.Visible = true;
                        this.richTextBox1.Visible = false;
                        this.statusStrip1.Visible = false;
                        this.pictureBox1.Image = (Bitmap)obj;
                    }
                }
                if (obj == null)
                {
                    string format = DataFormats.FileDrop;
                    obj = e.Data.GetData(format, false);
                    if (obj != null)
                    {
                        //获取第一个文件名
                        fileName = (obj as String[])[0];
                        try
                        {
                            if (PicExtensions.Contains(Path.GetExtension(fileName).ToLower()))
                            {
                                this.pictureBox1.Visible = true;
                                this.richTextBox1.Visible = false;
                                this.statusStrip1.Visible = false;
                                this.pictureBox1.ImageLocation = fileName;
                            }
                            else
                            {
                                this.pictureBox1.Visible = false;
                                this.richTextBox1.Visible = true;
                                this.statusStrip1.Visible = true;
                                this.richTextBox1.Clear();
                                foreach (var item in File.ReadLines(fileName, Encoding.Default))
                                {
                                    string tmp = item.Replace("\0", "");
                                    this.richTextBox1.AppendText(tmp);
                                    this.richTextBox1.AppendText(Environment.NewLine);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            this.pictureBox1.Visible = false;
                            this.richTextBox1.Visible = false;
                            this.statusStrip1.Visible = false;
                            MessageBox.Show("文件格式不对");
                        }
                    }
                }
            }
            catch { }
            return obj;
        }

        private void toolStripDropDownButton1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.richTextBox1.Clear();
            if (e.ClickedItem.Name == "tUTF8")
            {
                tUTF8.Checked = true;
                tANSI.Checked = false;
                tGBK.Checked = false;
                foreach (var item in File.ReadLines(fileName, Encoding.UTF8))
                {
                    string tmp = item.Replace("\0", "");
                    this.richTextBox1.AppendText(tmp);
                    this.richTextBox1.AppendText(Environment.NewLine);
                }
            }
            else if (e.ClickedItem.Name == "tANSI")
            {
                tUTF8.Checked = false;
                tANSI.Checked = true;
                tGBK.Checked = false;
                foreach (var item in File.ReadLines(fileName, Encoding.Default))
                {
                    string tmp = item.Replace("\0", "");
                    this.richTextBox1.AppendText(tmp);
                    this.richTextBox1.AppendText(Environment.NewLine);
                }
            }
            else if (e.ClickedItem.Name == "tGBK")
            {
                tUTF8.Checked = false;
                tANSI.Checked = false;
                tGBK.Checked = true;
                foreach (var item in File.ReadLines(fileName, Encoding.GetEncoding("GBK")))
                {
                    string tmp = item.Replace("\0", "");
                    this.richTextBox1.AppendText(tmp);
                    this.richTextBox1.AppendText(Environment.NewLine);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (File.Exists(Path.Combine(Application.StartupPath, "~tmp.txt")))
            {
                File.Delete(Path.Combine(Application.StartupPath, "~tmp.txt"));
            }
        }
    }
}
