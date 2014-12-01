using System;
using DataMining;

namespace DataMining
{
    public class Node
    {
        #region Variable
        private int _index;
        private double _value;
        #endregion

        #region Properties
        
        /// <summary>
        /// Index of Node
        /// </summary>
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (value != _index)
                    _index = value;
            }
        }

        /// <summary>
        /// Value of Index
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value != _value)
                    _value = value;
            }
        }

        #endregion

        #region Method

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Node()
        {
        }

        /// <summary>
        /// Constructor 1
        /// </summary>
        /// <param name="Index">Index of Value</param>
        /// <param name="Value">Value to Store</param>
        public Node(int Index, double Value)
        {
            _index = Index;
            _value = Value;
        }


        #endregion


    }
}
