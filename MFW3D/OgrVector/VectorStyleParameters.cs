using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OgrVectorImporter
{
    //矢量对象参数
    public class VectorStyleParameters
    {
        public Color PolygonColor = Color.Blue;
        public Color LineColor = Color.Red;
        public float LineWidth = 2.0f;
        public bool OutlinePolygons = true;
        public string TextFilterString = null;
        public TextFilterType TextFilterType = TextFilterType.Exact;
        public bool TextFilter = false;

        private static string keyDataFieldName;
        private static string labelFieldName;
        private static DataType dataType = DataType.Text;
        private static bool noData = false;
        private static double noDataValue;
        private static string layerName = "Layer Name";

        public string KeyDataFieldName
        {
            get { return keyDataFieldName; }
            set { keyDataFieldName = value; }
        }

        public string LabelFieldName
        {
            get { return labelFieldName; }
            set { labelFieldName = value; }
        }

        public DataType DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        public string Layername
        {
            get { return layerName; }
            set { layerName = value; }
        }

        public bool NoData
        {
            get { return noData; }
            set { noData = value; }
        }

        public double NoDataValue
        {
            get { return noDataValue; }
            set { noDataValue = value; }
        }
    }
}
