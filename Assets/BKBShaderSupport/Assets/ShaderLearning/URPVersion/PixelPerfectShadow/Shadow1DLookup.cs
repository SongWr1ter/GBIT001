using UnityEngine;

public class Shadow1DLookup : MonoBehaviour
{
    public RenderTexture occludersTexture; // 从之前步骤中获取的遮挡纹理
    public int lightSize = 256; // 阴影贴图大小
    public Material shadowLookupMaterial; // 使用上面的Shader创建的材质

    private RenderTexture shadowMapFBO;
    private Camera shadowMapCamera;

    void Start()
    {
        // 创建1D阴影贴图的RenderTexture
        shadowMapFBO = new RenderTexture(lightSize, 1, 0, RenderTextureFormat.ARGB32);
        shadowMapFBO.wrapMode = TextureWrapMode.Repeat;
        shadowMapFBO.filterMode = FilterMode.Bilinear;
        shadowMapFBO.Create();

        // 创建一个专门用于渲染阴影查找纹理的相机
        shadowMapCamera = new GameObject("Shadow Map Camera").AddComponent<Camera>();
        shadowMapCamera.targetTexture = shadowMapFBO;
        shadowMapCamera.clearFlags = CameraClearFlags.SolidColor;
        shadowMapCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        shadowMapCamera.orthographic = true;
        shadowMapCamera.orthographicSize = lightSize / 2f;
        shadowMapCamera.depth = -10;
        shadowMapCamera.cullingMask = LayerMask.GetMask("Occluders");
        shadowMapCamera.enabled = false;

        // 设置材质参数
        shadowLookupMaterial.SetTexture("_OccludersTexture", occludersTexture);
        shadowLookupMaterial.SetVector("_Resolution", new Vector2(lightSize, lightSize));
        shadowLookupMaterial.SetFloat("_Threshold", 0.75f);
    }

    public void RenderShadowLookup(Vector2 lightPosition)
    {
        // 设置相机位置，使光源居中
        shadowMapCamera.transform.position = new Vector3(
            lightPosition.x,
            lightPosition.y,
            -10 // 相机Z位置
        );

        // 渲染到RenderTexture
        shadowMapCamera.Render();
    }

    void OnDestroy()
    {
        // 释放资源
        if (shadowMapFBO != null)
        {
            shadowMapFBO.Release();
            Destroy(shadowMapFBO);
        }

        if (shadowMapCamera != null)
        {
            Destroy(shadowMapCamera.gameObject);
        }
    }
}