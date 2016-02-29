using NT.Builder.DbHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT.Builder.Winform
{
    public partial class FormBuilder : Form
    {
        private const string PROPERTYCODE = "        public virtual {1} {0} {{ get; set; }}";
        private DbUtility db = null;
        private DataTable tables;
        private string namespaceText = string.Empty;
        private string classbaseText = string.Empty;
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
                List<int> s = new List<int>();
                for (int i = 0; i < listBox1.SelectedIndices.Count; i++)
                {
                    s.Add(listBox1.SelectedIndices[i]);
                }
                namespaceText = textBox6.Text.Trim();
                classbaseText = textBox5.Text.Trim();

                backgroundWorker1.RunWorkerAsync(s);
                progressBar1.Visible = true;
                button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = button6.Enabled = false;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var item in (List<int>)e.Argument)
            {
                var name = tables.Rows[item]["name"].ToString();
                DataTable dt = db.TableInfo(name);

                string fileName = string.Format("{0}Info", name);
                StringBuilder fileText = new StringBuilder();

                fileText.AppendFormat("namespace {0}", namespaceText).AppendLine();
                fileText.AppendLine("{");
                fileText.AppendFormat("    public partial class {0} : {1}", fileName, string.IsNullOrEmpty(classbaseText) ? "ModelBase" : classbaseText).AppendLine();
                fileText.AppendLine("    {");
                foreach (DataColumn column in dt.Columns)
                {
                    fileText.AppendFormat(PROPERTYCODE, column.ColumnName, column.DataType.Name).AppendLine();
                }
                fileText.AppendLine("    }");
                fileText.AppendLine("}");

                File.WriteAllText(Path.Combine(folderBrowserDialog1.SelectedPath, fileName + ".cs"), fileText.ToString());
            }
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
