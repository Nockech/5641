using System;
using System.IO;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Controls;

namespace Cmd_run
{
    public partial class CreateFileWindow : Window
    {
        MainWindow _main;
        public string ViewModel { get; set; }
        string fullFilePath;

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void btnX_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public CreateFileWindow(MainWindow main)
        {
            InitializeComponent();
            _main = main;
        }

        //Init
        private void CreateFileWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FileDirTextBox.Text = _main.SelfPath;
            FileDirTextBox.ScrollToEnd();
        }

        //Create File
        private void btnCreateFile_Click(object sender, RoutedEventArgs e)
        {
            createFile();
        }
        private void createFile()
        {
            if (!(string.IsNullOrWhiteSpace(FileNameTextBox.Text) ||
                  string.IsNullOrWhiteSpace(FileExtTextBox.Text)))
            {
                if (Directory.Exists(FileDirTextBox.Text))
                {
                    fullFilePath = $@"{FileDirTextBox.Text}\{FileNameTextBox.Text}.{FileExtTextBox.Text}";
                    if (!File.Exists(fullFilePath))
                    {
                        using (FileStream fs = File.Create(fullFilePath))
                        {
                            _main.CmdProcess.StandardInput.WriteLine($"\"{fullFilePath}\"");
                        }
                        this.Close();
                    }
                    else { FileNameLabel.Content = "File already exists:"; }
                }
                else { DirPathLabel.Content = "Enter valid path:"; }
            }
            else { FileNameLabel.Content = "Enter valid name:"; }
        }

    }
}
