using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFColorPickerLib;

namespace WpfApplication1
{

    public partial class Paint 
    {
        private readonly Point[] _clickBezier = new Point[3];
        private int _countClickBezier;
        private int _countPicture;
        private bool _isAllotment;
        private bool _isCursor;
        private bool _isCurvedLine;
        private bool _isStraightLine;
        private bool _isCircle;
        private bool _isRectangle;
        private bool _isRemove;
        private bool _isResize;
        private bool _isBezierCurve;
        private Point _lineStart;
        private Point _lineEnd;
        private SolidColorBrush _brush;
        private Color _color;
        private Shape _dashedFigure;
        private Shape _leftTop;
        private Shape _rightBottom;
        private Shape _rightTop;
        private Shape _leftBottom;
        private Shape _selectedShape;
        private readonly Draw _draw;


        public Paint()
        {
            InitializeComponent();
            method.Content = "Указатель";
            _color = Colors.Black;
            _brush = new SolidColorBrush(_color);
            RectColorPicked.Fill = _brush;
            _brush = new SolidColorBrush(_color);
            _selectedShape = null;
            _color = ((SolidColorBrush)RectColorPicked.Fill).Color;
            tbThick.Text = slider.Value.ToString(CultureInfo.InvariantCulture);
            _draw = new Draw(Canvas);
            _isCursor = true;
        }


        private void FirstCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point lineEnd = e.GetPosition(Canvas);
           
            if (_isCursor)
            {
                if (_selectedShape != null)
                {
                    if (_isAllotment)
                    {
                        _selectedShape.Margin = new Thickness(_lineEnd.X, _lineEnd.Y, 0, 0);
                        _dashedFigure.Margin = new Thickness(_lineEnd.X - 10, _lineEnd.Y - 10, 0, 0);
                        _leftTop.Margin = new Thickness(_lineEnd.X - 12, _lineEnd.Y - 12, 0, 0);
                        _leftBottom.Margin = new Thickness(_lineEnd.X - 12, _dashedFigure.Margin.Top + _dashedFigure.Height - 2, 0, 0);
                        _rightTop.Margin = new Thickness(_dashedFigure.Margin.Left + _dashedFigure.Width - 2, _lineEnd.Y - 12, 0, 0);
                        _rightBottom.Margin = new Thickness(_dashedFigure.Margin.Left + _dashedFigure.Width - 2, _dashedFigure.Margin.Top + _dashedFigure.Height - 2, 0, 0);
                    }
                }
            }else
            if (_isCurvedLine)
            {
                _draw.Line(_brush, _lineStart, lineEnd, slider.Value);
                _lineStart = lineEnd;
            }
        }


        private void FirstCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isCurvedLine = false;
            _lineEnd = e.GetPosition(Canvas);

            if (_isStraightLine)
            {
                _draw.Line(_brush, _lineStart, _lineEnd, slider.Value);
            }
            else if (_isCircle)
            {
                _draw.Circle(_brush, _lineStart, _lineEnd, slider.Value);
            }
            else if (_isRectangle)
            {
                _draw.Rectangle(_brush, _lineStart, _lineEnd, slider.Value);
            }
            else if (_isResize)
            {
                if (_selectedShape != null)
                {
                    if (_selectedShape.GetType() == new Line().GetType())
                    {
                        ((Line)_selectedShape).X2 = _lineEnd.X;
                        ((Line)_selectedShape).Y2 = _lineEnd.Y;
                    }
                    else
                    {
                        _selectedShape.Width = Math.Abs(_lineEnd.X - _selectedShape.Margin.Left);
                        _selectedShape.Height = Math.Abs(_lineEnd.Y - _selectedShape.Margin.Top);

                    }

                    DeleteAdditionalShapes();
                }
                _isResize = false;
                _isAllotment = false;
            }


