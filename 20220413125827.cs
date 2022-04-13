using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class Binary : MonoBehaviour
{
    private string _fileName = "";
    private string _trianglescount = "";

    private int _total;//?????¦È?????_total*3????????¦Æ??????
    private int _number;
    private BinaryReader _binaryReader;

    private List<Vector3> _vertices;
    private List<int> _triangles;

    /// <summary>
    ///???STL??????????????????
    /// </summary>
    private void GetFileNameAndTrianglesCount()
    {
        //string fullPath = Path.GetFullPath(AssetDatabase.GetAssetPath(target));
        //string fullPath = "D:\\UnityProject\\DrawMesh\\Assets\\ImportSTL\\????.stl";
        string fullPath = "F:\\Notes\\test.stl";
        using (BinaryReader br = new BinaryReader(File.Open(fullPath, FileMode.Open)))
        {
            _fileName = Encoding.UTF8.GetString(br.ReadBytes(80));//stl??????????§µ??80?????????????
            _trianglescount = BitConverter.ToInt32(br.ReadBytes(4), 0).ToString();//???4?????›¥?????????????????
        }
    }
    /// <summary>
    /// ????STL??????
    /// </summary>
    private void CreateInstance()
    {
        //string fullPath = "D:\\UnityProject\\DrawMesh\\Assets\\ImportSTL\\????.stl";
        string fullPath = "F:\\Notes\\test.stl";
        int gameObjectCount = 60000;//????????????§Ñ??????????Unity?§Ö???Mesh????????????65000????

        _total = int.Parse(_trianglescount);
        _number = 0;
        _binaryReader = new BinaryReader(File.Open(fullPath, FileMode.Open));

        //?????84?????
        _binaryReader.ReadBytes(84);

        _vertices = new List<Vector3>();//?›¥?????¦Æ???????
        _triangles = new List<int>();//?›¥??????????

        while (_number < _total)
        {
            byte[] bytes;
            //?50???????ï…?›¥???????¦Å?????????????????????
            bytes = _binaryReader.ReadBytes(50);

            if (bytes.Length < 50)
            {
                _number += 1;
                continue;
            }
            //??????????????¦Å????????????????
            Vector3 vec1 = new Vector3(BitConverter.ToSingle(bytes, 12), BitConverter.ToSingle(bytes, 16), BitConverter.ToSingle(bytes, 20));
            Vector3 vec2 = new Vector3(BitConverter.ToSingle(bytes, 24), BitConverter.ToSingle(bytes, 28), BitConverter.ToSingle(bytes, 32));
            Vector3 vec3 = new Vector3(BitConverter.ToSingle(bytes, 36), BitConverter.ToSingle(bytes, 40), BitConverter.ToSingle(bytes, 44));

            _vertices.Add(vec1);
            _vertices.Add(vec2);
            _vertices.Add(vec3);

            _number += 1;
        }

        //??????§Õ???§Ò??§µ???0??????????§Ø?????????
        for (int triNum = 0; triNum < _vertices.Count; triNum++)
        {
            int gameObhectIndex = triNum / gameObjectCount;//????????????????????Ž¤?
            _triangles.Add(triNum - gameObhectIndex * gameObjectCount);
        }

        for (int meshNumber = 0; meshNumber < _vertices.Count; meshNumber += gameObjectCount)
        {
            //????GameObject
            GameObject tem = new GameObject(Path.GetFileNameWithoutExtension(fullPath));
            tem.name = meshNumber.ToString();
            MeshFilter mf = tem.AddComponent<MeshFilter>();
            MeshRenderer mr = tem.AddComponent<MeshRenderer>();

            Mesh m = new Mesh();
            mr.name = meshNumber.ToString();
            if ((_vertices.Count - meshNumber) >= gameObjectCount)
            {
                m.vertices = _vertices.ToArray().Skip(meshNumber).Take(gameObjectCount).ToArray();
                m.triangles = _triangles.ToArray().Skip(meshNumber).Take(gameObjectCount).ToArray();
            }
            else
            {
                m.vertices = _vertices.ToArray().Skip(meshNumber).Take(_vertices.Count - meshNumber).ToArray();
                m.triangles = _triangles.ToArray().Skip(meshNumber).Take(_vertices.Count - meshNumber).ToArray();
            }
            m.RecalculateNormals();

            mf.mesh = m;
            mr.material = new Material(Shader.Find("Standard"));

            _binaryReader.Close();

            //Debug.Log(tem.name + ":???????? " + _vertices.Count);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetFileNameAndTrianglesCount();
        CreateInstance();
    }

    // Update is called once per frame
    void Update()
    {

    }
}



