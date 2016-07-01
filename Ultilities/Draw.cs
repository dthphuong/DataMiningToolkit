using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using DataMining;

namespace Ultilities
{
    public class Draw
    {
        public enum PlotType
        {
            Histogram, 
            Box,
            Quantile,
            Quantile_Quantile,
            Scatter,
        }

        public ProMath math { get; set; }
        public Canvas myCanvas { get; set; }
        public List<DataItem> data {get;set;}
        //public Dataset dataset { get; set; }
        public int xD { get; set; }
        public int yD { get; set; }



        #region Adjust

        int fontSize = 12;
        public Brush color = Brushes.Blue;


        #endregion

        #region Method

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public Draw()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="myCanvas">your Canvas here</param>
        public Draw(Canvas myCanvas, ProMath math)
        {
            this.myCanvas = myCanvas;
            this.math = math;
            data = math.DistTable;
        }

        /// <summary>
        /// Draw new Line into myCanvas
        /// </summary>
        /// <param name="X1">x1</param>
        /// <param name="Y1">y1</param>
        /// <param name="X2"x2></param>
        /// <param name="Y2">y2</param>
        /// <param name="strokeThickness">thickness</param>
        /// <param name="color">color</param>
        public void Line(double X1, double Y1, double X2, double Y2, int strokeThickness )
        {
            Line l = new Line();
            
            l.X1 = X1;
            l.Y1 = Y1;
            l.X2 = X2;
            l.Y2 = Y2;
            l.StrokeThickness = strokeThickness;
            l.Stroke = color;

            myCanvas.Children.Add(l);
        }

        /// <summary>
        /// Draw new Text string into myCanvas
        /// </summary>
        /// <param name="X">X</param>
        /// <param name="Y">Y</param>
        /// <param name="text">text String</param>
        /// <param name="color">Color of text</param>
        public void Text(double X, double Y, string text)
        {
            TextBlock txt = new TextBlock();

            txt.FontSize = fontSize;
            txt.TextAlignment = System.Windows.TextAlignment.Center;
            txt.Text = text;
            txt.Margin = new System.Windows.Thickness(X - txt.Text.Length, Y, 0, 0);
            txt.Foreground = color;

            myCanvas.Children.Add(txt);
        }

        public void Rectangle(string toolTip, double X, double Y, double width, double height, int strokeThickness, Color color)
        {
            Rectangle r = new Rectangle();
            SolidColorBrush solidColor = new SolidColorBrush(color);

            r.Margin = new System.Windows.Thickness(X, Y, 0, 0);
            r.Width = width;
            r.Height = height;
            r.StrokeThickness = strokeThickness;
            r.Fill = solidColor;
            r.ToolTip = toolTip;

            myCanvas.Children.Add(r);
        }

        public void Point(double r, string toolTip, double X, double Y, Color color)
        {
            Ellipse e = new Ellipse();
            SolidColorBrush soliColor = new SolidColorBrush(color);

            e.Margin = new System.Windows.Thickness(X+r/2, Y-r/2, 0, 0);
            e.Width = e.Height = r;
            e.Fill = soliColor;
            e.ToolTip = toolTip;

            myCanvas.Children.Add(e);
        }

