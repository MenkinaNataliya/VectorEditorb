using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFColorPickerLib;

namespace WPFColorPicker
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window
  {
    public Window1()
    {
      InitializeComponent();
    }

    /// <summary>
    /// DEMO CODE
    /// </summary>
    private void btnPickColor_Click(object sender, RoutedEventArgs e)
    {
      //ColorDialog colorDialog = new ColorDialog(((SolidColorBrush)this.RectColorPicked.Fill).Color);
      ColorDialog colorDialog = new ColorDialog();
      colorDialog.SelectedColor = ((SolidColorBrush)this.RectColorPicked.Fill).Color;
      colorDialog.Owner = this;
      if ((bool)colorDialog.ShowDialog())
      {
        RectColorPicked.Fill = new SolidColorBrush(colorDialog.SelectedColor);
      }
    }
  }
}
