using System;
using System.Windows.Forms;

namespace CallRecorder.Forms
{
    public partial class SaveRecordForm : Form
    {
        public string Id { get; private set; } = string.Empty;
        public SaveRecordForm()
        {
            InitializeComponent();
            textBox1.KeyDown += (sender, args) => {
                if (args.KeyCode == Keys.Enter)
                {
                    button1.PerformClick();
                }
            };
        }
        private void SaveRecordForm_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                errorLabel.Visible = true;
                textBox1.SelectAll();
                return;
            }
            if (textBox1.Text.Contains("details/"))
            {
                int i = textBox1.Text.IndexOf("details/") + 8;
                while (textBox1.Text[i] != '/')
                {
                    Id += textBox1.Text[i];
                    i++;
                }
            }
            else
            {
                foreach (char c in textBox1.Text)
                {
                    if (c < '0' || c > '9')
                    {
                        errorLabel.Visible = true;
                        textBox1.SelectAll();
                        return;
                    }
                }
                Id = textBox1.Text;
            }

            Close();
        }
    }
}