        /// <summary>
        /// Draw system axis 
        /// </summary>
        /// <param name="plotType">plot type to draw</param>
        public void DrawAxis(PlotType plotType)
        {
            switch (plotType)
            {
                #region Initialize Histogram Plot
                case PlotType.Histogram:
                    {
                        Line(15, myCanvas.Height - 25, myCanvas.Width - 15, myCanvas.Height - 25, 2); //Axis

                        Line(15, myCanvas.Height - 25, 15, myCanvas.Height - 15, 2); //Min
                        Text(12, myCanvas.Height - 15, Math.Round(data.Min(m => m.Value),3).ToString());

                        Line(myCanvas.Width - 15, myCanvas.Height - 25, myCanvas.Width - 15, myCanvas.Height - 15, 2); //Max
                        Text(myCanvas.Width - 20, myCanvas.Height - 15, Math.Round(data.Max(m => m.Value),3).ToString());

                        //Line((myCanvas.Width - 30) / 2, myCanvas.Height - 25, (myCanvas.Width - 30) / 2, myCanvas.Height - 15, 2); //Mid
                        //Text((myCanvas.Width - 30) / 2 - 5, myCanvas.Height - 15, Math.Round((double)(data.Max(m => m.Value) - data.Min(n => n.Value)) / 2,3).ToString());
                    }
                    break;
                #endregion

                #region Initialize Box Plot
                case PlotType.Box:
                    {
                        double min = math.DistTable.Min(m => m.Value);
                        double pixel = (myCanvas.Width - 30) /math.Range() ;
                        List<double> Q = math.Quartile();
                        
                        Line(15, myCanvas.Height - 25, myCanvas.Width - 15, myCanvas.Height - 25, 2); //Axis
                        
                        Line(15, myCanvas.Height - 25, 15, myCanvas.Height - 15, 2); //Min
                        Text(12, myCanvas.Height - 15, Math.Round(math.DistTable.Min(m => m.Value), 3).ToString());

                        Line(myCanvas.Width - 15, myCanvas.Height - 25, myCanvas.Width - 15, myCanvas.Height - 15, 2); //Max
                        Text(myCanvas.Width - 20, myCanvas.Height - 15, Math.Round(math.DistTable.Max(m => m.Value), 3).ToString());

                        Line(15 + pixel * (Q[0] - min), myCanvas.Height - 25, 15 + pixel *( Q[0] - min), myCanvas.Height - 15, 2); //Q1
                        Text(15 + pixel * (Q[0] - min)-5, myCanvas.Height - 15, Q[0].ToString());

                        Line(15 + pixel * (Q[1] - min), myCanvas.Height - 25, 15 + pixel * (Q[1] - min), myCanvas.Height - 15, 2); //Q2
                        Text(15 + pixel * (Q[1] - min)-5, myCanvas.Height - 15, Q[1].ToString());

                        Line(15 + pixel * (Q[2] - min), myCanvas.Height - 25, 15 + pixel * (Q[2] - min), myCanvas.Height - 15, 2); //Q3
                        Text(15 + pixel * (Q[2] - min)-5, myCanvas.Height - 15, Q[2].ToString());
                    }
                    break;
                #endregion

                #region Initialize Quantile Plot
                case PlotType.Quantile:
                    {
                        double q = (myCanvas.Width - 50) * 0.25;

                        Line(25, 25, 25, myCanvas.Height - 25, 2); //Axis y
                        Line(25, myCanvas.Height - 25, myCanvas.Width - 25, myCanvas.Height - 25, 2); //Axis x

                        Line(15, myCanvas.Height - 25, 25, myCanvas.Height - 25, 2); //Min Value
                        Text(5, myCanvas.Height - 35, data.Min(m => m.Value).ToString());
                        Line(15, 25, 25,25, 2); //Max Value
                        Text(5,15, data.Max(m => m.Value).ToString());

                        Line(25, myCanvas.Height - 25, 25, myCanvas.Height - 15, 2); //0.00
                        Text(22, myCanvas.Height - 15,"0.00");
                        Line(myCanvas.Width - 25, myCanvas.Height - 25, myCanvas.Width - 25, myCanvas.Height - 15, 2); //1.00
                        Text(myCanvas.Width - 30, myCanvas.Height - 15, "1.00");
                        Line(25+q, myCanvas.Height - 25, 25+q, myCanvas.Height - 15, 2); //0.25
                        Text(22+q, myCanvas.Height - 15, "0.25");
                        Line(25 + 2*q, myCanvas.Height - 25, 25 + 2*q, myCanvas.Height - 15, 2); //0.50
                        Text(22 + 2*q, myCanvas.Height - 15, "0.50");
                        Line(25 + 3*q, myCanvas.Height - 25, 25 + 3*q, myCanvas.Height - 15, 2); //0.75
                        Text(22 + 3*q, myCanvas.Height - 15, "0.75");
                    }
                    break;
                #endregion

                #region Initialize Quantile-Quantile Plot
                case PlotType.Quantile_Quantile:
                    {
                        ProMath math1 = new ProMath(math.Data, xD); math1.MakeDistTable();
                        ProMath math2 = new ProMath(math.Data, yD); math2.MakeDistTable();

                        Line(25, 25, 25, myCanvas.Height - 25, 2); //Axis y
                        Line(25, myCanvas.Height - 25, myCanvas.Width - 25, myCanvas.Height - 25, 2); //Axis x

                        Line(15, myCanvas.Height - 25, 25, myCanvas.Height - 25, 2); //Min Value
                        Text(5, myCanvas.Height - 35, math1.DistTable.Min(m=>m.Value).ToString());
                        Line(15, 25, 25, 25, 2); //Max Value
                        Text(5, 15, math1.DistTable.Max(m => m.Value).ToString());

                        Line(25, myCanvas.Height - 25, 25, myCanvas.Height - 15, 2); //Min value
                        Text(22, myCanvas.Height - 15, math2.DistTable.Min(m => m.Value).ToString());
                        Line(myCanvas.Width - 25, myCanvas.Height - 25, myCanvas.Width - 25, myCanvas.Height - 15, 2); //Max value
                        Text(myCanvas.Width - 30, myCanvas.Height - 15, math2.DistTable.Max(m => m.Value).ToString());

                        //color = Brushes.LimeGreen;
                        Line(25, myCanvas.Height - 25, myCanvas.Width - 25, 25, 2);
                        color = Brushes.Black;
                    }
                    break;
                #endregion

                #region Initialize Scatter Plot
                case PlotType.Scatter:
                    {
                        ProMath xMath = new ProMath(math.Data, xD); xMath.MakeDistTable();
                        ProMath yMath = new ProMath(math.Data, yD); yMath.MakeDistTable();

                        Line(25, myCanvas.Height - 25, myCanvas.Width - 25, myCanvas.Height - 25, 2); //Axis x
                        Line(25, myCanvas.Height - 25, 25, myCanvas.Height - 15, 2); //Min Values
                        Text(22, myCanvas.Height - 15, xMath.DistTable.Min(m => m.Value).ToString());
                        Line(myCanvas.Width - 25, myCanvas.Height - 25, myCanvas.Width - 25, myCanvas.Height - 15, 2); //Max Value
                        Text(myCanvas.Width - 30, myCanvas.Height - 15, xMath.DistTable.Max(m => m.Value).ToString());

                        Line(25, 25, 25, myCanvas.Height - 25, 2); //Axis y
                        Line(15, myCanvas.Height - 25, 25, myCanvas.Height - 25, 2); //Min Value
                        Text(5, myCanvas.Height - 35, yMath.DistTable.Min(m => m.Value).ToString());
                        Line(15, 25, 25, 25, 2); //Max Value
                        Text(5, 15, yMath.DistTable.Max(m => m.Value).ToString());
                    }
                    break;
                #endregion
            }


        }

