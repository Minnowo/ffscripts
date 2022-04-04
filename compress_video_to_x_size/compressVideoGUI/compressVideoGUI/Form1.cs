using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace compressVideoGUI
{
    public partial class Form1 : Form
    {
        string PYTHON = GetPythonPath();
        string COMPRESS = "compress.py";
        string BATCH_PY = "batch_plus_rename.py";
        string FFMPEG = "..\\.ffmpeg\\ffmpeg.exe";
        string FFPROBE = "..\\.ffmpeg\\ffprobe.exe";

        string inputPath;
        string outputFormat;
        bool   audioOnly = false;
        bool   videoOnly = false;
        bool   overwrite = false;
        float  targetFileSize = 8;

        bool _preventOverflow = false;

        public Form1()
        {
            InitializeComponent();
            textBox3.Text = FFMPEG;
            textBox4.Text = FFPROBE;
            textBox6.Text = BATCH_PY;
            textBox7.Text = COMPRESS;
            textBox8.Text = PYTHON;
            numericUpDown1.Value = (decimal)targetFileSize;
        }

        private void InputFile_TextChanged(object sender, EventArgs e)
        {
            if (this._preventOverflow)
                return;

            inputPath = textBox1.Text;
        }

        private void OutputNameTemplate_TextChanged(object sender, EventArgs e)
        {
            if (this._preventOverflow)
                return;

            outputFormat = textBox2.Text;
        }

        private void BatchRenamePy_TextChanged(object sender, EventArgs e)
        {
            if (this._preventOverflow)
                return;

            BATCH_PY = textBox6.Text;
        }
        
        private void PythonPath_TextChanged(object sender, EventArgs e)
        {
            if (this._preventOverflow)
                return;

            PYTHON = textBox8.Text;
        }

        private void FFMPEG_Path_TextChanged(object sender, EventArgs e)
        {
            if (this._preventOverflow)
                return;

            FFMPEG = textBox3.Text;
        }

        private void FFPROBE_Path_TextChanged(object sender, EventArgs e)
        {
            if (this._preventOverflow)
                return;

            FFPROBE = textBox4.Text;
        }

        private void TargetFileSIzeMB_ValueChanged(object sender, EventArgs e)
        {
            if (this._preventOverflow)
                return;

            targetFileSize = (float)numericUpDown1.Value;
        }

        private void CompressPyPath_TextChanged(object sender, EventArgs e)
        {
            if (this._preventOverflow)
                return;

            COMPRESS = textBox7.Text;
        }

        private void OverwriteFiles_CheckedChanged(object sender, EventArgs e)
        {
            if (this._preventOverflow)
                return;
            
            overwrite = checkBox1.Checked;
        }

        private void RemoveAudio_CheckedChanged(object sender, EventArgs e)
        {
            if (this._preventOverflow)
                return;

            this._preventOverflow = true;
            CheckBox cb = sender as CheckBox;

            if (cb.Checked)
            {
                videoOnly = true;
                audioOnly = false;
                checkBox3.Checked = false; // disable the video only checkbox
                textBox5.Text = "mp4";
            }
            else
            {
                textBox5.Text = "mp4";
                audioOnly = checkBox3.Checked;
                videoOnly = false;
            }
            this._preventOverflow = false;
        }

        private void RemoveVideo_CheckedChanged(object sender, EventArgs e)
        {
            if (this._preventOverflow)
                return;

            this._preventOverflow = true;
            CheckBox cb = sender as CheckBox;

            if (cb.Checked)
            {
                audioOnly = true;
                videoOnly = false;
                checkBox2.Checked = false;
                textBox5.Text = "mp3";
            }
            else
            {
                textBox5.Text = "mp4";
                audioOnly = false;
                videoOnly = checkBox2.Checked;
            }
            this._preventOverflow = false;
        }

        private void BrowseInputFile_Click(object sender, EventArgs e)
        {
            string input = AskChooseFile();

            if (string.IsNullOrEmpty(input))
                return;

            textBox1.Text = input;
        }

        private void BrowseFFMPEGPath_Click(object sender, EventArgs e)
        {
            string input = AskChooseFile();

            if (string.IsNullOrEmpty(input))
                return;

            textBox3.Text = input;
        }

        private void BrowseFFPROBEPath_Click(object sender, EventArgs e)
        {
            string input = AskChooseFile();

            if (string.IsNullOrEmpty(input))
                return;

            textBox4.Text = input;
        }

        private void BrowseBatchPy_Click(object sender, EventArgs e)
        {
            string input = AskChooseFile();

            if (string.IsNullOrEmpty(input))
                return;

            textBox6.Text = input;
        }
        
        private void BrowseCompressPy_Click(object sender, EventArgs e)
        {
            string input = AskChooseFile();

            if (string.IsNullOrEmpty(input))
                return;

            textBox7.Text = input;
        }

        private void BrowsePythonPath_Click(object sender, EventArgs e)
        {
            string input = AskChooseFile();

            if (string.IsNullOrEmpty(input))
                return;

            textBox8.Text = input;
        }

        private void Run_Click(object sender, EventArgs e)
        {
            if (this._preventOverflow)
                return;

            if (!File.Exists(inputPath) && !Directory.Exists(inputPath))
            {
                ShowERROR($"The input path \"{inputPath}\" does not exist.");
                return;
            }

            if(!File.Exists(PYTHON))
            {
                ShowERROR($"The python path \"{PYTHON}\" does not exist.");
                return;
            }

            bool isBatch = Directory.Exists(inputPath); // if they input a folder, use batch

            List<string> args = new List<string>();
            args.Add(PYTHON);

            if (isBatch)
            {
                if (!File.Exists(BATCH_PY))
                {
                    ShowERROR($"The could not find batch_plus_rename.py, the given path \"{BATCH_PY}\" does not exist.");
                    return;
                }

                if (overwrite)
                {
                    ShowWARNING("Batch option does not support file overwrite, original files will remain.");
                }
                args.Add(BATCH_PY);

                outputFormat = outputFormat.Replace("\\", "\\\\");
                outputFormat = outputFormat.Replace("\"", "\\\"");

                args.Add("-f");
                args.Add('"' + outputFormat + '"');
            }
            else
            {
                if (!File.Exists(COMPRESS))
                {
                    ShowERROR($"The could not find compress.py, the given path \"{COMPRESS}\" does not exist.");
                    return;
                }


                args.Add(COMPRESS);

                if(overwrite)
                    args.Add("-o");

            }
            

            PYTHON = new FileInfo(PYTHON).FullName.Replace("\\", "\\\\");
            inputPath = new FileInfo(inputPath).FullName.Replace("\\", "\\\\");
            FFMPEG = new FileInfo(FFMPEG).FullName.Replace("\\", "\\\\");
            FFPROBE = new FileInfo(FFPROBE).FullName.Replace("\\", "\\\\");

            args.Add("-i");
            args.Add('"' + inputPath+ '"');
            args.Add("-t");
            args.Add(targetFileSize.ToString());
            
            args.Add("-fp");
            args.Add('"' + FFMPEG+ '"');
            args.Add("-fb");
            args.Add('"' + FFPROBE+ '"');

            if (audioOnly)
                args.Add("-ao");

            if (videoOnly)
                args.Add("-na");

            this._preventOverflow = true;
            this.Enabled = false;

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            // cmd.StartInfo.RedirectStandardInput = true;
            // cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = false;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.Arguments = "/C " + string.Join(" ", args);

            Console.WriteLine("\nrunning...\n" + string.Join(" ", args));

            cmd.Start();
            cmd.WaitForExit();

            // cmd.StandardOutput

            this.Enabled = true;
            this._preventOverflow = false;
        }

        private string AskChooseFile()
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "All Files (*.*)|*.*";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                return choofdlog.FileName;
            }
            return "";
        }

        private void ShowERROR(string text)
        {
            MessageBox.Show(this, text, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowWARNING(string text)
        {
            MessageBox.Show(this, text, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static string GetPythonPath(string requiredVersion = "", string maxVersion = "")
        {
            // this is really dumb, but it works so 
            string[] path = System.Environment.GetEnvironmentVariable("path").Split(';');

            foreach (string value in path)
            {
                string p = Path.Combine(value, "python.exe");
                if (File.Exists(p))
                {
                    Console.WriteLine("python found: " + p);
                    return p;
                }
            }

            return "";
        }


    }
}
