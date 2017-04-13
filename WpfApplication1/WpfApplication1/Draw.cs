using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace WpfApplication1
{
    public class Draw
    {
        private readonly Canvas _canvas;
        public  readonly List<Shape> Figures;
        private int _idFigures = 1;
        public Draw(Canvas canvas)
        {
            _canvas = canvas;
            Figures = new List<Shape>();
        }

        public Shape CheckFigure(Point position)
        {
            foreach (var item in Figures)
            {
                if (item.GetType() == new Line().GetType())
                {
                    var line = (Line)item;
                    if (line.IsMouseDirectlyOver)
                        return item;
                }
                if (position.X >= (item.Margin.Left - 2) && position.X <= (item.Margin.Left + item.Width + 2)
                    && position.Y >= (item.Margin.Top - 2) && position.Y <= (item.Margin.Top + item.Height + 2)) return item;
            }
            return null;
        }

        public string GetInformationFigures()
        {
            var str = "<?xml version=\"1.0\" encoding=\"UTF - 8\" standalone=\"no\"?>\n" +
                          "<svg " +
                          "width=\"" + _canvas.Width + "px\" " +
                          "height=\"" + _canvas.Height + "px\" " +
                          "viewBox=\"0 0 " + _canvas.Width + " " + _canvas.Height + "\" " +

                          "\n<g inkscape:label=\"Layer 1\" inkscape:groupmode=\"layer\" id=\"layer1\" > ";
            foreach (var item in Figures)
            {
                if (item.GetType() == new Line().GetType()) str += GetInfoLine((Line)item);
                else if (item.GetType() == new Ellipse().GetType()) str += GetInfoEllipce((Ellipse)item);
                else if (item.GetType() == new Rectangle().GetType()) str += GetInfoRectangle((Rectangle)item);
            }
            str += "</g>\n</svg>";
            return str;
        }

        private string GetInfoLine(Line line)
        {
            var str = "\n<path " +
                   "style=\"fill:none;" +
                   "stroke:" + line.Stroke +
                   ";stroke-width:" + line.StrokeThickness + "px;" +
                   "stroke-linecap:butt;stroke-linejoin:miter;stroke-opacity:1\"" +
                   " d=\"M 95.249999,107.25595 C 142.11904,63.410714 144.3869,62.654763 144.3869,62.654763 l 10.58333,-3.779764\"" +
                   " id=\"path" + _idFigures + "\"/>\n";
            _idFigures += 1;
            return str;
        }
        private string GetInfoEllipce(Ellipse ellipse)
        {

            var str = "\n<circle " +
                    " cx=\"" + ellipse.Margin.Left + "\"" +
                     " cy=\"" + ellipse.Margin.Top + "\"" +
                      " rx=\"" + ellipse.Width / 2 + "\"" +
                      " ry=\"" + ellipse.Height / 2 + "\"" +
                    " style=\"fill:" + ellipse.Fill + ";" +
                    " stroke-width:" + ellipse.StrokeThickness + "px;\"" +
                    " id=\"path" + _idFigures + "\"/>\n";
            _idFigures += 1;
            return str;
        }
        private string GetInfoRectangle(Rectangle rectangle)
        {
            var str = "\n<rect " +
                 "x=\"" + rectangle.Margin.Left + "\"" +
                 "y=\"" + rectangle.Margin.Top + "\"" +
                 "width=\"" + rectangle.Width + "\"" +
                 "height=\"" + rectangle.Height + "\"" +
                 "style=\"fill:" + rectangle.Fill + ";" +
                 ";stroke-width:" + rectangle.StrokeThickness + "px;" +
                 " id=\"path" + _idFigures + "\"/>\n";
            _idFigures += 1;
            return str;
        }

        public  void Beze(IReadOnlyList<Point> points, Brush brush, double thickness)
        {

            var path = new Path
            {
                Stroke = brush,
                StrokeThickness = thickness
            };
            var geometry = new PathGeometry();
            var figure = new PathFigure { StartPoint = points[0] };
            figure.Segments.Add(new QuadraticBezierSegment()
            {
                Point1 = points[1],
                Point2 = points[2]
            });
            geometry.Figures.Add(figure);
            path.Data = geometry;


            _canvas.Children.Add(path);

        }

        public void Rectangle(Brush brush,Point startPoint, Point endPoint, double thickness)
        {
            var figure = DefineShape(new Rectangle(), startPoint, endPoint, brush, thickness);
            _canvas.Children.Add(figure);
            Figures.Add(figure);
        }

        public void RemoveFigure(Shape select)
        {
            Figures.Remove(select);
        }

        public void RemoveAll()
        {
            Figures.Clear();
        }


        public void Circle(Brush brush, Point startPoint, Point endPoint, double thickness)
        {
            var figure = DefineShape(new Ellipse(), startPoint, endPoint,  brush, thickness);
            _canvas.Children.Add(figure);
            Figures.Add(figure);
        }

        public void Line(Brush brush, Point startPoint, Point endPoint, double thickness)
        {
            Line l = new Line
            {
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y,
                Stroke = brush,
                StrokeThickness = thickness
            };
            _canvas.Children.Add(l);
            Figures.Add(l);
        }

        private Shape DefineShape(Shape figure, Point startPoint, Point endPoint, Brush brush, double thickness)
        {
            
            figure.Fill = brush;
            figure.StrokeThickness = thickness;
            figure.Stroke = brush;

            // Set the width and height of the Ellipse.
            figure.Width = Math.Abs(endPoint.X - startPoint.X);
            figure.Height = Math.Abs(endPoint.Y - startPoint.Y);

            figure.Margin = new Thickness(startPoint.X, startPoint.Y, 0, 0);
            Figures.Add(figure);
            return figure;
        }
    }
}
