using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Data.SqlClient;
using UnityEngine.Assertions.Must;

public class dataCollecter : MonoBehaviour
{
    SqlConnection con;
    SqlCommand cmd;
    string connectionString = "workstation id=ReductionWorkloadDb.mssql.somee.com;packet size=4096;" +
            "user id=Bilonel_SQLLogin_1;pwd=cpiakmox9q;data source=ReductionWorkloadDb.mssql.somee.com;" +
            "persist security info=False;initial catalog=ReductionWorkloadDb;" 
        /*+"Trust Server Certificate=True"*/;
    

    [SerializeField] int GamerID = 3;
    float RunTime = 0;


    [SerializeField] Vector2 mapSize;
    Vector2 corner;
    [SerializeField] Transform player;
    float[,] map;
    // Start is called before the first frame update
    void Start()
    {
        if (mapSize == null) mapSize = new Vector2(50, 50);
        if (player == null) player = GameObject.FindGameObjectWithTag("player").transform;
        map = new float[((int)mapSize.x), ((int)mapSize.y)];
        for (int i = 0; i < ((int)mapSize.x); i++)
            for (int k = 0; k < ((int)mapSize.y); k++)
                map[i, k] = 0;
        Transform transf = GameObject.Find("Terrain").transform;
        corner = new Vector2(transf.position.x, transf.position.z);

        con = new SqlConnection(connectionString);
    }
    // Update is called once per frame
    void Update()
    {
        RunTime += Time.deltaTime;
        int[] indices = getPosition();
        if (indices == null) return;
        map[indices[0], indices[1]] += Time.deltaTime;
    }

    int[] getPosition()
    {
        Vector2 playerPosition = new Vector2(player.position.x - corner.x, player.position.z - corner.y);

        int x=((int)playerPosition.x), y=((int)playerPosition.y);
        if (x > ((int)mapSize.x) - 1 || y > ((int)mapSize.y) - 1 || x < 0 || y < 0)
            return null;
        return new int[] { x, y };
    }
    private void OnApplicationQuit()
    {
        Texture2D t = new Texture2D(((int)mapSize.x), ((int)mapSize.y));
        float max = 0;
        for (int i = 0; i < ((int)mapSize.x); i++)
            for (int k = 0; k < ((int)mapSize.y); k++)
                if (map[i, k] > max) max = map[i, k];
        for (int i = 0; i < ((int)mapSize.x); i++)
        {
            for (int k = 0; k < ((int)mapSize.y); k++)
            {
                float x = map[i, k] * 1f / max;
                t.SetPixel(i,k,new Color(x,x,x));
            }
        }
        
        var bits =ImageConversion.EncodeToPNG(t);
        if(isExistInDb(GamerID))
        {
            userData oldData = GetDB(GamerID);
            float oldRunTime = oldData.runTime;
            Texture2D oldTexture = new Texture2D(((int)mapSize.x), ((int)mapSize.y));
            oldTexture.LoadImage(oldData.img);

            for (int i = 0; i < ((int)mapSize.x); i++)
            {
                for (int k = 0; k < ((int)mapSize.y); k++)
                {
                    float colorNew = t.GetPixel(i, k).r;
                    float colorOld = oldTexture.GetPixel(i, k).r;
                    float colorTotal = ((colorNew * RunTime) + (colorOld * oldRunTime))/(RunTime+oldRunTime);
                    oldTexture.SetPixel(i, k, new Color(colorTotal, colorTotal, colorTotal));
                }
            }

            UpdateDB(GamerID, ImageConversion.EncodeToPNG(oldTexture), oldRunTime + RunTime);

        }
        else InsertDB(GamerID, bits, RunTime);

    }
    void InsertDB(int id, byte[] img,float totalRunTime)
    {
        con.Open();
        cmd = new SqlCommand(
            "INSERT INTO yogunluk_verileri " +
            "Values(@oyuncu_id, @img,@RunTime)", con);

        cmd.Parameters.Add("@oyuncu_id",
           SqlDbType.Int).Value = id;

        cmd.Parameters.Add("@img",
            SqlDbType.Image, img.Length).Value = img;

        cmd.Parameters.Add("@RunTime",
            SqlDbType.Float).Value = totalRunTime;
        cmd.ExecuteNonQuery();
        con.Close();
    }
    void UpdateDB(int id, byte[] img,float totalRunTime)
    {
        con.Open();
        cmd = new SqlCommand(
            "UPDATE yogunluk_verileri " +
            "SET _data=@img, TotalRunTime=@RunTime " +
            "WHERE oyuncu_id=@oyuncu_id", con);

        cmd.Parameters.Add("@oyuncu_id",
           SqlDbType.Int).Value = id;

        cmd.Parameters.Add("@img",
            SqlDbType.Image, img.Length).Value = img;

        cmd.Parameters.Add("@RunTime",
            SqlDbType.Float).Value = totalRunTime;
        cmd.ExecuteNonQuery();
        con.Close();
    }
    userData GetDB(int id)
    {
        con.Open();
        cmd = new SqlCommand(
            "Select * From yogunluk_verileri " +
            "Where oyuncu_id=@oyuncu_id", con);

        cmd.Parameters.Add("@oyuncu_id",
           SqlDbType.Int).Value = id;

        //cmd.ExecuteNonQuery();
        userData user = new userData();
        using (SqlDataReader oReader = cmd.ExecuteReader())
        {
            while (oReader.Read())
            {
                if (id != (int)oReader.GetValue(0)) return new userData();
                user.img = (byte[])oReader.GetValue(1);
                double x =(double)oReader.GetValue(2);
                user.runTime = ((float)x);
            }

            con.Close();
        }

        return user;
    }
    bool isExistInDb(int id)
    {
        con.Open();
        cmd = new SqlCommand(
            "Select * From yogunluk_verileri " +
            "Where oyuncu_id=@oyuncu_id", con);

        cmd.Parameters.Add("@oyuncu_id",
           SqlDbType.Int).Value = id;

        cmd.ExecuteNonQuery();
        var res = cmd.ExecuteScalar() != null;

        con.Close();
        return res;
    }

    struct userData
    {
        public byte[] img;
        public float runTime;
    }

}
