using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlterBotNetGUI
{
    public partial class FrmAlterBot : Form
    {
        public FrmAlterBot()
        {
            InitializeComponent();
        }

        // ReSharper disable UnusedParameter.Local
        private void BtnQuitter_Click(object sender, EventArgs e)
            // ReSharper restore UnusedParameter.Local
        {
            Close();
            Dispose();
        }
    }
}
