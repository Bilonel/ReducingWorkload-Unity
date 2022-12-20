using System.Collections.Generic;
using System;
using ReducingVertices;
using System.Numerics;
using System.Diagnostics;
using System.Linq;

class Model
{
    double slopeFactor = 0.02;
    public List<JoinedFaces> joinedFaceList = new List<JoinedFaces>();
    public List<Vertex> vertices = new List<Vertex>();
    public List<Face> faces = new List<Face>();

    public Model(string _data, float slopeFactor_=0.02f)
    {
        slopeFactor = slopeFactor_;
        Face.staticIndex = 0;
        Vertex.staticIndex = 0;
        string[] lines = _data.Split("\n");
        foreach (var line in lines)
        {
            if (line.Contains("v ")) // NEW VERTEX
            {
                string[] vars = line.Substring(2).Split(' ');
                if (vars.Length < 3) UnityEngine.Debug.Log("ERROR: Coordinates Of Vertex" + vertices.Count.ToString() + " Couldn't Find");
                vertices.Add(new Vertex(
                    float.Parse(vars[0]),
                    float.Parse(vars[1]),
                    float.Parse(vars[2])
                    ));
            }
            else if (line.Contains("f ")) // NEW FACE
            {
                string[] vars = line.Substring(2).Split(' ');

                List<Vertex> corners = new List<Vertex>();

                foreach (var node in vars)
                {
                    string str = node.Split('/')[0];
                    if (isDigit(str))
                        corners.Add(vertices[int.Parse(str) - 1]);
                }



                faces.Add(new Face(corners));
            }
        }
        setNeighborFaces();
    }
    bool isDigit(string str)
    {
        foreach (var chr in str)
            if (chr < '0' || chr > '9')
                return false;
        return true;
    }
    //public void printVerts(TextBox textBox)
    //{
    //    textBox.Text = "";
    //    foreach (var v in vertices)
    //    {
    //        textBox.Text += v.x.ToString() + " : " + v.z.ToString() + " : " + v.y.ToString() + Environment.NewLine;
    //        foreach (var f in v.linkedFaces)
    //            textBox.Text += "\t" + f.ToString();
    //        textBox.Text += Environment.NewLine;
    //    }
    //    foreach (var f in faces)
    //    {
    //        foreach (var c in f.corners)
    //            textBox.Text += c.index.ToString() + " : ";
    //        textBox.Text += "     | ";
    //        foreach (var n in f.neighborFaceIndexes)
    //            textBox.Text += "\t" + n.ToString();
    //        textBox.Text += Environment.NewLine;
    //    }