        /// <summary>
        /// Draw Histogram plot
        /// </summary>
        /// <param name="color">color of Histogram Plot</param>
        public void HistogramPlot(Color color)
        {
            List<string> tmpV = new List<string>();
            List<int> tmpF = new List<int>();

            string toolTip = "";
            int i = 0;
            double k = (myCanvas.Height - 40 - 1) / data.Max(m => m.Frequency);

            double rWidth = (myCanvas.Width - 30) / data.Count;
            double rHeight = 0;

            foreach (DataItem it in data)
            {
                if (k <= 0)
                    rHeight = it.Frequency / 2 - 15;
                else if (k == 1)
                    rHeight = it.Frequency - 15;
                else
                    if (k * it.Frequency > myCanvas.Height - 40)
                        rHeight = myCanvas.Height - 15;
                    else
                        rHeight = it.Frequency * k;

                toolTip = "Value = " + it.Value.ToString() + "\nFrequency = " + it.Frequency.ToString() + "\nProbability = " + it.Probability.ToString();
                Rectangle(toolTip, 15 + i * rWidth, myCanvas.Height - 25 - rHeight - 1, rWidth, rHeight, 2, color);
                i++;
            }


        }


        /// <summary>
        /// Draw Box plot
        /// </summary>
        /// <param name="color">color of Box Plot</param>
        public void BoxPlot(Color color)
        {
            double min = math.DistTable.Min(m => m.Value);
            double pixel = (myCanvas.Width - 30) /math.Range();
            List<double> Q = math.Quartile();
            string toolTip;

            toolTip = "Min = " + min.ToString() + "\nQ1 = " + Q[0].ToString() + "\nQ2 = " + Q[1].ToString();
            Rectangle(toolTip, 15 + pixel * (Q[0] - min), myCanvas.Height - 60, pixel * (Q[1] - min) - pixel * (Q[0] - min), 25, 2, color);
            toolTip = "Q3 = " + Q[2].ToString() + "\nRange = " + math.Range().ToString() + "\nMax = " + math.DistTable.Max(m => m.Value).ToString(); 
            Rectangle(toolTip, 15 + pixel * (Q[1] - min) + 1, myCanvas.Height - 60, pixel * (Q[2] - min) - pixel * (Q[1] - min), 25, 2, color);

            Line(15, myCanvas.Height - 46.5, 15 + pixel * (Q[0] - min), myCanvas.Height - 46.5, 2);
            Line(15 + pixel * (Q[2] - min), myCanvas.Height - 46.5, myCanvas.Width - 15, myCanvas.Height - 46.5, 2); 
        }


