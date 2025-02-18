using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Pixyz.Import;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using Application = UnityEngine.Application;
using UnityEngine.Networking;

public class EditorTools : Editor
{
    //添加Metedata组件
    [MenuItem("Tools/ZZJ/AddMetadata")]
    public static void AddMetadata()
    {
        var selectObj = Selection.gameObjects;
        foreach (var item in selectObj)
        {
            foreach (var colliderObj in item.GetComponentsInChildren<Collider>())
            {
                if(!colliderObj.GetComponent<Metadata>())
                {
                    Metadata metadata = colliderObj.gameObject.AddComponent<Metadata>();
                    metadata.addOrSetProperty("ElementId", colliderObj.name);
                }
            }
        }
    }

    //修改名字
    [MenuItem("Tools/ZZJ/ChangeName")]
    public static void ChangeName()
    {
        var selectObj = Selection.gameObjects.OrderBy(_=>_.transform.GetSiblingIndex());//按照选中物体在Inspect的先后顺序
            int index = 20;
        foreach (var item in selectObj)
        {
            item.gameObject.name = "KBC-CP" + index.ToString().PadLeft(3,'0');
            index++;
            //foreach (var colliderObj in item.GetComponentsInChildren<Collider>())
            //{
            //    colliderObj.name = item.name + index.ToString();
            //    index++;
            //}
        }
    }
    //修改MetaData中的ElementId
    [MenuItem("Tools/ZZJ/ChangeMetadataName")]
    public static void ChangeMetadataName()
    {
        var selectObj = Selection.gameObjects;
        foreach (var item in selectObj)
        {
            Metadata metadata = item.GetComponent<Metadata>();
            metadata.addOrSetProperty("ElementId", item.name);
        }
    }

    

    //将xasset的内容打包到指定的Web文件夹下
    [MenuItem("Tools/ZZJ/CopyToSA_Web")]
    public static void CopyWebToStreamingAssets()
    {
        string originPath = Application.dataPath.Replace("Assets", "Bundles")+ "/WebGL";
        string targetPath = Application.streamingAssetsPath + "/LoadAssets/Bundles/WebGL";
        DeletaFile(targetPath);
        CopyFileAndDire(originPath, targetPath);
        Debug.Log("�ļ��������");
    }
    //将xasset的内容打包到指定的Windows文件夹下
    [MenuItem("Tools/ZZJ/CopyToSA_Win")]
    public static void CopyWinToStreamingAssets()
    {
        string originPath = Application.dataPath.Replace("Assets", "Bundles") + "/Windows";
        string targetPath = Application.streamingAssetsPath + "/LoadAssets/Bundles/Windows";
        DeletaFile(targetPath);
        CopyFileAndDire(originPath, targetPath);
        Debug.Log("�ļ��������");
    }
    private static void CopyFileAndDire(string oriPath,string tarPath)
    {
        if(!Directory.Exists(tarPath))
        {
            Directory.CreateDirectory(tarPath);
        }
        var files = Directory.GetFiles(oriPath);
        foreach (var item in files)
        {
            if (item.Contains(".meta"))
            {
                continue;
            }
            string desPath = Path.Combine(tarPath, Path.GetFileName(item));
            File.Copy(item, desPath, true);
        }
        var dires = Directory.GetDirectories(oriPath);

        foreach (var item in dires)
        {
            if (item == null)
            {
                return;
            }
            string desPath = Path.Combine(tarPath, Path.GetFileName(item));
            CopyFileAndDire(item, desPath);
        }
    }
    //清除文件
    private static void DeletaFile(string path)
    {
        var fileName = Directory.GetFiles(path);
        foreach (var item in fileName)
        {
            File.Delete(item);
        }
        var direcName = Directory.GetDirectories(path);
        foreach (var item in direcName)
        {
            if (item == null)
            {
                return;
            }
            string desPath = Path.Combine(path, Path.GetFileName(item));
            DeletaFile(desPath);
        }
    }

    //为选中的物体中有mesh的物体（避免空物体）添加Collider
    [MenuItem("Tools/ZZJ/AddCollider")]
    public static void AddCollider()
    {
        var selectObj = Selection.gameObjects;
        foreach (var item in selectObj)
        {
            foreach (var obj in item.GetComponentsInChildren<MeshRenderer>())
            {
                if(!obj.GetComponent<Collider>())
                {
                    obj.gameObject.AddComponent<MeshCollider>();
                }
                
            }
        }
        Debug.Log("成功添加Collider");
    }
    //保存Mesh
    [MenuItem("Tools/ZZJ/SaveMesh")]
    static void SaveMesh()
    {
        var obj = Selection.gameObjects;
        foreach (var item in obj)
        {
            string name = item.name;
            Mesh mf = item.GetComponent<MeshFilter>().mesh;

            if (mf != null)
            {
                AssetDatabase.CreateAsset(mf, "Assets/1Add/CombineMesh/" + name + ".asset");
                item.GetComponent<MeshFilter>().mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/1Add/CombineMesh/" + name + ".asset");

            }
            else
            {
                Debug.Log(name + "出错了");
            }

        }
        Debug.Log("mesh保存完成");
    }
}