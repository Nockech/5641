using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace Cmd_run
{   
    public partial class MainWindow : Window
    {
        int Encode = 866;
        /*
        Change "Encode" variable to your cmd language
        Encoding list to different languages: 

        437 (US)
        720 (Arabic)
        737 (Greek)
        775 (Baltic)
        850 (Multilingual Latin I)
        852 (Latin II)
        855 (Cyrillic)
        857 (Turkish)
        858 (Multilingual Latin I + Euro)
        862 (Hebrew)
        866 (Russian)
        1254 (Turkish)
        */
        public Process CmdProcess = new Process();
        public string SelfPath = Environment.CurrentDirectory;
        private string CurrentFileDir = "";
        private string CurrentOption;
        private string CurrentFileName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void btnX_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Environment.Exit(0);
        }
        private void btn__Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void EntryTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && e.Key == Key.Enter)
                ManageProcesses(); 
        }

        //Buttons assigment
        private void btnErase_Click(object sender, RoutedEventArgs e)
        {
            EntryTextBox.Text = "";
        }
        private void btnSTART_Click(object sender, RoutedEventArgs e)
        {
            ManageProcesses();
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ConsoleOutTextBox.Text = "";
        }
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            ToggleWindow();
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            CallCreateFileWindow();
        }

        //Cmd Process Init
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo cmdStartInfo = new ProcessStartInfo("cmd");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            cmdStartInfo.WorkingDirectory = SelfPath;
            cmdStartInfo.StandardOutputEncoding = Encoding.GetEncoding(Encode);

            cmdStartInfo.UseShellExecute = false;
            cmdStartInfo.CreateNoWindow = true;
            cmdStartInfo.RedirectStandardInput = true;
            cmdStartInfo.RedirectStandardError = true;
            cmdStartInfo.RedirectStandardOutput = true;

            CmdProcess.StartInfo = cmdStartInfo;
            CmdProcess.Start();

            CmdProcess.OutputDataReceived += cmdProcess_OutputDataReceived;
            CmdProcess.ErrorDataReceived += cmdProcess_ErrorDataReceived;

            CmdProcess.BeginOutputReadLine();
            CmdProcess.BeginErrorReadLine();
        }

        //Combobox change option event
        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            ConsoleOutTextBox.Text = "";
            switch (StartOptsBox.SelectedIndex)
            {
                case 0:
                    { CurrentOption = "python"; break; }
                case 1:
                    { CurrentOption = "cmd"; break; }
                case 2:
                    { CurrentOption = "settings"; startOptsInit(); break; }
            }
        }

        //Option chosing
        private void ManageProcesses()
        {
            switch (CurrentOption)
            {         //Main commands
                case "python":
                    {
                        StartPyCommand();
                        break;
                    }
                case "cmd":
                    {
                        StartCmdCommand();
                        break;
                    }
                case "settings":
                    {
                        StartOptCommand();
                        break;
                    } //Setting commands
                case "settings.setdir":
                    {
                        setDirCommand();
                        break;
                    }
            }
        }

        //Find file
        private void fullScan(string selfDir, string fileName)
        {
            try
            {
                foreach (string file in Directory.GetFiles(SelfPath, fileName))
                {
                    CurrentFileDir = Path.GetDirectoryName(file);
                }
            }
            catch { }
            scanForFiles(selfDir, fileName);
        }
        private void scanForFiles(string selfDir, string fileName)
        {
            foreach (string dir in Directory.GetDirectories(selfDir))
            {
                try
                {
                    foreach (string file in Directory.GetFiles(dir, fileName))
                    {
                        CurrentFileDir = Path.GetDirectoryName(file);
                    }
                    scanForFiles(dir, fileName);
                }
                catch (Exception Error)
                {
                    ConsoleOutTextBox.Text += Error.Message;
                }
            }
        }

        //Start py file function
        private void StartPyCommand()
        {
            CurrentFileName = EntryTextBox.Text;

            fullScan(SelfPath, CurrentFileName);
            CmdProcess.StandardInput.WriteLine($"python \"{$@"{CurrentFileDir}\{CurrentFileName}"}\"");

            CurrentFileDir = "";
        }

        //Start cmd command function
        private void StartCmdCommand()
        {
            CmdProcess.StandardInput.WriteLine(EntryTextBox.Text);
        }

        //Cmd process output/error recreived
        private void cmdProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.Data.StartsWith(@"C:\"))
                    ConsoleOutTextBox.Text += $">>> [{EntryTextBox.Text}]\n\r";
                else
                    ConsoleOutTextBox.Text += e.Data + "\n\r";
                ConsoleScrollbarDiv.ScrollToEnd();
            });
        }
        private void cmdProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.Data.StartsWith(@"C:\"))
                    ConsoleOutTextBox.Text += $">>> [{EntryTextBox.Text}]\n\r";
                else
                    ConsoleOutTextBox.Text += e.Data + "\n\r";
                ConsoleScrollbarDiv.ScrollToEnd();
            });
        }

        //Settings mode functions
        private void startOptsInit()
        {
            ConsoleOutTextBox.Text = "Settings mode.\nType \"help\" for more information\n\n";
        }
        private void StartOptCommand()
        {
            switch (EntryTextBox.Text)
            {
                case "help":
                    ConsoleOutTextBox.Text +=
                        "[setdir]    => set working directory\n" +
                        "[getdir]    => current working directory\n" +
                        "[setdelay]  => set start delay\n" +
                        "[nodelay]   => clear delay\n" +
                        "[setrepeat] => set start cycle delay\n" +
                        "[norepeat]  => clear cycle delay\n\n";
                    break;
                case "setdir":
                    ConsoleOutTextBox.Text += $">>>Enter new working directory\n\n";
                    CurrentOption = "settings.setdir";
                    break;
                case "getdir":
                    ConsoleOutTextBox.Text += $"[getdir] >>> {SelfPath}\n\n";
                    break;
                default:
                    ConsoleOutTextBox.Text += "Unknown command.Type \"help\" to get command list\n\n";
                    break;
            }
        }
        private void setDirCommand()
        {
            if (Directory.Exists(EntryTextBox.Text))
            {
                SelfPath = EntryTextBox.Text;
                ConsoleOutTextBox.Text += $"Succesfully changed directory to:\n{SelfPath}\n\n";
            }
            else
            {
                ConsoleOutTextBox.Text += "Unable to change directory: Current directory does not exist\n\n";
            }
            CurrentOption = "settings";
        }

        //Toggle Window function
        private void ToggleWindow()
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog
            {
                Title = "Select working directory:",
                IsFolderPicker = true,
                InitialDirectory = SelfPath,

                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                DefaultDirectory = SelfPath,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SelfPath = dlg.FileName;
                ConsoleOutTextBox.Text += ">>> Changed directory to: \"" + SelfPath + "\"\n";
            }

        }

        //Create new file window calling
        private void CallCreateFileWindow()
        {
            CreateFileWindow createFileWindow = new CreateFileWindow(this);
            createFileWindow.ViewModel = "ViewModel";
            createFileWindow.Show();
        }

    }
}