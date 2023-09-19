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
        struct FileManager {
            public string fileName;
            public DateTime time;

            public FileManager(string name, DateTime time) { 
                this.fileName = name;
                this.time = time;
            }
        }

        List<FileManager> _fileManager = new List<FileManager>();

        public MainWindow()
        {
            InitializeComponent();
            using (var sw = new StreamWriter("D:\\Test.txt"))
            { 
                sw.WriteLine("");
                sw.Close();
            }
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                ComboBoxDisk.Items.Add(drive.Name +" Всего: "+ ConvertBiteToGb(drive.TotalSize) +"Gb Свободно: "+ ConvertBiteToGb(drive.TotalFreeSpace) +"Gb");
            }
        }

        void AddFileInList(FileManager fileManager) 
        { 
            this._fileManager.Add(fileManager);
            CheckFileList();
        }

        void CheckFileList() 
        { 
            foreach (FileManager file in this._fileManager.ToArray())
            {
                DateTime currentTime = DateTime.Now;

                if ((currentTime - file.time).Seconds > 10 || (currentTime - file.time).Seconds < 0) 
                { 
                    this._fileManager.Remove(file);
                }
            }
        }

        void SaveOpennedFileList()
        {
            CheckFileList();
            using (var sw = new StreamWriter("D:\\Test.txt"))
            {
                foreach (var file in this._fileManager)
                {
                    sw.WriteLine(file.fileName + "\t" + file.time.ToString());
                }
                sw.Close();
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
                string[] directory = Directory.GetDirectories(diskName);
                foreach (string dir in directory)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    ListBoxDirectory.Items.Add(dirInfo.FullName + "|| " + dirInfo.LastWriteTime + " || "+ dirInfo.Root);
                }
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
                    string path = $@"{selectedValue}";
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
            SaveOpennedFileList();
            Close();
        }

        private void ClosedWindow(object sender, EventArgs e)
        {
            SaveOpennedFileList();
        }
    }
}