            _lineStart = _lineEnd;
        }

        private void DeleteAdditionalShapes()
        {
            Canvas.Children.Remove(_dashedFigure);
            Canvas.Children.Remove(_leftBottom);
            Canvas.Children.Remove(_leftTop);
            Canvas.Children.Remove(_rightBottom);
            Canvas.Children.Remove(_rightTop);
            _dashedFigure = null;
            _rightTop = null;
            _rightBottom = null;
            _leftTop = null;
            _leftBottom = null;
            _selectedShape = null;
        }



        private void FirstCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _lineStart = e.GetPosition(Canvas);
            if ( method.Content == $"Кривая линия") _isCurvedLine = true; 

            if ( _isCursor)
            {              
                if (_selectedShape != null)
                {
                    if (_isAllotment)
                    {
                        if (_lineStart.X >= (_rightBottom.Margin.Left - 2) &&
                            _lineStart.X <= (_rightBottom.Margin.Left + _rightBottom.Width + 2)
                            && _lineStart.Y >= (_rightBottom.Margin.Top - 2) &&
                            _lineStart.Y <= (_rightBottom.Margin.Top + _rightBottom.Height + 2)) _isResize = true;
                    }
                    else
                    {
                        _isAllotment = true;
                        if (_dashedFigure != null)
                        {
                            Canvas.Children.Remove(_dashedFigure);
                            _dashedFigure = null;
                        }

                        _dashedFigure = TMP(_selectedShape.Margin.Left - 10, _selectedShape.Margin.Top - 10);

                        _dashedFigure.StrokeThickness = 1;
                        _dashedFigure.Fill = null;
                        _dashedFigure.StrokeDashArray = new DoubleCollection() { 2 };
                        _dashedFigure.Width = Math.Abs(_selectedShape.Width + 20);
                        _dashedFigure.Height = Math.Abs(_selectedShape.Height + 20);
                        Canvas.Children.Add(_dashedFigure);

                        _leftTop = TMP(_dashedFigure.Margin.Left - 2, _dashedFigure.Margin.Top - 2);
                        Canvas.Children.Add(_leftTop);


                        _leftBottom = TMP(_dashedFigure.Margin.Left - 2, _dashedFigure.Margin.Top + _dashedFigure.Height - 2);
                        Canvas.Children.Add(_leftBottom);

                        _rightTop = TMP(_dashedFigure.Margin.Left + _dashedFigure.Width - 2, _dashedFigure.Margin.Top - 2);
                        Canvas.Children.Add(_rightTop);


                        _rightBottom = TMP(_dashedFigure.Margin.Left + _dashedFigure.Width - 2,
                            _dashedFigure.Margin.Top + _dashedFigure.Height - 2);
                        Canvas.Children.Add(_rightBottom);
                    }
                    
                }else _selectedShape = _draw.CheckFigure(_lineStart);
            }
            else if (_isRemove)
            {
                var select = _draw.CheckFigure(_lineStart);
                Canvas.Children.Remove(select);
                _draw.RemoveFigure(select);

            }
            else if (_isBezierCurve)
            {
                _clickBezier[_countClickBezier] = e.GetPosition(Canvas);
                if (_countClickBezier == 2)
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

        private Rectangle TMP( double left, double top)
        {
            return new Rectangle
            {
                Fill = new SolidColorBrush(Colors.Green),
                Stroke = new SolidColorBrush(Colors.Green),
                Width = Math.Abs(5),
                Height = Math.Abs(5),
                Margin = new Thickness(left,top, 0, 0)
            };
        }
        private void SetAllFlags()
        {

            _isCursor = false;
            _isCurvedLine = false;
            _isStraightLine = false;
            _isCircle = false;
            _isRectangle = false;
            
            _isRemove = false;
            _isResize = false;
            _isBezierCurve = false;
            _isAllotment = false;
        }

        private void bt_Click(object sender, RoutedEventArgs e)
        {
            if (_isAllotment)
            {
               DeleteAdditionalShapes();
            }
            SetAllFlags();
            var buttonName =((Button)e.Source).Name;
            switch (buttonName)
            {
                case "btSimpleLine":
                {
                    method.Content = "Кривая линия";
                    break;
                }
                case "btStraightLine":
                {
                        method.Content = "Прямая линия";
                        _isStraightLine = true; break;
                }
                case "btBezierCurve":
                {
                        method.Content = "Линия Безье (по 3 точкам)";
                        _isBezierCurve = true; break;
                }
                case "btCircle":
                {
                        method.Content = "Круг";
                        _isCircle = true; break;
                } 
                case "btRectangle":
                {
                        method.Content = "Прямоугольник";
                        _isRectangle = true; break;
                }
                case "btCursor":
                {
                        method.Content = "Указатель";
                        _isCursor = true; break;
                }
                case "btDelete":
                {
                        method.Content = "Удаление";
                        _isRemove = true; break;
                }
            }
        }
        private void btRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
            _draw.RemoveAll();
        }

        private void btSaveAll_Click(object sender, RoutedEventArgs e)
        {
            using (FileStream fs = File.Create("D:\\picture" + _countPicture + ".svg"))
            {
                var str = _draw.GetInformationFigures();
                _countPicture++;
                Byte[] info = new UTF8Encoding(true).GetBytes(str);
                fs.Write(info, 0, info.Length);
            }
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

            _color = ((SolidColorBrush)RectColorPicked.Fill).Color;
            _brush = new SolidColorBrush(_color);
        }



        private void slider_MouseMove(object sender, MouseEventArgs e)
        {
            tbThick.Text = slider.Value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