    //}
    //public void printFaces(TextBox textBox)
    //{
    //    textBox.Text = "";
    //    foreach (var item in joinedFaceList)
    //        textBox.Text += item._toString() + Environment.NewLine;
    //}
    void setNeighborFaces()
    {
        int i = 0;
        foreach (var face in faces)
        {
            List<int> lookUp = new List<int>();
            foreach (var corner in face.corners)
            {
                foreach (var linkedFaceIndex in corner.linkedFaces)
                {
                    i++;
                    if (linkedFaceIndex == face.index) continue;
                    if (lookUp.FindIndex(x => x == linkedFaceIndex) == -1) lookUp.Add(linkedFaceIndex);
                    else
                    {
                        lookUp.Remove(linkedFaceIndex);
                        face.neighborFaceIndexes.Add(linkedFaceIndex);
                    }
                }
            }
        }

        UnityEngine.Debug.Log("Linked : "+i);
    }
    List<Vertex>[] isEqual(Face face1, Face face2)
    {
        //if(face1.corners.Count==3&& face2.corners.Count == 3)
        //{
        //    List<int> mid = new List<int>();
        //    int corner1=-1;
        //    int corner2=-1;
        //    bool reverse = face1.corners[0].linkedFaces.FindIndex(x => x == face2.index) > -1;
        //    for (int i = 0; i < 3; i++)
        //    {
        //        if (face1.corners[i].linkedFaces.FindIndex(x => x == face2.index) > -1)  // MID POINT
        //        {
        //            mid.Add(face1.corners[i].index);
        //        }
        //        else
        //        {
        //            corner1 = face1.corners[i].index;
        //        }
        //    }
        //    for(int i=0;i<3;i++)
        //    {
        //        if (face2.corners[i].linkedFaces.FindIndex(x => x == face1.index) < 0)
        //        {
        //            corner2 = face2.corners[i].index;
        //            break;
        //        }    
        //    }

        //    float midX = (vertices[mid[0]].x + vertices[mid[1]].x) / 2;
        //    float midY = (vertices[mid[0]].y + vertices[mid[1]].y) / 2;
        //    float midZ = (vertices[mid[0]].z + vertices[mid[1]].z) / 2;

        //    bool flag = false;
        //    if (corner1 > 0 && corner2 > 0 && mid.Count==2)
        //        flag = distanceBetweenVertexAndVector(corner1, corner2,new Vector3(midX,midY,midZ));
        //    if (flag)
        //    {
        //        List<Vertex> vs = new List<Vertex>();
        //        List<Vertex> corners = new List<Vertex>();
        //        for(int i=0;i<3;i++)
        //        {
        //            if(!vs.Exists(x=>x==face1.corners[i]))
        //            {
        //                vs.Add(face1.corners[i]);
        //            }
        //            if (!vs.Exists(x => x == face2.corners[i]))
        //                vs.Add(face2.corners[i]);
        //        }
        //        for(int i=0;i<4;i++)
        //        {
        //            int min = int.MaxValue;
        //            foreach(var item in vs)
        //                if (item.index < min)
        //                    min = item.index;
        //            Vertex minV = vertices[min];
        //            vs.Remove(minV);
        //            corners.Add(minV);
        //        }
        //            return new List<Vertex>[]{
        //                    corners,
        //                    new List<Vertex>()
        //                    {
        //                        vertices[mid[0]],
        //                        vertices[mid[1]]
        //                    }
        //                };
               
        //    }
        //}
        if (face1.corners.Count == 3 && face2.corners.Count == 3)
        {
            List<int> mid = new List<int>();
            int corner1 = -1;
            int corner2 = -1;
            for (int i = 0; i < 3; i++)
            {
                if (face1.corners[i].linkedFaces.FindIndex(x => x == face2.index) > -1)  // MID POINT
                {
                    mid.Add(face1.corners[i].index);
                }
                else
                {
                    corner1 = face1.corners[i].index;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (face2.corners[i].linkedFaces.FindIndex(x => x == face1.index) < 0)
                {
                    corner2 = face2.corners[i].index;
                    break;
                }
            }
            

            float midX = (vertices[mid[0]].x + vertices[mid[1]].x) / 2;
            float midY = (vertices[mid[0]].y + vertices[mid[1]].y) / 2;
            float midZ = (vertices[mid[0]].z + vertices[mid[1]].z) / 2;

            bool flag = false;
            if (corner1 > 0 && corner2 > 0 && mid.Count == 2)
                flag = distanceBetweenVertexAndVector(corner1, corner2, new Vector3(midX, midY, midZ));
            if (flag)
            {
                List<Vertex> tmp = new List<Vertex>();
                if (face1.corners[0].linkedFaces.FindIndex(x => x == face2.index) >-1)
                {
                    tmp = new List<Vertex>()
                            {
                                vertices[mid[0]],
                                vertices[corner1],
                                vertices[mid[1]],
                                vertices[corner2],
                            };
                }
                else
                {
                    tmp = new List<Vertex>()
                            {
                                vertices[corner1],
                                vertices[mid[0]],
                                vertices[corner2],
                                vertices[mid[1]],
                            };
                }

                return new List<Vertex>[]{
                            tmp,
                            new List<Vertex>()
                            {
                                vertices[mid[0]],
                                vertices[mid[1]]
                            }
                        };

            }
        }
        //else if(face1.corners.Count == 4 && face2.corners.Count == 4)
        //{
        //    List<int> mid = new List<int>();
        //    List<int> corners = new List<int>();
        //    List<int> pairFace1 = new List<int>();
        //    List<int> pairFace2 = new List<int>();
        //    for (int i = 0; i < 3; i++)
        //    {
        //        if (face1.corners[i].linkedFaces.FindIndex(x => x == face2.index) > -1)  // MID POINT
        //        {
        //            mid.Add(face1.corners[i].index);
        //            corners.Add(face2.corners[i].index);
        //            pairFace2.Add(face2.corners[i].index);
        //        }
        //        else
        //        {
        //            corners.Add(face1.corners[i].index);
        //            pairFace1.Add(face1.corners[i].index);
        //        }
        //    }

        //    bool flag = distanceBetweenVertexAndVector(pairFace1[0], pairFace2[0], mid[0])
        //        && distanceBetweenVertexAndVector(pairFace1[1], pairFace2[1], mid[1]);

        //    if (flag)
        //        return new List<Vertex>[]{
        //                new List<Vertex>(){
        //                    vertices[corners[0]],
        //                    vertices[corners[1]],
        //                    vertices[corners[2]],
        //                    vertices[corners[3]]
        //            },
        //                new List<Vertex>()
        //                {
        //                    vertices[mid[0]],
        //                    vertices[mid[1]]
        //                }
        //            };

        //}
        return new List<Vertex>[] { new List<Vertex>(), new List<Vertex>() };
    }
    bool distanceBetweenVertexAndVector(int start, int finish, Vector3 point)
    {
        Vertex vertex1 = vertices[start], vertex2 = vertices[finish];
        double a = (vertex1.vector - vertex2.vector).Length(); // 3 POINTS -> 1 TRIANGLE
        double b = (vertex2.vector - point).Length();
        double c = (point - vertex1.vector).Length();

        double u = (a + b + c) / 2;
        double area = Math.Sqrt(u * (u - a) * (u - b) * (u - c));
        double height = 2 * area / a;
        
        return height < slopeFactor;
    }
    bool distanceBetweenVertexAndVector(int start, int finish, int mid)
    {
        Vertex vertex1 = vertices[start], vertex2 = vertices[finish], point = vertices[mid];
        double a = (vertex1.vector - vertex2.vector).Length(); // 3 POINTS -> 1 TRIANGLE
        double b = (vertex2.vector - point.vector).Length();
        double c = (point.vector - vertex1.vector).Length();

        double u = (a + b + c) / 2;
        double area = Math.Sqrt(u * (u - a) * (u - b) * (u - c));
        double height = 2 * area / a;

        return height < slopeFactor;
    }
    public void Run()
    {
        int i = 0;
        foreach (var face in faces)
        {
            if (face.joined) continue;
                foreach (var neigh in face.neighborFaceIndexes)
                {
                    i++;
                    if (faces[neigh].joined) continue;
                    faces[neigh].neighborFaceIndexes.Remove(face.index); // REMOVE NEIGHBOR REFERENCE FROM PARTNER
                    List<Vertex> cornersOfNewFace = isEqual(face, faces[neigh])[0];
                    List<Vertex> midVertices = isEqual(face, faces[neigh])[1];
                    if (cornersOfNewFace.Count > 0)
                    {
                        joinedFaceList.Add(new JoinedFaces(face, faces[neigh], cornersOfNewFace, midVertices));
                        break;
                    }
                }
        }
        UnityEngine.Debug.Log("Neigh : "+i);
    }
    public string Save()
    {
        List<Vertex> newVertices = new List<Vertex>();
        foreach (var f in faces)
        {
            if (f.joined)
            {
                JoinedFaces Jf = joinedFaceList.Find(x => x.face1 == f || x.face2 == f);

                foreach (var v in Jf.corners)
                    if (newVertices.FindIndex(x => x == v) < 0)
                        newVertices.Add(v);
            }
            else
                foreach (var v in f.corners)
                    if (newVertices.FindIndex(x => x == v) < 0)
                        newVertices.Add(v);
        }
        int index = 0;
        foreach (var v in vertices)
            if (newVertices.IndexOf(v) > -1)
                v.index = index++;
            else v.index = -1;

        string facesData = "";
        string verticesData = "";

        //foreach(var item in joinedFaceList)
        //{
        //    string faceData = "f ";
        //    foreach(var ver in item.corners)
        //    {
        //        string vertexData = "v " + ver.x.ToString() + " " + ver.z.ToString() + " " + ver.y.ToString(); // VERTEX DATA COMPLETED
        //        verticesData += vertexData + Environment.NewLine;

        //        faceData += ver.index.ToString() + " ";
        //    }// ONE FACE DATA COMPLETED
        //    facesData += faceData + Environment.NewLine;
        //}
        foreach (var ver in vertices)
        {
            if (ver.index != -1)
            {
                string vertexData = "v " + ver.x.ToString() + " " + ver.z.ToString() + " " + ver.y.ToString();
                     // VERTEX DATA COMPLETED
                verticesData += vertexData + Environment.NewLine;
            }
        }
        foreach (var face in faces)
        {
            string faceData = "f ";
            var cornersTemp = new List<Vertex>();
            if (face.joined)
            {
                JoinedFaces Jf = joinedFaceList.Find(x => x.face1 == face || x.face2 == face);
                if (Jf != null)
                {
                    joinedFaceList.Remove(Jf);
                    cornersTemp = Jf.corners;
                }
                else continue;
            }
            else
            {
                cornersTemp = face.corners;
            }
            if(cornersTemp.Count==4) cornersTemp.Reverse();
            foreach (var ver in cornersTemp)
            {
                faceData += (ver.index + 1).ToString() + " ";
            }
            facesData += faceData + Environment.NewLine;
        }
        return verticesData + facesData;
    }

}
