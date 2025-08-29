using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assembler
{
    public partial class PopUp : Form
    {
        public PopUp()
        {
            InitializeComponent();
            button1.Text = "Using the program\r\n\r\nAddressing Modes:\r\n\r\nImmediate\r\nIn order to make use of immediate addressing you will use the prefix ‘#’\r\nFor example, “INP R3 #2”\r\nThis will load the denary value 2 into register 3.\r\n\r\nRelative \r\nIn order to make use of relative addressing you will use the prefix ‘@’\r\nFor Example, “INP R3 @2”\r\nThis will load the value stored in memory address 2 into register 3.\r\n\r\nDirect\r\nIn order to make use of direct addressing you simple need to do the number of register\r\nFor example, “INP R3 2”\r\nThis will store the value in register 2 into register 3.\r\n";
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
