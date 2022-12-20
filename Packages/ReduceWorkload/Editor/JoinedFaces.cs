using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReducingVertices
{
    internal class JoinedFaces
    {
        public Face face1;
        public Face face2;
        public List<Vertex> corners { get; set; }
        public List<Vertex> commons { get; set; }
        public JoinedFaces(Face face1, Face face2,List<Vertex> c,List<Vertex> mid)
        { 
            this.face1 = face1;this.face2 = face2; corners = c; commons = mid;
            face1.joined = face2.joined = true;
        }
        public string _toString()
        {
            return "Face " + face1.index.ToString() + " & " + "Face " + face2.index.ToString();
        }
    }
}
