using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReducingVertices
{
    internal class Face
    {
        public bool joined = false;
        public static int staticIndex = 0;
        public int index = -1;
        public List<Vertex> corners = new List<Vertex>(4);
        public List<int> neighborFaceIndexes = new List<int>();
        public Face(List<Vertex> vertIndices)
        {
            index = staticIndex++;
            corners = vertIndices;
            UnityEngine.Debug.Log("cornerCount : " + corners.Count.ToString());
            foreach(var c in corners)
            {
               c.addFace(index);
            }
        }
    }
}
