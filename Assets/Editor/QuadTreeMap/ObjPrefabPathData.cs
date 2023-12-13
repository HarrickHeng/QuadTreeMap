using UnityEngine;

public class ObjPrefabPathData
{
    public readonly GameObject obj;
    private readonly ObjPrefabData quadTreeObjPrefabPath;

    public string ObjPrefabPath
    {
        get => quadTreeObjPrefabPath.prefabPath;
        set => quadTreeObjPrefabPath.prefabPath = value;
    }

    public Vector3 ObjModelSize
    {
        get => quadTreeObjPrefabPath.modelSize;
        set => quadTreeObjPrefabPath.modelSize = value;
    }

    public string objPrefabPathSimple;

    public ObjPrefabPathData(GameObject obj, string prefabPath)
    {
        this.obj = obj;
        quadTreeObjPrefabPath = obj.GetComponent<ObjPrefabData>();
        if (quadTreeObjPrefabPath == null)
            quadTreeObjPrefabPath = obj.AddComponent<ObjPrefabData>();
        quadTreeObjPrefabPath.prefabPath ??= string.Empty;
        if (!string.IsNullOrEmpty(prefabPath))
        {
            quadTreeObjPrefabPath.prefabPath = prefabPath;
        }
        if (quadTreeObjPrefabPath.prefabPath.IndexOf('/') == -1 ||
            quadTreeObjPrefabPath.prefabPath.IndexOf('.') == -1) return;
        var temp = quadTreeObjPrefabPath.prefabPath.Split('/');
        var temp2 = temp[^1].Split('.');
        objPrefabPathSimple = temp2[^2];
    }
}