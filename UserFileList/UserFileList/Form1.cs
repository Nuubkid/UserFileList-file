using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace UserFileList
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a folder";
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderBrowserDialog.SelectedPath;
                    List<string> files = GetAllFilesInFolder(folderPath);

                    int totalFiles = files.Count;
                    int processedFiles = 0;

                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = totalFiles;
                    progressBar1.Step = 1;

                    foreach (string file in files)
                    {
                        string relativePath = GetRelativePath(folderPath, file);
                        // Replace forward slashes with backward slashes
                        relativePath = relativePath.Replace("/", "\\");
                        string hash = ComputeFileHash(file);
                        ListViewItem item = new ListViewItem(relativePath);
                        item.SubItems.Add(hash);
                        listViewFiles.Items.Add(item);

                        processedFiles++;
                        progressBar1.Value = processedFiles; // Update the progress bar value
                    }

                    // Reset the progress bar value to 0
                    progressBar1.Value = 0;
                }
            }
        }

        private List<string> GetAllFilesInFolder(string folderPath)
        {
            List<string> files = new List<string>();
            string[] subfolders = Directory.GetDirectories(folderPath);

            foreach (string file in Directory.GetFiles(folderPath))
            {
                files.Add(file);
            }

            foreach (string subfolder in subfolders)
            {
                files.AddRange(GetAllFilesInFolder(subfolder));
            }

            return files;
        }

        private string GetRelativePath(string rootPath, string fullPath)
        {
            Uri rootUri = new Uri(rootPath);
            Uri fullUri = new Uri(fullPath);
            return Uri.UnescapeDataString(rootUri.MakeRelativeUri(fullUri).ToString());
        }

        private string ComputeFileHash(string filePath)
        {
            using (MD5 md5 = MD5.Create())
            using (FileStream stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = md5.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in listViewFiles.SelectedItems)
                {
                    listViewFiles.Items.Remove(item);
                }
            }
        }

        private void SaveUserFileList(string filePath)
        {
            using (XmlTextWriter writer = new XmlTextWriter(filePath, null))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("list");

                foreach (ListViewItem item in listViewFiles.Items)
                {
                    string fileName = item.Text;
                    string hash = item.SubItems[1].Text;

                    // Add a backslash to the beginning of the local attribute
                    string localPath = "\\" + fileName;

                    writer.WriteStartElement("file");
                    writer.WriteAttributeString("local", localPath);
                    writer.WriteAttributeString("hash", hash);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    

        private void button3_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Save userFileList.dat";
                saveFileDialog.Filter = "User File List|userFileList.dat";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    SaveUserFileList(filePath);
                    MessageBox.Show("userFileList.dat saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select multiple files";
                openFileDialog.Filter = "All Files|*.*";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    int totalFiles = openFileDialog.FileNames.Length;
                    int processedFiles = 0;

                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = totalFiles;
                    progressBar1.Step = 1;

                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        // Replace forward slashes with backward slashes in the file path
                        string normalizedFilePath = filePath.Replace("/", "\\");

                        string fileName = Path.GetFileName(normalizedFilePath);
                        string hash = ComputeFileHash(normalizedFilePath);

                        ListViewItem item = new ListViewItem(fileName);
                        item.SubItems.Add(hash);
                        listViewFiles.Items.Add(item);

                        processedFiles++;
                        progressBar1.Value = processedFiles; // Update the progress bar value
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<string> hashes = new List<string>();

            foreach (ListViewItem item in listViewFiles.Items)
            {
                string filePath = item.Text;
                string hash = item.SubItems[1].Text;
                hashes.Add(hash);
            }

            // Calculate the hash of all merged files
            string combinedHash = ComputeCombinedHash(hashes);
            string hasdhedtext = combinedHash;
            // Display the combined hash in TextBox1
            textBox1.Text = hasdhedtext.ToUpper();
        }
        private string ComputeCombinedHash(List<string> hashes)
        {
            // Concatenate all individual hashes
            string concatenatedHashes = string.Join("", hashes);

            // Calculate the hash of the concatenated string
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(concatenatedHashes));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://github.com/Nuubkid";

            // Open the URL in the default web browser
            System.Diagnostics.Process.Start(url);
        }

        private void button6_Click(object sender, EventArgs e)
        {

            if (listViewFiles.Items.Count > 0)
            {
                listViewFiles.Items.Clear();
            }
            else
            {
                MessageBox.Show("No List to be clear");
            }
        }
    }
}
