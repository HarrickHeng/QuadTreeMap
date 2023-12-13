using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 八叉树场景空间管理器
/// </summary>
public class QuadTreeMapManager : MonoBehaviour
{
    public float InitialWorldSize = 1000;
    public float MinNodeSide = 1f;
    public float LoosenessVal = 1.2f;
    private BoundsOctree<ObjData> mTree;
    private ObjData[] mCurrMapData;
    public Camera Camera;

    private void Start()
    {
        // var mapDataJsonPath = $"QuadTreeMapJsonData/{SceneManager.GetActiveScene().name}";
        // StartCoroutine(LoadMapData(mapDataJsonPath));
        
        // Test
        mTree = new BoundsOctree<ObjData>(InitialWorldSize, transform.position, MinNodeSide, LoosenessVal);
        for (int i = 0; i < 1000; i++)
        {
            var randomPosition = new Vector3(Random.Range(-1000f, 1000f), 0, Random.Range(-1000f, 1000f));
            var randomScale = Vector3.one * Random.Range(0.5f, 2f);
            var objData = new ObjData("Cube", randomPosition, Quaternion.identity, randomScale, Vector3.one)
                {
                    uid = i
                };
            mTree.Add(objData, objData.GetObjBounds());
        }
    }

    IEnumerator LoadMapData(string path)
    {
        var request = Resources.LoadAsync<TextAsset>(path);
        yield return request;
        if (request == null)
        {
            Debug.LogError($"读取地图数据失败① {path}");
            yield break;
        }

        var jsonAsset = request.asset as TextAsset;
        if (jsonAsset == null)
        {
            Debug.LogError($"读取地图数据失败② {path}");
            yield break;
        }

        var datas = JObject.Parse(jsonAsset.text)["objDatas"];
        if (datas == null)
        {
            Debug.LogError($"读取地图数据失败③ {path}");
            yield break;
        }

        mCurrMapData = datas.ToObject<ObjData[]>();
        mTree = new BoundsOctree<ObjData>(InitialWorldSize, transform.position, MinNodeSide, LoosenessVal);
        if (mCurrMapData == null)
        {
            Debug.LogError($"读取地图数据失败④ {path}");
            yield break;
        }

        for (int i = 0, len = mCurrMapData.Length; i < len; ++i)
        {
            var objData = mCurrMapData[i];
            objData.uid = i;
            mTree.Add(objData, objData.GetObjBounds());
        }
    }

    public void Update()
    {
        mTree?.TriggerMove(Camera);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (mTree == null) return;
        mTree.DrawAllBounds();
        mTree.DrawAllObjects();
        // mTree.DrawCollisionChecks();
    }
#endif
}