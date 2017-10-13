using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Quad64
{
    partial class SelectLevelForm : Form
    {
        public ushort levelID = 0x10;
        public bool changeLevel = false;
        public SelectLevelForm(ushort levelID)
        {
            InitializeComponent();
            this.levelID = levelID;
            ROM rom = ROM.Instance;
			//comboBox1.Item
			foreach (var entry in rom.getLevelEntriesCopy())
				comboBox1.Items.Add(entry.Title + " (0x" + entry.ID.ToString("X2") + ")");
            //comboBox1.Items.Add("Custom ID value");
            comboBox1.SelectedIndex = rom.getLevelIndex(levelID);
        }

        private void button1_Click(object sender, EventArgs e)
		{
			if (ROM.Instance.getLevelEntry(comboBox1.SelectedIndex, out LevelEntry entry))
			{
				levelID = entry.ID;
				changeLevel = true;
			}
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
