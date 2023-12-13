using UnityEngine;

public static class GameObjectUtil
{
    #region CheckBoundIsInCamera 判断摄像头视锥

    /// <summary>
    /// 判断摄像头视锥
    /// </summary>
    /// <param name="bound"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static bool CheckBoundIsInCamera(this Bounds bound, Camera camera)
    {
        var worldPos = Vector4.one;
        var code = 63;
        for (var i = -1; i <= 1; i += 2)
        {
            for (var j = -1; j <= 1; j += 2)
            {
                for (var k = -1; k <= 1; k += 2)
                {
                    worldPos.x = bound.center.x + i * bound.extents.x;
                    worldPos.y = bound.center.y + j * bound.extents.y;
                    worldPos.z = bound.center.z + k * bound.extents.z;

                    code &= ComputeOutCode(camera.projectionMatrix * camera.worldToCameraMatrix * worldPos);
                }
            }
        }

        return code == 0;

        int ComputeOutCode(Vector4 projectionPos)
        {
            var outCode = 0;
            if (projectionPos.x < -projectionPos.w) outCode |= 1;
            if (projectionPos.x > projectionPos.w) outCode |= 2;
            if (projectionPos.y < -projectionPos.w) outCode |= 4;
            if (projectionPos.y > projectionPos.w) outCode |= 8;
            if (projectionPos.z < -projectionPos.w) outCode |= 16;
            if (projectionPos.z > projectionPos.w) outCode |= 32;
            return outCode;
        }
    }

    #endregion

    #region GetInstanceBounds 获取游戏对象实例的边界框

    /// <summary>
    /// 获取游戏对象实例的边界框
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public static bool GetInstanceBounds(this GameObject gameObject, out Bounds bounds)
    {
        var renderers = gameObject.GetComponentsInChildren<Renderer>();

        var prefabBounds = new Bounds();
        foreach (var renderer in renderers)
        {
            if (renderer == null) continue;
            if (prefabBounds.size == Vector3.zero)
            {
                prefabBounds = renderer.bounds;
            }
            else
            {
                prefabBounds.Encapsulate(renderer.bounds);
            }
        }

        if (prefabBounds.size != Vector3.zero)
        {
            bounds = prefabBounds;
            return true;
        }

        bounds = default;
        return false;
    }

    #endregion
}