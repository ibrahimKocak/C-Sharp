using System;
using System.Drawing;
using System.Windows.Forms;

namespace BiyoenformatikOdev1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool text_correct;
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            text_correct = true;

            foreach (char c in richTextBox1.Text)
            {
                if (c != Convert.ToChar("A") && c != Convert.ToChar("C") && c != Convert.ToChar("G") && c != Convert.ToChar("T"))
                {
                    text_correct = false;
                    break;
                }

                richTextBox2.Text = richTextBox1.Text.Replace("T", "U");
            }

            if (text_correct == false)
                richTextBox2.Text = "*Unexpected character input, please check your input!\n\nNot:The Input should be a DNA code like AATCCGAGGCT without any blank, blank line, lower case or other input.";
        }
        
        //Down just about GUI
        private void Form1_Resize(object sender, EventArgs e)
        {
            richTextBox1.Width = this.Width / 2 - 20;
            richTextBox2.Width = this.Width / 2 - 20;
            richTextBox1.Height = this.Height - 100;
            richTextBox2.Height = this.Height - 100;

            richTextBox2.Location = new Point(richTextBox1.Location.X + richTextBox1.Width + 5, richTextBox2.Location.Y);
            linkLabel1.Location = new Point(this.Width - 50, 9);
            linkLabel2.Location = new Point(this.Width - 50, this.Height - 55);

            label2.Location = new Point(richTextBox2.Location.X, label2.Location.Y);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("The application converts DNA sequence to mRNA sequence. For action just need to enter DNA sequence to left box. The application automatically convert it to mRNA sequence.");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Coded by ibrahim Kocak");
        }
    }
}
