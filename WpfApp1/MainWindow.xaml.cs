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
using ClassLibrary1;
using System.Globalization;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    class DoubleStrConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                double val = (double)value;
                return $"{val:0.0}";
            }
            catch (Exception error)
            {
                MessageBox.Show($"Unexpected error: {error.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "DoubleStrConv: ERROR";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string val = value as string;
                return double.Parse(val);
            }
            catch (Exception error)
            {
                MessageBox.Show($"Unexpected error: {error.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "DoubleStrConv: ERROR";
            }
        }
    }

    public partial class MainWindow : Window
    {

        MeasuredData md = new();
        SplineParameters sp = new();
        public ViewData vd { get; set; } = new();
        public MainWindow()
        {
            InitializeComponent();

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }
        private void TextBox(object sender, TextCompositionEventArgs e)
        {
            vd.Changed = true;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            vd.Clear();
        }
        private void MeasuredData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !vd.Data.Md.SetErr() && !vd.Data.Sp.SetErr();
        }

        private void MeasuredData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            vd.Data.Md.Func = vd.SpfList.selectedFunc.func;
            vd.Changed = true;
            vd.MdSetGrid();
            vd.Chart.AddPlot(vd.Data.Md.Grid, vd.Data.Md.Measured, 2, "Points");
        }

        private void Splines_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !vd.Data.Md.SetErr() && !vd.Data.Sp.SetErr();
        }

        private void Splines_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            double a = 213123;
            double[] Int = new double[1];
            double[] r = vd.Splain(ref a, ref Int);

            double[] res = new double[vd.Data.Sp.N];
            for (int i = 0; i < res.Length; i++)
                res[i] = r[0 + 3 * i];
            double[] grid = new double[vd.Data.Sp.N];
            for (int i = 0; i < vd.Data.Sp.N; i++)
                grid[i] = vd.Data.Md.Start + i * (vd.Data.Md.End - vd.Data.Md.Start) / (vd.Data.Sp.N - 1);

            vd.Chart.AddPlot(grid, res, 1, "Splain1");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(vd.Data.Str[2]);
        }

        private void TextBox2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int val;
            if (!Int32.TryParse(e.Text, out val) && e.Text != ",")
            {
                e.Handled = true; // отклоняем ввод
            }
        }
        private void TextBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int val;
            if (!Int32.TryParse(e.Text, out val))
            {
                e.Handled = true; // отклоняем ввод
            }
        }
    }
    public static class Cmd
    {
        public static readonly RoutedUICommand MeasuredData = new
            (
                "MeasuredData",
                "MeasuredData",
                typeof(Cmd),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D1, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand Splines = new
        (
            "Splines",
            "Splines",
            typeof(Cmd),
            new InputGestureCollection()
            {
                    new KeyGesture(Key.D2, ModifierKeys.Control)
            }
        );
    }
}
