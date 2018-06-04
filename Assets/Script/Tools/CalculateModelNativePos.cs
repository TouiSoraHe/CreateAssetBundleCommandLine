using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CalculateModelNativePos
{

    private static Camera _camera;

    public enum CalculateType
    {
        FullScreen,
        Real,
        Node
    }

    private enum ScreenHV
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// 计算模型自适应位置
    /// </summary>
    /// <param name="target">全屏模式：target传入根节点，真实尺寸模式：target无论传什么都返回Vector3.zero，定点模式：target传入聚焦目标的transform</param>
    /// <param name="camera">参与计算的相机对象</param>
    /// <param name="caululateType">FullScreen：全屏模式、Real：真实尺寸、Node：定点模式</param>
    /// <returns></returns>
    public static Vector3 Calculate(Transform target, Camera camera, Vector3 rootPosition, CalculateType caululateType)
    {
        if (target == null || camera == null)
        {
            Debug.LogError("函数参数target、camera不能为null");
            return Vector3.zero;
        }

        Vector3 RootPosition = rootPosition;

        _camera = camera;
        Vector3 center = Vector3.zero;
        Renderer[] renders = target.GetComponentsInChildren<Renderer>();
        foreach (Renderer child in renders)
        {
            center += child.bounds.center;
        }
        int renderCount = renders.Length;
        center = new Vector3(center.x / renderCount, center.y / renderCount, center.z / renderCount);
        Bounds bounds = new Bounds(center, Vector3.zero);

        List<float> boundsX = new List<float>();
        List<float> boundsY = new List<float>();
        List<float> boundsZ = new List<float>();
        foreach (Renderer child in renders)
        {
            boundsX.Add(child.bounds.min.x);
            boundsY.Add(child.bounds.min.y);
            boundsZ.Add(child.bounds.min.z);
            boundsX.Add(child.bounds.max.x);
            boundsY.Add(child.bounds.max.y);
            boundsZ.Add(child.bounds.max.z);
        }

        //-测试代码------------------------------------------------------
        //List<Vector3> matrix = new List<Vector3>();
        //matrix.Add(new Vector3(boundsX.Min(), boundsY.Min(), boundsZ.Min()));
        //matrix.Add(new Vector3(boundsX.Max(), boundsY.Min(), boundsZ.Min()));
        //matrix.Add(new Vector3(boundsX.Max(), boundsY.Max(), boundsZ.Min()));
        //matrix.Add(new Vector3(boundsX.Min(), boundsY.Max(), boundsZ.Min()));
        //matrix.Add(new Vector3(boundsX.Min(), boundsY.Min(), boundsZ.Max()));
        //matrix.Add(new Vector3(boundsX.Max(), boundsY.Min(), boundsZ.Max()));
        //matrix.Add(new Vector3(boundsX.Max(), boundsY.Max(), boundsZ.Max()));
        //matrix.Add(new Vector3(boundsX.Min(), boundsY.Max(), boundsZ.Max()));
        //foreach (Vector3 item in matrix)
        //{
        //    bounds.Encapsulate(item);
        //}
        //GameObject gameObj = new GameObject();
        //BoxCollider boxCollider = gameObj.AddComponent<BoxCollider>();
        //boxCollider.center = bounds.center;// - target.position;
        //boxCollider.size = bounds.size;
        ////gameObj.transform.position = target.position;

        //Vector3 boxCollideCenter = bounds.center;// - target.position;
        //Vector3 boxCollideCenter = bounds.center;

        //GameObject centerObj = new GameObject();
        //centerObj.transform.position = boxCollideCenter;
        //----------------------------------------------------------------

        Vector3 boxCollideCenter = bounds.center;
        //模型近平面 的 左下角及右上角的点
        Vector3 leftDown = new Vector3(boundsX.Min(), boundsY.Min(), boundsZ.Min());
        Vector3 rightUp = new Vector3(boundsX.Max(), boundsY.Max(), boundsZ.Min());

        Vector3 caululatePosition = GetModelFinalPosition(leftDown, rightUp, boxCollideCenter);

        Vector3 finalPosition = Vector3.zero;
        switch (caululateType)
        {
            case CalculateType.FullScreen:
                finalPosition = caululatePosition;
                break;
            case CalculateType.Real:
                finalPosition = Vector3.zero;
                break;
            case CalculateType.Node:
                finalPosition = caululatePosition;
                break;
        }
        //gameObj.transform.position = finalPosition;

        //由于提取中心点boxColliderCenter时 相当于是默认将模型放置到0，0，0点来进行计算，所以最后计算结果加回这个初始坐标
        return finalPosition + RootPosition;
    }

    /// <summary>
    /// 计算模型自适应的位置
    /// </summary>
    /// <param name="leftDown">模型距离相机最近平面左下角的点</param>
    /// <param name="rightUp">模型距离相机最近平面右上角的点</param>
    /// <param name="boxCenter">模型boxCollider中心点</param>
    /// <returns></returns>
    static Vector3 GetModelFinalPosition(Vector3 leftDown, Vector3 rightUp, Vector3 boxCenter)
    {
        Vector3 center = new Vector3((leftDown.x + rightUp.x) * 0.5f, (leftDown.y + rightUp.y) * 0.5f, leftDown.z);

        Vector2 leftPoint = _camera.WorldToScreenPoint(new Vector3(leftDown.x, 0, boxCenter.z));
        Vector2 rightPoint = _camera.WorldToScreenPoint(new Vector3(rightUp.x, 0, boxCenter.z));
        Vector3 finalPosHorizontal = Vector3.zero;
        finalPosHorizontal = CalculateFinalPos(leftPoint, rightPoint, center, boxCenter, ScreenHV.Horizontal);

        Vector2 upPoint = _camera.WorldToScreenPoint(new Vector3(0, rightUp.y, boxCenter.z));
        Vector2 downPoint = _camera.WorldToScreenPoint(new Vector3(0, leftDown.y, boxCenter.z));
        Vector3 finalPosVertical = Vector3.zero;
        finalPosVertical = CalculateFinalPos(downPoint, upPoint, center, boxCenter, ScreenHV.Vertical);
        if (finalPosHorizontal.z >= finalPosVertical.z)
            return finalPosHorizontal;
        else
            return finalPosVertical;
    }


    /// <summary>
    /// 根据屏幕横向和纵向的宽度，计算模型最终的位置
    /// </summary>
    /// <param name="minPos">横向或纵向的最小值</param>
    /// <param name="maxPos">横向或纵向的最大值</param>
    /// <param name="forwardPlaneCenter">模型距离相机最近的面的中心点</param>
    /// <param name="boxCenter">boxCollider中心点</param>
    /// <param name="screenHV">屏幕横纵屏枚举</param>
    /// <returns></returns>
    static Vector3 CalculateFinalPos(Vector2 minPos, Vector2 maxPos, Vector3 forwardPlaneCenter, Vector3 boxCenter, ScreenHV screenHV)
    {
        Vector3 finalPos = Vector3.zero;
        // finalZ/y = a/b;  模型中心距离相机的最终距离/模型中心平面与相机的当前距离 = 模型中心点平面宽或高/屏幕的宽或高
        float a = Vector2.Distance(maxPos, minPos);
        Plane plane = new Plane(-_camera.transform.forward, boxCenter);
        //模型中心平面到相机的距离 无论正负在屏幕的投影都是相同的，目的是模型显示在相机前面，所以距离直接取绝对值
        float y = Mathf.Abs(plane.GetDistanceToPoint(_camera.transform.position));
        float b = Screen.width;
        switch (screenHV)
        {
            case ScreenHV.Horizontal:
                b = Screen.width;
                break;
            case ScreenHV.Vertical:
                b = Screen.height;
                break;
        }

        //finalDis*6/5 相当于模型占屏幕比例 5/6
        float finalDis = a * y / b * 6 / 5;

        if (finalDis < _camera.nearClipPlane)
            finalDis = _camera.nearClipPlane;

        //计算模型gameObject的最终worldPos
        finalPos = _camera.transform.position + _camera.transform.forward * (finalDis);
        finalPos -= boxCenter;
        Vector3 fix = (boxCenter - forwardPlaneCenter).normalized;
        //由于模型到相机的距离是基于中心点计算，但最终效果是使模型的最近平面显示效果正确，所以将模型在中心点到最近面中心点的方向上移动 两个中心点距离单位的长度
        finalPos += Vector3.Distance(forwardPlaneCenter, boxCenter) * fix;

        return finalPos;
    }


    /// <summary>
    /// 获取boxCollider的8个顶点
    /// </summary>
    /// <param name="boxcollider"></param>
    /// <returns></returns>
    Vector3[] GetBoxColliderVertexPositions(BoxCollider boxcollider)
    {
        var vertices = new
Vector3[8];
        //下面4个点
        vertices[0] = boxcollider.transform.TransformPoint(boxcollider.center + new
Vector3(boxcollider.size.x, -boxcollider.size.y, boxcollider.size.z) * 0.5f);

        vertices[1] = boxcollider.transform.TransformPoint(boxcollider.center + new
Vector3(-boxcollider.size.x, -boxcollider.size.y, boxcollider.size.z) * 0.5f);

        vertices[2] = boxcollider.transform.TransformPoint(boxcollider.center + new
Vector3(-boxcollider.size.x, -boxcollider.size.y, -boxcollider.size.z) * 0.5f);

        vertices[3] = boxcollider.transform.TransformPoint(boxcollider.center + new
Vector3(boxcollider.size.x, -boxcollider.size.y, -boxcollider.size.z) * 0.5f);

        //上面4个点
        vertices[4] = boxcollider.transform.TransformPoint(boxcollider.center + new
Vector3(boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f);

        vertices[5] = boxcollider.transform.TransformPoint(boxcollider.center + new
Vector3(-boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f);

        vertices[6] = boxcollider.transform.TransformPoint(boxcollider.center + new
Vector3(-boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f);

        vertices[7] = boxcollider.transform.TransformPoint(boxcollider.center + new
Vector3(boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f);
        return
vertices;

    }
}