        /// <summary>
        /// Draw Quantile plot
        /// </summary>
        /// <param name="color">color of Quantile Plot</param>
        public void QuantilePlot (Color color)
        {
            int n = data.Count;
            double min = data.Min(m => m.Value);
            double max = data.Max(M => M.Value);
            double wPixel = (myCanvas.Width-50) / 100;
            double hPixel = (myCanvas.Height-50) /(max-min);
            List<double> Q = math.Quartile();

            for (int i = 0; i < data.Count; ++i)
            {
                double x = 15 + (wPixel * 100 * (i + 0.5)) / n;
                double y = myCanvas.Height -25 - hPixel * (data[i].Value-min);


                string toolTip = "Value = " + data[i].Value.ToString() + "\nFrequency = " + data[i].Frequency.ToString() + "\nProbability = " + data[i].Probability.ToString();
                Point(7, toolTip, x, y, color);

                //Q1
                x = 15 + (wPixel * 25);
                y = myCanvas.Height - 30 - hPixel *( Q[0]-min);
                toolTip = "Q1 = Value = " + Q[0].ToString() + "\nFrequency = " + data[i].Frequency.ToString() + "\nProbability = " + data[i].Probability.ToString();
                Point(7, toolTip, x, y, Color.FromArgb(170, 0, 250, 0));

                //Q2
                x = 15 + (wPixel * 50);
                y = myCanvas.Height - 30 - hPixel * (Q[1]-min) ;
                toolTip = "Median = Value = " + Q[1].ToString() + "\nFrequency = " + data[i].Frequency.ToString() + "\nProbability = " + data[i].Probability.ToString();
                Point(7, toolTip, x, y, Color.FromArgb(170, 0, 250, 0));

                //Q3
                x = 15 + (wPixel * 75);
                y = myCanvas.Height - 30 - hPixel * (Q[2]-min) ;
                toolTip = "Q3 = Value = " + Q[2].ToString() + "\nFrequency = " + data[i].Frequency.ToString() + "\nProbability = " + data[i].Probability.ToString();
                Point(7,toolTip, x, y, Color.FromArgb(170, 0, 250, 0));
            }

        }


        /// <summary>
        /// Draw QQ Plot
        /// </summary>
        /// <param name="color"></param>
        public void QQPlot(Color color)
        {
            ProMath math1 = new ProMath(math.Data, xD); math1.MakeDistTable();
            ProMath math2 = new ProMath(math.Data, yD); math2.MakeDistTable();

            string toolTip = "";
            double wPixel, hPixel, maxX, minX, maxY, minY;
            double x, y;
            int n;

            if (math1.DistTable.Count < math2.DistTable.Count)
            {
                List<double> xQ = math2.Quartile();
                List<double> yQ = math1.Quartile();

                n = math1.DistTable.Count;
                maxX = math2.DistTable.Max(m => m.Value); minX = math2.DistTable.Min(m => m.Value);
                maxY = math1.DistTable.Max(m => m.Value); minY = math1.DistTable.Min(m => m.Value);
                wPixel = (myCanvas.Width - 50) / (maxX - minX);
                hPixel = (myCanvas.Height - 50) / (maxY - minY);

                for (int i = 0; i < n; ++i)
                {
                    x = 25 + wPixel * (math2.DistTable[i].Value - minX);
                    y = myCanvas.Height - 25 - hPixel * (math1.DistTable[i].Value - minY);
                    toolTip = "(" + math2.DistTable[i].Value + " ; " + math1.DistTable[i].Value + ")";

                    Point(7, toolTip, x, y, color);
                }

                toolTip = "Q1 Value \n" + "(" + xQ[0] + " ; " + yQ[0] + ")";
                Point(7, toolTip, 25 + wPixel * (xQ[0] - minX), myCanvas.Height - 25 - hPixel * (yQ[0] - minY), Color.FromArgb(170, 0, 250, 0));
                toolTip = "Median Value \n" + "(" + xQ[1] + " ; " + yQ[1] + ")";
                Point(7, toolTip, 25 + wPixel * (xQ[1] - minX), myCanvas.Height - 25 - hPixel * (yQ[1] - minY), Color.FromArgb(170, 0, 250, 0));
                toolTip = "Q3 Value \n" + "(" + xQ[2] + " ; " + yQ[2] + ")";
                Point(7, toolTip, 25 + wPixel * (xQ[2] - minX), myCanvas.Height - 25 - hPixel * (yQ[2] - minY), Color.FromArgb(170, 0, 250, 0));

            }
            else
            {
                List<double> xQ = math1.Quartile();
                List<double> yQ = math2.Quartile();

                n = math2.DistTable.Count;
                maxX = math1.DistTable.Max(m => m.Value); minX = math1.DistTable.Min(m => m.Value);
                maxY = math2.DistTable.Max(m => m.Value); minY = math2.DistTable.Min(m => m.Value);
                wPixel = (myCanvas.Width - 50) / (maxX - minX);
                hPixel = (myCanvas.Height - 50) / (maxY - minY);

                for (int i = 0; i < n; ++i)
                {
                    x = 25 + wPixel * (math1.DistTable[i].Value - minX);
                    y = myCanvas.Height - 25 - hPixel * (math2.DistTable[i].Value - minY);
                    toolTip = "(" + math1.DistTable[i].Value + " ; " + math2.DistTable[i].Value + ")";

                    Point(7, toolTip, x, y, color);
                }

                toolTip = "Q1 Value \n" + "(" + xQ[0] + " ; " + yQ[0] + ")";
                Point(7, toolTip, 25 + wPixel * (xQ[0] - minX), myCanvas.Height - 25 - hPixel * (yQ[0] - minY), Color.FromArgb(170, 0, 250, 0));
                toolTip = "Median Value \n" + "(" + xQ[1] + " ; " + yQ[1] + ")";
                Point(7, toolTip, 25 + wPixel * (xQ[1] - minX), myCanvas.Height - 25 - hPixel * (yQ[1] - minY), Color.FromArgb(170, 0, 250, 0));
                toolTip = "Q3 Value \n" + "(" + xQ[2] + " ; " + yQ[2] + ")";
                Point(7, toolTip, 25 + wPixel * (xQ[2] - minX), myCanvas.Height - 25 - hPixel * (yQ[2] - minY), Color.FromArgb(170, 0, 250, 0));
            }

            


        }


