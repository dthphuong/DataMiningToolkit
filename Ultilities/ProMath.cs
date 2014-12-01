using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataMining;

namespace Ultilities
{
    public class DataItem
    {
        public double Value { get; set; }
        public int Frequency { get; set; }
        public double Probability { get; set; }
    }


    public class ProMath
    {
        #region Variable

        List<DataItem> distTable = new List<DataItem>();
        Dataset data = new Dataset();
        int d=1;

        #endregion

        #region Properties

        /// <summary>
        /// Distribute Table
        /// </summary>
        public List<DataItem> DistTable
        {
            get
            {
                return distTable;
            }
            set
            {
                if (value != distTable)
                    distTable = value;
            }
        }

        /// <summary>
        /// Dataset
        /// </summary>
        public Dataset Data
        {
            get
            {
                return data;
            }
            set
            {
                if (value != data)
                    data = value;
            }
        }

        //Demensional
        public int D
        {
            get
            {
                return d;
            }
            set
            {
                if (value != d)
                    d = value;
            }
        }

        #endregion

        #region Method

        //Empty Constructor
        public ProMath()
        {
            distTable.Clear();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Dataset</param>
        /// <param name="d">Demensional</param>
        public ProMath(Dataset data, int d)
        {
            distTable.Clear();
            this.data = data;
            this.d = d;
        }

        /// <summary>
        /// Create Distribute Table
        /// </summary>
        public void MakeDistTable()
        {
            List<double> lstData = new List<double>();

            switch (data.dataType)
            {
                case DatasetType.InpTypes.LibSVM:
                    {
                        for (int i = 0; i < data.Count; ++i)
                        {
                            if (d == 0) d = 1;
                            if (data.LibSVMData[i][d-1].Index == d)
                            {
                                lstData.Add(data.LibSVMData[i][d-1].Value);
                            }
                            else
                                continue;
                        }
                    }
                    break;
                case DatasetType.InpTypes.CSV:
                    {
                        if (data.IsNumberic[d-1] == 1)
                        {
                            for (int i = 0; i < data.Count; ++i)
                            {
                                lstData.Add(double.Parse(data.CSVData[i][d-1]));
                            }
                        }
                    }
                    break;
            }

            lstData.Sort();
            int count = 1;
            double x = lstData[0];
            distTable.Clear();

            for (int i = 1; i < lstData.Count; ++i)
            {
                if (lstData[i] == x)
                {
                    count++;
                }
                else
                {
                    distTable.Add(new DataItem { Value = x, Frequency = count, Probability = Math.Round((double)count / lstData.Count, 3) });

                    x = lstData[i];
                    count = 1;
                }

                if (i == lstData.Count - 1)
                    distTable.Add(new DataItem { Value = x, Frequency = count, Probability = Math.Round((double)count / lstData.Count, 3) });
            }
        }


        /// <summary>
        /// Calc the expect of dimensional d
        /// </summary>
        /// <returns>expect of dimensional d</returns>
        public double Expect()
        {
            double expect = 0;
            double sum = 0;

            for (int i = 0; i < distTable.Count; ++i )
            {
                expect += distTable[i].Value * distTable[i].Frequency;
                sum += distTable[i].Frequency;
            }

            expect = Math.Round(expect / sum, 3);

            return expect;
        }

        /// <summary>
        /// Calc the normal Normal Variance of dimensional d
        /// </summary>
        /// <returns>normal variance of dimensional d</returns>
        public double NormalVariance()
        {
            double variance = 0;
            double sum = 0;

            for (int i = 0; i < distTable.Count; ++i)
            {
                variance += distTable[i].Value * distTable[i].Value * distTable[i].Frequency;
                sum += distTable[i].Frequency;
            }

            variance = Math.Round(variance / sum - Math.Pow(Expect(),2),3);

            return variance;
        }

        /// <summary>
        /// Calc the normal Adjusted Variance of dimensional d
        /// </summary>
        /// <returns>adjusted variance of dimensional d</returns>
        public double AdjustedVariance()
        {
            double variance = 0;
            double sum = 0;

            for (int i = 0; i < distTable.Count; ++i)
                sum += distTable[i].Frequency;

            variance = Math.Round(sum / (sum - 1)* NormalVariance(),3); 

            return variance;
        }

        /// <summary>
        /// Calc the Deviation of dimensional d
        /// </summary>
        /// <returns>deviation of dimensional d</returns>
        public double Deviation()
        {
            return Math.Sqrt(NormalVariance());
        }

        /// <summary>
        /// Calc Median from List data
        /// </summary>
        /// <param name="data">List of data</param>
        /// <returns>median</returns>
        public double Median(List<double> data)
        {
            int k = data.Count / 2 - 1;

            if (data.Count % 2 == 0)
                return Math.Round((data[k] + data[k + 1]) / 2, 5);
            else
                return Math.Round(data[k + 1], 3);
        }

        /// <summary>
        /// Calc the Median of dimensional d
        /// </summary>
        /// <returns>median of dimensional d</returns>
        public double Median()
        {
            double med = 0;
            List<double> temp = new List<double>();

            for (int i = 0; i < distTable.Count; ++i )
                temp.Add(distTable[i].Value);

            med = Median(temp);

            return med;
        }

        /// <summary>
        /// Calc the Mod of dimensional d
        /// </summary>
        /// <returns>mod of dimensional d</returns>
        public List<double> Mod ()
        {
            List<double> result = new List<double>();
            List<double> temp = new List<double>();
            double max = 0;

            for (int i = 0; i < distTable.Count; ++i)
                temp.Add(distTable[i].Probability);

            max = temp.Max();
            
            for (int i = 0; i < temp.Count; ++i)
            {
                if (temp[i] == max)
                    result.Add(distTable[i].Value);
            }

            return result;
        }

        /// <summary>
        /// Calc Range of demensional d
        /// </summary>
        /// <returns>Range of demensional d</returns>
        public double Range()
        {
            return distTable.Max(v => v.Value) - distTable.Min(z => z.Value);
        }

        /// <summary>
        /// Calc Quartile of demensional d
        /// </summary>
        /// <param name="data">Dataset</param>
        /// <param name="d">Demensional</param>
        /// <returns>Quartile</returns>
        public List<double> Quartile ()
        {
            List<double> result = new List<double>() {0,0,0};
            List<double> temp = new List<double>();
            List<double> part = new List<double>();
            int mid = 0;

            for (int i = 0; i < distTable.Count; ++i)
                temp.Add(distTable[i].Value);

            mid = temp.Count / 2 - 1;
            result[1] = Median(temp); //Q2

            for (int i = 0; i < temp.Count / 2; ++i)
                part.Add(temp[i]);
            result[0] = Median(part); //Q1

            part.Clear();
            for (int i = temp.Count / 2 + 1; i < temp.Count; ++i)
                part.Add(temp[i]);
            result[2] = Median(part); //Q3

            return result;
        }

        /// <summary>
        /// Calc InterQuartile of demensional d
        /// </summary>
        /// <param name="data">Dataset</param>
        /// <param name="d">Demensional</param>
        /// <returns>InterQuartile Range</returns>
        public double InterQuartile ()
        {
            List<double> temp = new List<double>();
            temp = Quartile();
            return ( temp[2] - temp[0] );
        }

        #endregion
    }
}
