using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPefectShadows : MonoBehaviour
{
    private RenderTexture occludersFBO;
    private Texture2D occludersTexture;
    private Camera occluderCamera;
    private int lightSize = 256; // 默认大小
    private Vector2 lightPosition; // 光源位置 (mx, my)

    void Start()
    {
        // 创建RenderTexture (类似FBO)
        occludersFBO = new RenderTexture(lightSize, lightSize, 0, RenderTextureFormat.ARGB32);
        occludersFBO.Create();

        // 创建一个专门用于渲染遮挡物体的相机
        occluderCamera = new GameObject("Occluder Camera").AddComponent<Camera>();
        occluderCamera.targetTexture = occludersFBO;
        occluderCamera.clearFlags = CameraClearFlags.SolidColor;
        occluderCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        occluderCamera.orthographic = true;
        occluderCamera.orthographicSize = lightSize / 2f;
        occluderCamera.depth = -10; // 确保它是最底层的相机
        occluderCamera.cullingMask = LayerMask.GetMask("Obstacle"); // 只渲染遮挡物体层
        occluderCamera.enabled = false; // 不自动渲染
    }

    public void RenderOccluders(Vector2 lightPosition)
    {
        // 设置光源位置
        this.lightPosition = lightPosition;

        // 设置相机位置，使光源居中
        occluderCamera.transform.position = new Vector3(
            lightPosition.x,
            lightPosition.y,
            -10 // 相机Z位置
        );

        // 渲染到RenderTexture
        occluderCamera.Render();

        // 如果需要，可以将RenderTexture转换为Texture2D
        if (occludersTexture == null || occludersTexture.width != lightSize || occludersTexture.height != lightSize)
        {
            occludersTexture = new Texture2D(lightSize, lightSize, TextureFormat.ARGB32, false);
        }

        RenderTexture.active = occludersFBO;
        occludersTexture.ReadPixels(new Rect(0, 0, lightSize, lightSize), 0, 0);
        occludersTexture.Apply();
        RenderTexture.active = null;

        // // 如果需要翻转Y轴（类似LibGDX的flip操作）
        // // 可以通过Matrix来实现
        // Matrix4x4 flipMatrix = Matrix4x4.identity;
        // flipMatrix[1, 1] = -1f;
        // flipMatrix[3, 1] = lightSize;
        // occludersTexture.texGenMatrix = flipMatrix;
    }

    void OnDestroy()
    {
        // 释放资源
        if (occludersFBO != null)
        {
            occludersFBO.Release();
            Destroy(occludersFBO);
        }

        if (occludersTexture != null)
        {
            Destroy(occludersTexture);
        }

        if (occluderCamera != null)
        {
            Destroy(occluderCamera.gameObject);
        }
    }
}
