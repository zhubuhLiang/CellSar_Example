using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JohnsonSolid
{
    //This Class provides functions for reading 3D-modeling data of Johnson solids from .WRL files and making JohnSonSolid mesh in Unity. 

    publis string johnsonFileLocation;
    private static List<Vector3> vertices = new List<Vector3>();
    private static List<int> triangles = new List<int>();
    private static int vertexIndex =0;
    private static MeshRenderer meshRender;
    private static MeshFilter meshFilter;
    private static GameObject solidObject;


    public static void MakeChosenSolid(int num, Vector3 pos)
    {
        string text = JohnsonSolidText(num);
        MakeSolid(text, pos);
    }


    static void JohnsonSolidText(int num)
    {
        string name = num < 100 ? "0" + num.ToString() : num.ToString();
        return System.IO.File.ReadAllText(johnsonFileLocation + name + ".wrl").Trim();
    }


    static void MakeSolid(string text, Vector3 pos)
    {
        List<Vector3> points = Points(PointText(text));
        List<int[]> faces = Faces(FaceText(text));

        foreach (int[] face in faces)
        {
            List<int> faceTriangles = FaceTriangles(face);
            foreach (int pointIndex in faceTriangles)
            {
                vertices.Add(points[pointIndex] + pos);
                triangles.Add(vertexIndex);
                vertexIndex += 1;
            }
        }

        CreateMesh(pos);
        ClearMeshData();
    }


    public static List<Vector3> Points(string pointString)
    {
        char[] charsToTrim = { 'p', 'o', 'i', 'n', 't', '[', ']' };
        string pointsTrimed = pointString.Trim().Trim(charsToTrim);
        string[] pointCoordsOld = pointsTrimed.Split(',');
        ArraySegment<string> pointCoords = new ArraySegment<string>(pointCoordsOld, 0, pointCoordsOld.Length - 1);

        List<Vector3> points = new List<Vector3>();
        foreach (string pointCoordUntrimmed in pointCoords)
        {
            string pointCoord = pointCoordUntrimmed.Trim();
            string[] dimensions = pointCoord.Split(' ');
            Vector3 point = new Vector3();
            int count = 0;

            foreach (string dimensionString in dimensions)
            {
                float dimension = float.Parse(dimensionString.Trim());
                point[count] = dimension;
                count += 1;
            }
            points.Add(point);
        }

        return points;
    }


    public static string PointText(string text)
    {
        int pointStartIndex = text.IndexOf("point[");
        int pointEndIndex = text.IndexOf("]", pointStartIndex);
        return text.Substring(pointStartIndex, pointEndIndex - pointStartIndex + 1);
    }


    public static List<int[]> Faces(string coordIndexString)
    {
        char[] charsToTrim = { 'c', 'o', 'r', 'd', 'I', 'n', 'd', 'e', 'x', '[', ']' };
        string facesTrimmed = coordIndexString.Trim().Trim(charsToTrim);
        string[] facesOld = facesTrimmed.Split(new string[] { "-1," }, StringSplitOptions.None);
        ArraySegment<string> facesString = new ArraySegment<string>(facesOld, 0, facesOld.Length - 1);
        List<int[]> faces = new List<int[]>();

        char[] toTrim = { ' ', ',' };
        foreach (string faceString in facesString)
        {
            List<int> face = new List<int>();
            string[] pointIndicesString = faceString.Trim(toTrim).Split(',');
            int count = 0;
            foreach (string pointIndexString in pointIndicesString)
            {
                int pointIndex = int.Parse(pointIndexString.Trim());
                face.Add(pointIndex);
                count += 1;
            }
            faces.Add(face.ToArray());
        }
        return faces;
    }


    public static string FaceText(string text)
    {
        int pointStartIndex = text.IndexOf("point[");
        int pointEndIndex = text.IndexOf("]", pointStartIndex);
        int faceStartIndex = text.IndexOf("coordIndex[", pointEndIndex);
        int faceEndIndex = text.IndexOf("]", faceStartIndex);
        return text.Substring(faceStartIndex, faceEndIndex - faceStartIndex + 1);
    }


    public static List<int> FaceTriangles( int[]face)
    {
        List<int> faceTris = new List<int>();
        int numOfTriangles = face.Length - 2;
        for(int i=0;i< numOfTriangles; i++)
        {
            faceTris.Add(face[0]);
            faceTris.Add(face[i+1]);
            faceTris.Add(face[i+2]);
        }
        return faceTris;
    }


    static void CreateMesh(Vector3 pos)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        solidObject = new GameObject();
        meshFilter = solidObject.AddComponent<MeshFilter>();
        meshRender = solidObject.AddComponent<MeshRenderer>();
        meshFilter.mesh = mesh;
        Vector4 colorVector = GetColorFromPos(pos);
        meshRender.material.color = new Color(colorVector.x, colorVector.y, colorVector.z, colorVector.w);
    }


    static void ClearMeshData()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
    }


    static Vector4 GetColorFromPos(Vector3 pos)
    {
        int num = (int)((pos.x / 2) * 10 + pos.z /2);
        int numR = num / 25;
        int numG = (num/5)%5;
        int numB = num % 5;
        int numA = num % 3;
        float r = numR * 0.18f + 0.23f ;
        float g = numG * 0.18f + 0.13f;
        float b = numB * 0.18f + 0.13f;
        float a = numA * 0.3f + 0.3f;

        return new Vector4(r,g,b,a);        
    }
    

}
