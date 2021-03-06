/***********************************************************************
        filename:   RectMaskEffectUpdateBToT.cs
        created:    2012.04.28
        author:     Twj

        purpose:    矩形从下到上遮罩效果的更新
*************************************************************************/

using UnityEngine;
using System.Collections;

public class RectMaskEffectUpdateBToT : RectMaskEffectUpdateSideToSide
{
    // public functions

    // 构造函数
    // @params: startTime - 遮罩效果开始时间
    // @params: totalTime - 遮罩效果总时间
    // @params: halfHeight - 矩形宽的一半
    // @params: halfWidth - 矩形高的一半
    public RectMaskEffectUpdateBToT(
            float startTime,
            float totalTime,
            float halfHeight,
            float halfWidth )
    : base( startTime, totalTime, halfHeight, halfWidth )
    {
        arrIndicesFirst = new int[]
                            {
                                4, 5, 6,
                            };
    
        arrIndicesAfter = new int[]
                            {
                                0, 3, 4, 5, 6, 7,
                            };
    }
    
    // 更新遮罩效果
    // @params: mesh - 遮罩网格
    public override void UpdateEffect( Mesh mesh )
    {
        base.UpdateEffect( mesh );

        // 更新顶点数据
        Vector3[] vertices = mesh.vertices;
        foreach ( int index in arrIndices )
        {
            vertices[index].y = (deltaTime - halfTotalTime) / halfTotalTime * halfHeight;
        }

        mesh.vertices = vertices;
    }
    /////////////////////////////////////////////////////////////////////
}