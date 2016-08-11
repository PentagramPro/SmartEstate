using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleReportGenerator
{
    public class DiagramBuilder
    {
        private List<DataObject> Data;
        private int Index;

        private int recordsBefore = 0, recordsAfter = 0;
        private float OverallMax = float.NaN, OverallMin = float.NaN;
        private SortedDictionary<DateTime, Node> Nodes = new SortedDictionary<DateTime, Node>();

        private class Node
        {
            public float Min, Avg, Max;
            public int Count = 0;

            public Node(float val)
            {
                Min = val;
                Max = val;
                Avg = val;
            }
        }

        public DiagramBuilder(List<DataObject> data, int index)
        {
            Data = data;
            Index = index;
        }

        public void Build()
        {
            DateTime now = DateTime.Now;
            DateTime start = now.AddHours(-24);



            foreach (var dataObject in Data)
            {
                if (dataObject.Time < start)
                {
                    recordsBefore++;
                    continue;
                }

                if (dataObject.Time > now)
                {
                    recordsAfter++;
                    continue;
                }

                DateTime n = new DateTime(dataObject.Time.Year, dataObject.Time.Month, dataObject.Time.Day,
                    dataObject.Time.Hour, 0, 0);

                Node node = null;
                float curVal = dataObject.Values[Index];
                if (!Nodes.TryGetValue(n, out node))
                {
                    node = new Node(curVal);
                    Nodes[n] = node;
                }
                node.Avg = (node.Avg*node.Count + curVal)*(node.Count + 1);
                node.Count++;
                node.Max = Math.Max(node.Max, curVal);
                node.Min = Math.Min(node.Min, curVal);

                if (float.IsNaN(OverallMax) || OverallMax < node.Max)
                    OverallMax = node.Max;
                if (float.IsNaN(OverallMin) || OverallMin < node.Min)
                    OverallMin = node.Min;
            }
        }

        private void StoreDiagram(int width, int height)
        {
            int step = width/(Nodes.Count + 1);

            using (var bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    int xPos = step;

                    foreach (var node in Nodes)
                    {
                        
                    }
                }
            }
        }
    }
}
