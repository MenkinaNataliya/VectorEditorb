using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFColorPickerLib;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace WpfApplication1
{

    public partial class MainWindow
    {
        private readonly Point[] _clickBezier =new Point[3];
        private int _countClickBezier;
        private int _countPicture;
        
        private bool _isCurvedLine ;
        private bool _isStraightLine ;
        private bool _isCircle ;
        private bool _isRectangle;
        private bool _isMove;
        private bool _isRemove;
        private bool _isResize ;
        private bool _isBezierCurve ;
        private Point _lineStart;
        private Point _lineEnd;
        private SolidColorBrush _brush;
        private Color _color;

        private Shape _selectedShape;
        private readonly Draw _draw;


        public MainWindow()
        {
            InitializeComponent();
            
            _color = Colors.Black;
            _brush = new SolidColorBrush(_color);
            RectColorPicked.Fill = _brush;
            _brush = new SolidColorBrush(_color);
            _selectedShape = null;
            _color = ((SolidColorBrush)RectColorPicked.Fill).Color;
            tbThick.Text = slider.Value.ToString(CultureInfo.InvariantCulture);
            _draw= new Draw(FirstCanvas);


        }

        private void MyCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _lineStart = e.GetPosition(FirstCanvas);
            if(rbCurvedLine.IsChecked != null && (bool) rbCurvedLine.IsChecked) _isCurvedLine = true;
            else if (_isMove || _isResize)
            {
                  _selectedShape = _draw.CheckFigure(_lineStart);
            }
            else if (_isRemove)
            {
                var select = _draw.CheckFigure(_lineStart);
                FirstCanvas.Children.Remove(select);

                
            }
            else if (_isBezierCurve)
            {
                _clickBezier[_countClickBezier] = e.GetPosition(FirstCanvas);
                if (_countClickBezier ==2)
                {
                    _draw.Beze(_clickBezier, _brush, slider.Value);
                    _countClickBezier = 0;
                }
                else
                {
                    _countClickBezier++;
                }
            }
        }



        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point lineEnd = e.GetPosition(FirstCanvas);
            if (_isCurvedLine)
            {
               
                _draw.Line(_brush,_lineStart, lineEnd, slider.Value);
                _lineStart = lineEnd;
            }
            if (_isMove)
            {
                if (_selectedShape != null)
                    _selectedShape.Margin = new Thickness(lineEnd.X, lineEnd.Y, 0, 0);
            }
            if (_isResize)
            {
                if (_selectedShape != null)
                {
                    if (_selectedShape.GetType() == new Line().GetType())
                    {
                        ((Line) _selectedShape).X2 = lineEnd.X;
                        ((Line) _selectedShape).Y2 = lineEnd.Y;
                    }
                    else
                    {
                        _selectedShape.Width = Math.Abs(_lineEnd.X - _selectedShape.Margin.Left);
                        _selectedShape.Height = Math.Abs(_lineEnd.Y - _selectedShape.Margin.Top);
                    }
                    _selectedShape = null;
                }
            }
        }
        private void MyCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isCurvedLine = false;
            _lineEnd = e.GetPosition(FirstCanvas);
            if (_isStraightLine)
            {
                _draw.Line(_brush,_lineStart, _lineEnd, slider.Value);
                         
            }
            else if (_isCircle)
            {
                _draw.Circle(_brush, _lineStart,_lineEnd, slider.Value);
            }
            else if (_isRectangle)
            {
               _draw.Rectangle(_brush, _lineStart, _lineEnd, slider.Value);
            }
            else if(_isMove || _isResize)
                _selectedShape = null;

            _lineStart = _lineEnd;

        }



        private void slider_MouseMove(object sender, MouseEventArgs e)
        {
            tbThick.Text = slider.Value.ToString(CultureInfo.CurrentCulture);
        }

        private void btColor_Click(object sender, RoutedEventArgs e)
        {
            var colorDialog = new ColorDialog
            {
                SelectedColor = ((SolidColorBrush)RectColorPicked.Fill).Color,
                Owner = this
            };
            RectColorPicked.Fill = new SolidColorBrush(colorDialog.SelectedColor);

            var showDialog = colorDialog.ShowDialog();
            if (showDialog != null && (bool)showDialog)
            {
                RectColorPicked.Fill = new SolidColorBrush(colorDialog.SelectedColor);
            }

            _brush = new SolidColorBrush(_color);

            _color = ((SolidColorBrush)RectColorPicked.Fill).Color;
        }




       

        private void btSave_Click(object sender, RoutedEventArgs e)
        {

            using (FileStream fs = File.Create("D:\\picture"+ _countPicture + ".svg"))
            {
                var str = _draw.GetInformationFigures();
               
                
                _countPicture++;
                Byte[] info = new UTF8Encoding(true).GetBytes(str);
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

        }

        


        private void rbMove_Checked(object sender, RoutedEventArgs e)
        {
            SetAllFlags();
            _isMove = true;

        }

        private void rbCurvedLine_Checked(object sender, RoutedEventArgs e)
        {
            SetAllFlags();
            _isCurvedLine = true;
            
        }

        private void rbStraightLine_Checked(object sender, RoutedEventArgs e)
        {
            SetAllFlags();
            _isStraightLine = true;
        }

        private void rbCircle_Checked(object sender, RoutedEventArgs e)
        {
            SetAllFlags();
            _isCircle = true;
        }

        private void rbRectangle_Checked(object sender, RoutedEventArgs e)
        {
            SetAllFlags();
            _isRectangle = true;
        }

        private void btRemove_Click(object sender, RoutedEventArgs e)
        {
            FirstCanvas.Children.Clear();
            _draw.RemoveAll();
        }

        private void rbRemoveobject_Checked(object sender, RoutedEventArgs e)
        {
            SetAllFlags();
            _isRemove = true;
        }

        private void rbResize_Checked(object sender, RoutedEventArgs e)
        {
            SetAllFlags();
            _isResize = true;
        }

        private void SetAllFlags()
        {


             _isCurvedLine = false;
            _isStraightLine = false;
            _isCircle = false;
            _isRectangle = false;
            _isMove = false;
            _isRemove = false;
            _isResize = false;
            _isBezierCurve = false;
        }

        private void rbBezierCurve_Checked(object sender, RoutedEventArgs e)
        {
            SetAllFlags();
            _isBezierCurve = true;
        }
    }
}
