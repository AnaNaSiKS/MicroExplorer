using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.ComponentModel.Design;

namespace MicroExplorer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<FileManager> fileManager = new List<FileManager>();
        public MainWindow()
        {
            InitializeComponent();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                ComboBoxDisk.Items.Add(drive.Name +" FullSize: "+ ConvertBiteToGb(drive.TotalSize) +"Gb FreeSpace: "+ ConvertBiteToGb(drive.TotalFreeSpace) +"Gb");
            }
        }

        struct FileManager {
            public string fileName;
            public DateTime time;

            public FileManager(string name, DateTime time) { 
                this.fileName = name;
                this.time = time;
            }
        }

        void AddFileInList(FileManager fileManager) 
        { 
            this.fileManager.Add(fileManager);
            checkFileList();
        }

        void checkFileList() 
        { 
            foreach (FileManager file in this.fileManager.ToArray())
            {
                DateTime currentTime = DateTime.Now;

                if ((currentTime - file.time).Seconds > 10 || (currentTime - file.time).Seconds < 0) 
                { 
                    this.fileManager.Remove(file);
                }
            }
        }

        double ConvertBiteToGb(double size)
        {
            return Math.Round(size /1024 /1024 /1024 ,1);
        }


        private void ComboBoxDisk_Selected(object sender, SelectionChangedEventArgs e)
        {
            string diskName = $@"{ComboBoxDisk.SelectedItem.ToString()[0]}:\";
            ListBoxDirectory.Items.Clear();

            try
            {
                if (Directory.Exists(diskName))
                {
                    string[] directory = Directory.GetDirectories(diskName);
                    foreach (string dir in directory)
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(dir);
                        ListBoxDirectory.Items.Add(dirInfo.FullName + " || " + dirInfo.LastWriteTime + " || "+ dirInfo.Root);
                    }
                }
                else ListBoxDirectory.Items.Add("Диск не найден");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ListBoxFile_Selected(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = ListBoxDirectory.SelectedItem;
            ListBoxFile.Items.Clear();

            if (selectedValue != null)
            {
                try
                {
                    string path = selectedValue.ToString();
                    string[] files = Directory.GetFiles(path.Split('|')[0]);
                    foreach (string file in files)
                    {
                        ListBoxFile.Items.Add(file);
                    }
                }
                catch (UnauthorizedAccessException ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void ListBoxFileOpen_Selected(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = ListBoxFile.SelectedItem;

            if (selectedValue != null)
            {
                try
                {
                    string path = $@"{selectedValue}";//selectedValue.ToString().Replace(@"\", @"\\");
                    MessageBox.Show(path);
                    Process.Start(path);
                    AddFileInList(new FileManager(path, DateTime.Now));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при запуске файла: {ex.Message}");
                }
            }
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            checkFileList();
            foreach (FileManager file in this.fileManager)
            { 
                MessageBox.Show(file.fileName + file.time.ToString());
            }
        }
    }
}
