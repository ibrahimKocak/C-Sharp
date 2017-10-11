using System;
using System.Drawing;
using System.Windows.Forms;

namespace BiyoenformatikOdev2
{
    public partial class Form1 : Form
    {
        private string[] codons ={"TTT", "TTC", "TTA", "TTG", "TCT","TCC", "TCA", "TCG", "TAT", "TAC", "TAA", "TAG", "TGT", "TGC", "TGA", "TGG",
                                  "CTT","CTC", "CTA", "CTG", "CCT", "CCC", "CCA", "CCG", "CAT", "CAC", "CAA", "CAG", "CGT", "CGC", "CGA", "CGG",
                                  "ATT", "ATC", "ATA", "ATG", "ACT", "ACC", "ACA", "ACG", "AAT", "AAC", "AAA", "AAG","AGT", "AGC", "AGA", "AGG",
                                  "GTT", "GTC", "GTA", "GTG", "GCT", "GCC", "GCA", "GCG", "GAT", "GAC", "GAA", "GAG", "GGT", "GGC", "GGA", "GGG"};

        private string[] aminos = {"F", "F", "L", "L", "S", "S", "S", "S", "Y", "Y", "*", "*", "C", "C", "*", "W",
                                   "L", "L", "L", "L", "P", "P", "P", "P", "H", "H", "Q", "Q", "R", "R", "R", "R",
                                   "I", "I", "I", "M", "T", "T", "T", "T", "N", "N", "K", "K", "S", "S", "R", "R",
                                   "V", "V", "V", "V", "A", "A", "A", "A", "D", "D", "E", "E", "G","G", "G", "G", };

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        bool text_correct;
        string codon;
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            codon = "";
            text_correct = true;
            
            if(comboBox1.SelectedIndex == 0)
            {
                foreach (char c in richTextBox1.Text.Replace("U", "T"))
                {
                    if (c != Convert.ToChar("A") && c != Convert.ToChar("C") && c != Convert.ToChar("G") && c != Convert.ToChar("T"))
                    {
                        text_correct = false;
                        break;
                    }

                    codon += c;

                    if(codon.Length == 3)
                    {
                        richTextBox2.Text = richTextBox2.Text + aminos[Array.IndexOf(codons, codon)];
                        codon = "";
                    }
                }

                if (text_correct == false)
                    richTextBox2.Text = "*Unexpected character input, please check your input!\n\nNot:The Input should be a DNA code like AATCCGAGGCT (like FLVHNDIA for amino acids) without any blank, blank line, lower case or other input.";
            }
            else
            {
                foreach (char c in richTextBox1.Text)
                {
                    if (Array.IndexOf(aminos, Convert.ToString(c)) == -1)
                    {
                        text_correct = false;
                        break;
                    }
                    
                    richTextBox2.Text = richTextBox2.Text + codons[Array.IndexOf(aminos, Convert.ToString(c))];
                }

                if (text_correct == false)
                    richTextBox2.Text = "*Unexpected character input, please check your input!\n\nNot:The Input should be a DNA code like AATCCGAGGCT (like FLVHNDIA for amino acids) without any blank, blank line, lower case or other input.";
            }
        }
        
        //Down just about GUI
        private void Form1_Resize_1(object sender, EventArgs e)
        {
            richTextBox1.Width = this.Width / 2 - 20;
            richTextBox2.Width = this.Width / 2 - 20;
            richTextBox1.Height = this.Height -100;
            richTextBox2.Height = this.Height -100;

            richTextBox2.Location = new Point(richTextBox1.Location.X + richTextBox1.Width + 5, richTextBox2.Location.Y);
            linkLabel1.Location = new Point(this.Width - 50, 9);
            linkLabel2.Location = new Point(this.Width - 50, this.Height - 55);
            
            label2.Location = new Point(richTextBox2.Location.X, label2.Location.Y);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("The application converts DNA sequence or protein sequence to other one. For action just need to enter inputs to left box. The application automatically detects the input and convert it to other one.");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Coded by ibrahim Kocak");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }
    }
}
