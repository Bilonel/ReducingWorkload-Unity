using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ReducingVertices
{
    internal class Vertex
    {
        public List<int> linkedFaces = new List<int>();
        public static int staticIndex = 0;
        public int index=-1;

        public float x = -1, y = -1, z = -1;
        public Vector3 vector
        {
            get
            {
                return new Vector3(x,y,z);
            }
        }
        public Vertex(float x,float z,float y) 
        { 
            index = staticIndex++; this.x = x;this.y = y;this.z = z;     
        }

        public void addFace(int f)
        {
            linkedFaces.Add(f);
        }
    }
}
