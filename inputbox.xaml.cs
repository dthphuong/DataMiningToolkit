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

namespace DataMiningToolkit
{
    /// <summary>
    /// Interaction logic for inputbox.xaml
    /// </summary>
    public partial class inputbox : Window
    {
        public inputbox()
        {
            InitializeComponent();
            txtInp.Focus();
        }

        public int GetDemensional()
        {
            if (txtInp.Text != "")
            {
                try
                {
                    return int.Parse(txtInp.Text);
                }
                catch
                {
                    MessageBox.Show("Second Demensional must be Integer number. \nPlease check and input again", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return -1;
                }
            }
            else
                return -1;
            
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtInp.Text = "-1";
            this.Hide();
        }
    }
}