        /// <summary>
        /// Draw Scatter Plot 
        /// </summary>
        /// <param name="xColor">??</param>
        /// <param name="yColor">??</param>
        public void ScatterPlot(Color color1, Color color2)
        {
            Dataset tmpData = new Dataset();
            ProMath xMath = new ProMath(math.Data, xD); xMath.MakeDistTable();
            ProMath yMath = new ProMath(math.Data, yD); yMath.MakeDistTable();

            double xMax = xMath.DistTable.Max(m => m.Value);
            double xMin = xMath.DistTable.Min(m => m.Value);
            double yMax = yMath.DistTable.Max(m => m.Value);
            double yMin = yMath.DistTable.Min(m => m.Value);
            double wPixel = (myCanvas.Width - 50) /( xMax-xMin);
            double hPixel = (myCanvas.Height - 50) / (yMax-yMin);

            double x=0, y=0;
            string toolTip="";

            tmpData = math.Data;
            
            switch (math.Data.dataType)
            { 
                case DatasetType.InpTypes.LibSVM:
                    for (int i = 0; i< tmpData.Count; ++i)
                    {
                        x = 18 + wPixel * (tmpData.LibSVMData[i][xD - 1].Value - xMin);
                        y = myCanvas.Height - 25 - hPixel * (tmpData.LibSVMData[i][yD - 1].Value - yMin);
                        toolTip = "Demenisional " + xD + " : " + tmpData.LibSVMData[i][xD-1].Value + "\nDemenisional " + yD + " : " + tmpData.LibSVMData[i][yD-1].Value;

                        Point(7,toolTip, x, y, Color.FromArgb(200,0,(byte)(i+1),(byte)(i+1)));
                    }
                    break;
                case DatasetType.InpTypes.CSV:
                    for (int i = 0; i< tmpData.Count; ++i)
                    {
                        x = 18 + wPixel * (double.Parse(tmpData.CSVData[i][xD-1])-xMin);
                        y = myCanvas.Height - 25 - hPixel * (double.Parse(tmpData.CSVData[i][yD-1])-yMin);
                        toolTip = "Demenisional " + xD + " : " + tmpData.CSVData[i][xD-1] + "\nDemenisional " + yD + " : " + tmpData.CSVData[i][yD-1];

                        Point(7,toolTip, x, y, Color.FromArgb(200,0,(byte)(i+1),(byte)(i+1)));
                    }
                    break;
            }


        }





        #endregion


    }
}
