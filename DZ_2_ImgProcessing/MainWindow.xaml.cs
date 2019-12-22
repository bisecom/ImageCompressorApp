using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DZ_2_ImgProcessing
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string getFilesDirPath { get; set; }
        public string setFilesDirPath { get; set; }
        public List<string> pathsList { get; set; }
        public List<string> pathsBandWList { get; set; }
        public List<ParallObj> myParallObj { get; set; }
        public int ImgQtyDecreasing { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            CBItemsAdding();
        }

        void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender; // checked RadioButton

            if (asynch.IsChecked == true && radioButton.Name == "asynch")
            {
                this.Resources["ExpanderEnable"] = true;
            }

            if (synch.IsChecked == true && radioButton.Name == "synch")
            {
                this.Resources["ExpanderEnable"] = false;
            }
        }



        private void GetButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        getFilesDirPath = dialog.SelectedPath;
                        string supportedExtensions = "*.jpg,*.gif,*.png,*.bmp,*.jpe,*.jpeg,*.ico,*.eps,*.tif";
                        pathsList = new List<string>();
                        foreach (string imageFile in Directory.GetFiles(getFilesDirPath, "*.*", SearchOption.AllDirectories).Where(s => supportedExtensions.Contains(System.IO.Path.GetExtension(s).ToLower())))
                        {
                            pathsList.Add(imageFile);
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Something goes wrong! Please, try another folder!"); return; }
            
        }

        private void SaveButtonClick(object sender, EventArgs e)
        {
            //get directory path
            setFilesDirPath = "";
            try
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        setFilesDirPath = dialog.SelectedPath;

                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Something goes wrong! Please, try another folder!"); return; }

            System.Drawing.Image img = System.Drawing.Image.FromFile(pathsList[0]);
        }

        private void LetsDoItButtonClick(object sender, EventArgs e)
        {
            myParallObj = new List<ParallObj>();
            bool processFulfilled = false;
            try
            {
                foreach (var imgPath in pathsList)
                {
                    ParallObj temp = new ParallObj();
                    temp.ImgPathObj = imgPath;
                    temp.ImgQualityObj = ImgQtyDecreasing;
                    temp.pathDestinationObj = setFilesDirPath;
                    myParallObj.Add(temp);
                }
            }
            catch (Exception ex)
            { MessageBox.Show("Please, put correct data!"); return; }

            //Black&White and Decreased quality, synch processing
            if (getFilesDirPath != "" && setFilesDirPath != "" && pathsList != null && ImgQtyDecreasing != 0 &&
                CheckBoxBlackWhite.IsChecked == true && synch.IsChecked == true && asynch.IsChecked == false)
            {
                var stopWatch = Stopwatch.StartNew();
                foreach (var imageData in myParallObj)
                {
                    Operating.SaveImgAndGray(imageData);
                }
                MessageBox.Show("Processing time is " + stopWatch.Elapsed.TotalSeconds.ToString() + " sec."); processFulfilled = true;
            }
            //Decreased quality, synch processing
            if (getFilesDirPath != "" && setFilesDirPath != "" && pathsList != null && ImgQtyDecreasing != 0 &&
                CheckBoxBlackWhite.IsChecked == false && synch.IsChecked == true && asynch.IsChecked == false)
            {
                var stopWatch = Stopwatch.StartNew();
                foreach (var imageData in myParallObj)
                {
                    Operating.SaveImg(imageData);
                }
                MessageBox.Show("Processing time is " + stopWatch.Elapsed.TotalSeconds.ToString() + " sec."); processFulfilled = true;
            }

            //Decreased quality, asynch processing, ThreadPool
            if (getFilesDirPath != "" && setFilesDirPath != "" && pathsList != null && ImgQtyDecreasing != 0 &&
                CheckBoxBlackWhite.IsChecked == false && synch.IsChecked == false && asynch.IsChecked == true &&
                ThreadPoolBtn.IsChecked == true)
            {
                var stopWatch = Stopwatch.StartNew();
                foreach (var element in myParallObj)
                {
                    ThreadPool.QueueUserWorkItem(state=>Operating.SaveImg(element));
                }
                MessageBox.Show("Processing time is " + stopWatch.Elapsed.TotalSeconds.ToString() + " sec."); processFulfilled = true;
            }

            //Decreased quality, asynch processing, Parallel
            if (getFilesDirPath != "" && setFilesDirPath != "" && pathsList != null && ImgQtyDecreasing != 0 &&
                CheckBoxBlackWhite.IsChecked == false && synch.IsChecked == false && asynch.IsChecked == true &&
                ParallelBtn.IsChecked == true)
            {
                var stopWatch = Stopwatch.StartNew();
                ParallelLoopResult result = Parallel.ForEach<ParallObj>(myParallObj, Operating.SaveImg);
                MessageBox.Show("Processing time is " + stopWatch.Elapsed.TotalSeconds.ToString() + " sec."); processFulfilled = true;
            }

            //Black&White and Decreased quality, asynch processing, ThreadPool
            if (getFilesDirPath != "" && setFilesDirPath != "" && pathsList != null && ImgQtyDecreasing != 0 &&
                CheckBoxBlackWhite.IsChecked == true && asynch.IsChecked == true && ThreadPoolBtn.IsChecked == true)
            {
                var stopWatch = Stopwatch.StartNew();
                foreach (var element in myParallObj)
                {
                    ThreadPool.QueueUserWorkItem(state => Operating.SaveImgAndGray(element));
                }
                MessageBox.Show("Processing time is " + stopWatch.Elapsed.TotalSeconds.ToString() + " sec."); processFulfilled = true;
            }
            //Black&White and Decreased quality, asynch processing, Parallel
            if (getFilesDirPath != "" && setFilesDirPath != "" && pathsList != null && ImgQtyDecreasing != 0 &&
                CheckBoxBlackWhite.IsChecked == true && asynch.IsChecked == true && ParallelBtn.IsChecked == true)
            {
                var stopWatch = Stopwatch.StartNew();
                ParallelLoopResult resultBandW = Parallel.ForEach<ParallObj>(myParallObj, Operating.SaveImgAndGray);
                MessageBox.Show("Processing time is " + stopWatch.Elapsed.TotalSeconds.ToString() + " sec."); processFulfilled = true;
            }
            if (processFulfilled == false)
            { MessageBox.Show("Please, put correct data! \nChoose all options again!"); }

            setFilesDirPath = ""; getFilesDirPath = "";
            pathsList = null; myParallObj = null; 
    }

        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ImgQtyDecreasing = Convert.ToInt32((string)comboBox.SelectedItem);
        }

        private void CBItemsAdding()
        {
            for (int i = 1; i < 100; i++)
            {
                percentsList.Items.Add(i.ToString());
            }
        }

    }
}
