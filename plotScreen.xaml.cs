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
using System.Windows.Shapes;
using DataMining;
using Ultilities;

namespace DataMiningToolkit
{
    /// <summary>
    /// Interaction logic for plotScreen.xaml
    /// </summary>
    public partial class plotScreen : Window
    {
        Draw draw;
        SolidColorBrush solidColor;

        public plotScreen(ProMath math,  string plot, int xD, int yD,Color ltColor, Color plotColor)
        {
            InitializeComponent();
            
            pCanvas.Height = this.Height - 40;
            pCanvas.Width = this.Width -16;

            btnClose.Margin = new Thickness(pCanvas.Width - btnClose.Width, 0,0,0);

            pCanvas.Children.Clear();
            draw = new Draw(pCanvas, math);
            solidColor = new SolidColorBrush(ltColor);
            draw.color = solidColor;

            switch (plot)
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
                        draw.xD = xD;
                        draw.yD = yD;
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
