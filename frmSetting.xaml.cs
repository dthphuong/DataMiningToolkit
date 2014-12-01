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
using System.Windows.Shapes;

namespace DataMiningToolkit
{
    /// <summary>
    /// Interaction logic for frmSetting.xaml
    /// </summary>
    public partial class frmSetting : Window
    {

        public frmSetting()
        {
            InitializeComponent();
            cbLineText.SelectedIndex=0;
            cbHistogram.SelectedIndex = 0;
        }

        public Color GetLineTextColor()
        {
            Color color = new Color() ; 

            switch (cbLineText.Text)
            {
                case "Black":
                    color= Color.FromArgb(200, 0, 0, 0);
                    break;
                case "Red":
                    color= Color.FromArgb(200, 250, 0, 0);
                    break;
                case "Green":
                    color= Color.FromArgb(200, 0, 250, 0);
                    break;
                case "Blue":
                    color= Color.FromArgb(200, 0, 0, 250);
                    break;
                case "Violet":
                    color = Color.FromArgb(200, 64, 0, 128);
                    break;
                case "Pink":
                    color = Color.FromArgb(200, 250, 0, 250);
                    break;
            }

            return color;
        }

        public Color GetPlotColor()
        {
            Color color = new Color();

            switch (cbHistogram.Text)
            {
                case "Black":
                    color = Color.FromArgb(200, 0, 0, 0);
                    break;
                case "Red":
                    color = Color.FromArgb(200, 250, 0, 0);
                    break;
                case "Green":
                    color = Color.FromArgb(200, 0, 250, 0);
                    break;
                case "Blue":
                    color = Color.FromArgb(200, 0, 0, 250);
                    break;
                case "Violet":
                    color = Color.FromArgb(200, 64, 0, 128);
                    break;
                case "Pink":
                    color = Color.FromArgb(200, 250, 0, 250);
                    break;
            }

            return color;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
