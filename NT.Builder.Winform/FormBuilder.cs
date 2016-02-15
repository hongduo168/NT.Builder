using NT.Builder.DbHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT.Builder.Winform
{
    public partial class FormBuilder : Form
    {
        private DbUtility db = null;
        private DataTable tables;
        public FormBuilder()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            db = new DbUtility(textBox1.Text.Trim(), textBox2.Text.Trim(), textBox4.Text.Trim(), textBox3.Text.Trim());
            if (db.IsConnection)
            {
                tables = db.Tables();
                List<string> listBox = new List<string>();
                foreach (DataRow row in tables.Rows)
                {
                    string title = row["name"].ToString();
                    if (row["xtype"].ToString().Trim() == "V")
                    {
                        title += "（视图）";
                    }
                    listBox.Add(title);
                }

                listBox1.DataSource = listBox;



                this.ButtonState(false);
            }
            else
            {
                MessageBox.Show("请检查数据库信息是否正确！");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.ButtonState(true);

        }

        /// <summary>
        /// 控制全局按钮可用状态
        /// </summary>
        /// <param name="allowInout"></param>
        private void ButtonState(bool allowInout = false)
        {
            if (allowInout)
            {
                textBox1.Enabled = textBox2.Enabled = textBox3.Enabled = textBox4.Enabled = button1.Enabled = true;
                button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = button6.Enabled = false;
            }
            else
            {
                textBox1.Enabled = textBox2.Enabled = textBox3.Enabled = textBox4.Enabled = button1.Enabled = false;
                button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = button6.Enabled = true;
            }
        }

        /// <summary>
        /// 选择数据表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (tables.Rows.Count < 1) return;
            listBox1.SelectedIndices.Clear();
            var keyItem = tables.Select("xtype='V'").FirstOrDefault();
            if (keyItem != null)
            {
                var index = tables.Rows.IndexOf(keyItem);

                for (int i = 0; i < index; i++)
                {
                    listBox1.SelectedIndices.Add(i);
                }
            }
        }

        /// <summary>
        /// 选择视图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (tables.Rows.Count < 1) return;
            listBox1.SelectedIndices.Clear();
            var keyItem = tables.Select("xtype='V'").FirstOrDefault();
            if (keyItem != null)
            {
                var index = tables.Rows.IndexOf(keyItem);

                for (int i = index; i < tables.Rows.Count; i++)
                {
                    listBox1.SelectedIndices.Add(i);
                }
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (tables.Rows.Count < 1) return;
            for (int i = 0; i < tables.Rows.Count; i++)
            {
                listBox1.SelectedIndices.Add(i);
            }
        }

        /// <summary>
        /// 生成Model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            var dialog = folderBrowserDialog1.ShowDialog();
            if (dialog == DialogResult.OK)
            {



                backgroundWorker1.RunWorkerAsync();
                progressBar1.Visible = true;
                button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = button6.Enabled = false;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(5000);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;

            button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = button6.Enabled = true;
        }
    }
}
