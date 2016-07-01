using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataMining;

namespace DataMining
{
    public class Dataset
    {
        #region Variable
        public DatasetType.InpTypes dataType = new DatasetType.InpTypes();
        private int _count;

        //LibSVM
        private double[] _label;
        private Node[][] _libsvmData;
        private int _maxIndex;

        //CSV
        private string[] _header;
        private string[][] _csvData;
        private int[] _isNumberic;

        #endregion

        #region Properties

        /// <summary>
        /// Number of Vectors
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                if (value != _count)
                    _count = value;
            }
        }

        /// <summary>
        /// Vector Label
        /// </summary>
        public double[] Label
        {
            get
            {
                return _label;
            }
            set
            {
                if (value != _label)
                    _label = value;
            }
        }

        /// <summary>
        /// Vector Data
        /// </summary>
        public Node[][] LibSVMData
        {
            get
            {
                return _libsvmData;
            }
            set
            {
                if (value != _libsvmData)
                    _libsvmData = value;
            }
        }

        /// <summary>
        /// Max Index of a vector
        /// </summary>
        public int MaxIndex
        {
            get
            {
                return _maxIndex;
            }
            set
            {
                if (value != _maxIndex)
                    _maxIndex = value;
            }
        }

        /// <summary>
        /// A Header of CSV Data
        /// </summary>
        public string[] Header
        {
            get
            {
                return _header;
            }
            set
            {
                if (value != _header)
                    _header = value;
            }
        }

        /// <summary>
        /// Data of CSV Data
        /// </summary>
        public string[][] CSVData
        {
            get
            {
                return _csvData;
            }
            set
            {
                if (value != _csvData)
                    _csvData = value;
            }
        }

        /// <summary>
        /// Check data in CSV is NUmberic ?
        /// </summary>
        public int[] IsNumberic
        {
            get
            {
                return _isNumberic;
            }
            set
            {
                if (value != _isNumberic)
                    _isNumberic = value;
            }
        }

        public int NrPos { get; set; }
        public int NrNeg { get; set; }

        #endregion

        #region Method
        
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public Dataset()
        {
        }

        /// <summary>
        /// Constructor for LibSVM Type
        /// </summary>
        /// <param name="count">Number of Vectors</param>
        /// <param name="label">Label of Vector</param>
        /// <param name="data">Data of Vector</param>
        /// <param name="maxIndex">Max Index of a Vector</param>
        public Dataset(int count, double[] label, Node[][] data, int maxIndex)
        {
            dataType = DatasetType.InpTypes.LibSVM;
            _count = count;
            _label = label;
            _libsvmData = data;
            _maxIndex = maxIndex;
        }

        /// <summary>
        /// Constructor for CSV Type
        /// </summary>
        /// <param name="count">Number of Data</param>
        /// <param name="header">Header of Data</param>
        /// <param name="data">Data of CSV</param>
        public Dataset(int count, string[] header, string[][] data, int[] isNumberic)
        {
            dataType = DatasetType.InpTypes.CSV;
            _count = count;
            _header = header;
            _csvData = data;
            _isNumberic = isNumberic;
        }


        /// <summary>
        /// Read a Dataset from a Stream
        /// </summary>
        /// <param name="stream">Stream to Read</param>
        /// <returns>The Dataset</returns>
        private static Dataset Read(Stream stream, DatasetType.InpTypes Type)
        {
            StreamReader reader = new StreamReader(stream);
            //LibSVM Variable
            List<double> Y = new List<double>();
            List<Node[]> X = new List<Node[]>();
            int max_index = 0;
            //CSV Variable
            string[] csvY;
            List<string[]> csvX = new List<string[]>();
            List<int> isNumber = new List<int>();

            if (Type == DatasetType.InpTypes.LibSVM)
            {
                while (!reader.EndOfStream)
                {
                    string[] tmp = reader.ReadLine().Split(' ');

                    for (int i = 0; i < tmp.Length; ++i)
                        if (tmp[i] == "")
                            tmp = tmp.Where(w => w != tmp[i]).ToArray();

                    int m = tmp.Length - 1;
                    Node[] x = new Node[m];

                    Y.Add(double.Parse(tmp[0]));

                    for (int i = 0; i < m; ++i)
                    {
                        string[] t = tmp[i+1].Split(':');
                        x[i] = new Node();
                        x[i].Index = int.Parse(t[0]);
                        x[i].Value = double.Parse(t[1]);
                    }

                    if (m > 0)
                        max_index = Math.Max(max_index, x[m - 1].Index);

                    X.Add(x);
                }

                return new Dataset(Y.Count, Y.ToArray(), X.ToArray(), max_index);
            }
            else
            {
                csvY = reader.ReadLine().Split(',');

                while (!reader.EndOfStream)
                {
                    string[] tmp = reader.ReadLine().Split(',');
                    csvX.Add(tmp);
                }

                int nrFlag;
                double tmpData;

                for (int i = 0; i < csvX.Count; ++i)
                {
                    nrFlag = 0;

                    for (int j = 0; j < csvY.Length; ++j)
                    {
                        try
                        {
                            tmpData = double.Parse(csvX[j][i]);
                            nrFlag = 1;
                        }
                        catch
                        {
                            nrFlag = 0;
                            break;
                        }
                    }

                    if (nrFlag == 0)
                        isNumber.Add(0);
                    else
                        isNumber.Add(1);
                }

                return new Dataset(csvX.Count, csvY, csvX.ToArray(),isNumber.ToArray());
            }
        }

        /// <summary>
        /// Read a Dataset from a File.
        /// </summary>
        /// <param name="filename">The path of the file to Read</param>
        /// <returns></returns>
        public static Dataset Read(string filename, DatasetType.InpTypes filetype)
        {
            FileStream fStream = File.OpenRead(filename);

            if (filetype == DatasetType.InpTypes.LibSVM)
            {
                try
                {
                    return Read(fStream, DatasetType.InpTypes.LibSVM);
                }
                finally
                {
                    fStream.Close();
                }
            }
            else
            {
                try
                {
                    return Read(fStream, DatasetType.InpTypes.CSV);
                }
                finally
                {
                    fStream.Close();
                }
            }
        }

        /// <summary>
        /// Count Number of Positive and Negative label
        /// </summary>
        public void CountPosNeg()
        {
            int nrPos = 0, nrNeg = 0;

            for (int i = 0; i < Count; ++i)
            {
                if (Label[i] > 0)
                    nrPos++;
                else
                    nrNeg++;
            }

            NrPos = nrPos; NrNeg = nrNeg;
        }


        public void Export(DatasetType.InpTypes toType, string path)
        {
            switch (toType)
            {
                case DatasetType.InpTypes.CSV:
                    {
                        StreamWriter sw = new StreamWriter(path);

                        for (int i = 0; i < MaxIndex - 1; ++i)
                            sw.Write("att_" + (i + 1) + ",");
                        sw.WriteLine("class");

                        for (int i = 0; i < Count; ++i )
                        {
                            sw.Write(LibSVMData[i][0].Value);
                            for (int j = 1; j < MaxIndex - 1 ; ++j)
                                sw.Write("," + LibSVMData[i][j].Value);
                            sw.WriteLine("," + Label[i]);
                        }

                        sw.Close();
                    }
                    break;

                case DatasetType.InpTypes.LibSVM:
                    {
                        StreamWriter sw = new StreamWriter(path);



                        sw.Close();
                    }
                    break;
            }
        }

        #endregion

    }
}
