using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneObjStatus
{
    Loading, //加载中
    Loaded //加载完毕
}

public class SceneObjData
{
    public ObjData objData;
    public SceneObjStatus status;
    public GameObject obj;

    public SceneObjData(ObjData data)
    {
        objData = data;
        obj = null;
    }
}

public class ObjManager : MonoBehaviour
{
    public static ObjManager Instance;
    private Dictionary<int, SceneObjData> mActiveSceneObjDatas = new Dictionary<int, SceneObjData>();
    private List<int> mUnLoadUIDs = new List<int>();

    private void Awake()
    {
        Instance = this;
    }

    public void LoadAsync(ObjData objData)
    {
        if (mActiveSceneObjDatas.ContainsKey(objData.uid))
            return;
        StartCoroutine(LoadObj(objData));
    }

    public void Unload(int uid)
    {
        if (mActiveSceneObjDatas.ContainsKey(uid) && mUnLoadUIDs.Contains(uid) == false)
        {
            mUnLoadUIDs.Add(uid);
        }

        for (int i = 0, len = mUnLoadUIDs.Count; i < len; ++i)
        {
            if (i >= mUnLoadUIDs.Count) break;
            if (mActiveSceneObjDatas[mUnLoadUIDs[i]] != null &&
                mActiveSceneObjDatas[mUnLoadUIDs[i]].status != SceneObjStatus.Loaded) continue;
            Destroy(mActiveSceneObjDatas[mUnLoadUIDs[i]].obj);
            mActiveSceneObjDatas.Remove(mUnLoadUIDs[i]);
            mUnLoadUIDs.RemoveAt(i--);
        }
    }

    IEnumerator LoadObj(ObjData obj)
    {
        var sceneObjData = new SceneObjData(obj)
        {
            status = SceneObjStatus.Loading
        };
        mActiveSceneObjDatas.Add(obj.uid, sceneObjData);
        var request = Resources.LoadAsync<GameObject>(obj.resPath);
        yield return request;
        var resObj = request.asset as GameObject;
        yield return new WaitUntil(() => resObj != null);
        sceneObjData.status = SceneObjStatus.Loaded;
        SetObjTransform(resObj, sceneObjData);
    }

    private void SetObjTransform(GameObject prefab, SceneObjData sceneObj)
    {
        var targetScene = SceneManager.GetSceneByName(SceneManager.GetActiveScene().name);
        if (targetScene.IsValid())
        {
            sceneObj.obj = Instantiate(prefab);
            SceneManager.MoveGameObjectToScene(sceneObj.obj, targetScene);
            sceneObj.obj.transform.position = sceneObj.objData.pos;
            sceneObj.obj.transform.rotation = sceneObj.objData.rot;
            sceneObj.obj.transform.localScale = sceneObj.objData.scale;
        }
        else
        {
            Debug.LogError($"Target scene not found: {SceneManager.GetActiveScene().name}");
        }
    }
}