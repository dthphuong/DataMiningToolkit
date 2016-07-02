using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Win32;
using System.IO;
using DataMining;
using Ultilities;

namespace DataMiningToolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variable

        ObservableCollection<AttItems> myAttItems = new ObservableCollection<AttItems>();
        public ObservableCollection<AttItems> tblAttribute { get { return myAttItems; } } //Binding to lstAttribute

        ObservableCollection<AttInfo> myAttInfo = new ObservableCollection<AttInfo>();
        public ObservableCollection<AttInfo> tblStatistic { get { return myAttInfo; } } //Binding to lstStatistic

        Dataset dataset = new Dataset();
        ProMath math;
        Draw draw;
        string filename = "";
        public Canvas transCanvas;

        //Color defaultColor = Color.FromArgb(220, 0, 100, 150);
        Color ltColor = Color.FromArgb(220, 0, 0, 0);
        Color plotColor = Color.FromArgb(220, 0, 100, 150);
        SolidColorBrush solidColor;

        frmSetting st = new frmSetting();

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            Initialize();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            filename = "";
            string fileExt = "";

            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Choose your Dataset";
            openDialog.DefaultExt = ".libsvm";
            openDialog.Filter = "LibSVM data file (*.libsvm)|*.libsvm|CSV data file (*.csv)|*.csv";
            openDialog.ShowDialog();

            filename = openDialog.FileName;

            if (filename != "")
            {
                FileInfo fInfo = new FileInfo(filename);
                fileExt = fInfo.Extension;
                tbName.Text = fInfo.Name.Substring(0, fInfo.Name.IndexOf('.'));
                myAttItems.Clear();
                cbxD.Items.Clear();
                cbyD.Items.Clear();
                ShowStatus("Reading dataset . . . . .");

                double startTime = DateTime.Now.Millisecond;

                switch (fileExt)
                {
                    case ".libsvm":
                        {
                            dataset.dataType = DatasetType.InpTypes.LibSVM;
                            dataset = Dataset.Read(filename, dataset.dataType);
                            
                            for (int i = 0; i < dataset.MaxIndex-1; ++i)
                            {
                                myAttItems.Add(new AttItems { attNo = i + 1, attName = "att_" + (i + 1) });
                                cbxD.Items.Add("att_" + (i + 1));
                                cbyD.Items.Add("att_" + (i + 1));
                            }

                            //myAttItems.Add(new AttItems { attNo = dataset.MaxIndex+1, attName = "class" });

                            tbCount.Text = dataset.Count.ToString();
                            tbAttribute.Text =(dataset.MaxIndex+1).ToString();
                        }
                        break;
                    case ".csv":
                        {
                            dataset.dataType = DatasetType.InpTypes.CSV;
                            dataset = Dataset.Read(filename, dataset.dataType);

                            for (int i = 0; i < dataset.Header.Length; ++i)
                            {
                                myAttItems.Add(new AttItems { attNo = i + 1, attName = dataset.Header[i] });
                                cbxD.Items.Add(dataset.Header[i]);
                                cbyD.Items.Add(dataset.Header[i]);
                            }

                            tbCount.Text = dataset.Count.ToString();
                            tbAttribute.Text = (dataset.Header.Length).ToString();
                        }
                        break;
                }

                btnExport.IsEnabled = true;
                btnShow.IsEnabled = true;
                lstAttribute.SelectedIndex = 0;

                double endTime = DateTime.Now.Millisecond;

                math = new ProMath(dataset, 1);
                math.MakeDistTable();
                ShowStatus("Reading completed ! Reading time : " + (endTime-startTime).ToString() + " MilliSecond" );
            }

        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            string path = "";

            switch (dataset.dataType)
            {
                case DatasetType.InpTypes.LibSVM:
                    {
                        saveDialog.Title = "Export LibSVM to CSV";
                        saveDialog.Filter = "CSV data file (*.csv)|*.csv";
                        saveDialog.ShowDialog();
                        path = saveDialog.FileName;

                        dataset.Export(DatasetType.InpTypes.CSV, path);

                        MessageBox.Show("Export LibSVM to CSV completed", "Data Mining Tookit", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                case DatasetType.InpTypes.CSV:
                    {
                        MessageBox.Show("This feature is Coming soon !", "Data Mining Tookit", MessageBoxButton.OK, MessageBoxImage.Information);
                        //saveDialog.Title = "Export CSV to LibSVM";
                        //saveDialog.Filter = "LibSVM data file (*.libsvm)|*.libsvm";
                        //saveDialog.ShowDialog();
                        //path = saveDialog.FileName;

                        //dataset.Export(DatasetType.InpTypes.LibSVM, path);

                        //MessageBox.Show("Export CSV to LibSVM completed", "Data Mining Tookit", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
            }
        }

        #region myMethod

        /// <summary>
        /// Initialize when Form Load
        /// </summary>
        private void Initialize()
        {
            btnExport.IsEnabled = false;
            btnShow.IsEnabled = false;

            //INIT TextBlock
            tbName.Text = ""; tbCount.Text = ""; tbAttribute.Text = "";
            tbAtName.Text = "";  tbType.Text = "";

            //INIT Combo Plot
            cbPlot.Items.Add("Histogram");
            cbPlot.Items.Add("Box");
            cbPlot.Items.Add("Quantile");
            cbPlot.Items.Add("Quantile - Quantile");
            
            cbPlot.Text = "Histogram";

            ShowStatus("Welcome to Data Mining Toolkit !");
        }

        /// <summary>
        /// Show status of program on status bar
        /// </summary>
        /// <param name="status">your message</param>
        private void ShowStatus(string status)
        {
            tbStatus.Text = status;
        }

        #endregion

        private void lstAttribute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int d = lstAttribute.SelectedIndex;
            math = new ProMath(dataset, d+1);
            math.MakeDistTable();
            List<double> Q = math.Quartile();

            myAttInfo.Clear();
            if (d!=-1) tbAtName.Text = myAttItems[d].attName;
            if (dataset.dataType == DatasetType.InpTypes.LibSVM)
                tbType.Text = "Numberic";
            else
            {
                if (dataset.IsNumberic[d] == 1)
                    tbType.Text = "Numberic";
                else
                    tbType.Text = "Nominal";
            }

            myAttInfo.Add(new AttInfo { infoName = "Expect", infoValue = math.Expect().ToString()});
            myAttInfo.Add(new AttInfo { infoName = "Normal Variance", infoValue = math.NormalVariance().ToString() });
            myAttInfo.Add(new AttInfo { infoName = "Adjusted Variance", infoValue = math.AdjustedVariance().ToString() });
            myAttInfo.Add(new AttInfo { infoName = "Deviation", infoValue = math.Deviation().ToString() });
            myAttInfo.Add(new AttInfo { infoName = "Median", infoValue = math.Median().ToString() });
            for (int i = 0; i < math.Mod().Count; ++i ) myAttInfo.Add(new AttInfo { infoName = "Mod", infoValue = math.Mod()[i].ToString() + "  " });
            myAttInfo.Add(new AttInfo { infoName = "Min", infoValue = math.DistTable.Min(m => m.Value).ToString() });
            myAttInfo.Add(new AttInfo { infoName = "Max", infoValue = math.DistTable.Max(m=>m.Value).ToString() });
            myAttInfo.Add(new AttInfo { infoName = "Range", infoValue = math.Range().ToString() });
            myAttInfo.Add(new AttInfo { infoName = "Q1", infoValue = Q[0].ToString() });
            myAttInfo.Add(new AttInfo { infoName = "Q2", infoValue = Q[1].ToString() });
            myAttInfo.Add(new AttInfo { infoName = "Q3", infoValue = Q[2].ToString() });
            myAttInfo.Add(new AttInfo { infoName = "Interquartile range", infoValue = math.InterQuartile().ToString() });

            myCanvas.Children.Clear();
            draw = new Draw(myCanvas, math);
            plotColor = st.GetPlotColor();
            ltColor = st.GetLineTextColor();
            solidColor = new SolidColorBrush(ltColor);
            draw.color = solidColor;
            
            switch (cbPlot.SelectedItem.ToString())
            {
                case "Histogram":
                    {
                        draw.DrawAxis(Draw.PlotType.Histogram);
                        //draw.HistogramPlot(Color.FromArgb(220, 0, 100, 150));
                        draw.HistogramPlot(plotColor);
                    }
                    break;
                case "Box":
                    {
                        draw.DrawAxis(Draw.PlotType.Box);
                        //draw.BoxPlot(Color.FromArgb(220, 0, 100, 150));
                        draw.BoxPlot(plotColor);
                    }
                    break;
                case "Quantile":
                    {
                        draw.DrawAxis(Draw.PlotType.Quantile);
                        //draw.QuantilePlot(Color.FromArgb(220, 0, 100, 150));
                        draw.QuantilePlot(plotColor);
                    }
                    break;
                case "Quantile - Quantile":
                    {
                        inputbox inp = new inputbox();
                        inp.ShowDialog();
                        draw.xD = lstAttribute.SelectedIndex +1;
                        draw.yD = inp.GetDemensional();
                        inp.Close();

                        if (draw.yD != -1)
                        {
                            draw.DrawAxis(Draw.PlotType.Quantile_Quantile);
                            //draw.QQPlot(Color.FromArgb(220, 0, 100, 150));
                            draw.QQPlot(plotColor);
                        }
                    }
                    break;
            }
        }

        private void cbPlot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                myCanvas.Children.Clear();
                draw = new Draw(myCanvas, math);
                plotColor = st.GetPlotColor();
                ltColor = st.GetLineTextColor();
                solidColor = new SolidColorBrush(ltColor);
                draw.color = solidColor;

                switch (cbPlot.SelectedItem.ToString())
                {
                    case "Histogram":
                        {
                            draw.DrawAxis(Draw.PlotType.Histogram);
                            //draw.HistogramPlot(Color.FromArgb(220, 0, 100, 150));
                            draw.HistogramPlot(plotColor);
                        }
                        break;
                    case "Box":
                        {
                            draw.DrawAxis(Draw.PlotType.Box);
                            //draw.BoxPlot(Color.FromArgb(220, 0, 100, 150));
                            draw.BoxPlot(plotColor);
                        }
                        break;
                    case "Quantile":
                        {
                            draw.DrawAxis(Draw.PlotType.Quantile);
                            //draw.QuantilePlot(Color.FromArgb(220, 0, 100, 150));
                            draw.QuantilePlot(plotColor);
                        }
                        break;
                    case "Quantile - Quantile":
                        {
                            inputbox inp = new inputbox();
                            inp.ShowDialog();
                            draw.xD = lstAttribute.SelectedIndex + 1;
                            draw.yD = inp.GetDemensional();
                            inp.Close();

                            if (draw.yD != -1)
                            {
                                draw.DrawAxis(Draw.PlotType.Quantile_Quantile);
                                //draw.QQPlot(Color.FromArgb(220, 0, 100, 150));
                                draw.QQPlot(plotColor);
                            }
                        }
                        break;
                }
            }
            catch
            { }
        }

        private void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            plotScreen pScreen = new plotScreen(math, cbPlot.Text, draw.xD, draw.yD, ltColor, plotColor);
            pScreen.Show();
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            Draw myDraw = new Draw(vCanvas, math);

            vCanvas.Children.Clear();

            myDraw.xD = cbxD.SelectedIndex + 1; myDraw.yD = cbyD.SelectedIndex + 1;

            myDraw.DrawAxis(Draw.PlotType.Scatter);

            myDraw.ScatterPlot(Color.FromArgb(180, 250, 0, 0), Color.FromArgb(180, 0, 250, 0));

            ShowStatus("Show Visualize completed !");
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                st.ShowDialog();
            }
            catch
            {
                st = new frmSetting();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            st.Close();
        }

    }

    #region Binding Class

    public class AttItems
    {
        public int attNo { get; set; }
        public string attName { get; set; }
    }

    public class AttInfo
    {
        public string infoName { get; set; }
        public string infoValue { get; set; }
    }

    #endregion
}
