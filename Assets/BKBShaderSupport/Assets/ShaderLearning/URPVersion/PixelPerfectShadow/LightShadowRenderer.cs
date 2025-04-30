using UnityEngine;

public class LightShadowRenderer : MonoBehaviour
{
    public RenderTexture shadowLookupTexture; // 1D阴影查找纹理
    public int lightSize = 256; // 光照区域大小
    public Material lightShadowMaterial; // 使用上面的Shader创建的材质
    public Color lightColor = Color.white; // 光源颜色

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // 设置材质参数
        lightShadowMaterial.SetTexture("_ShadowLookupTexture", shadowLookupTexture);
        lightShadowMaterial.SetVector("_Resolution", new Vector2(lightSize, lightSize));
        lightShadowMaterial.SetColor("_LightColor", lightColor);
    }

    public void RenderLightShadow(Vector2 lightPosition)
    {
        // 设置光源位置
        Vector3 lightWorldPosition = new Vector3(lightPosition.x, lightPosition.y, 0);

        // 创建一个四边形用于渲染光照区域
        Mesh quadMesh = MeshGenerator.CreateQuad(lightSize, lightSize);

        // 创建一个临时的GameObject来渲染光照区域
        GameObject lightQuad = new GameObject("Light Quad");
        lightQuad.transform.position = lightWorldPosition;
        lightQuad.transform.SetParent(transform);

        // 添加MeshFilter和MeshRenderer组件
        lightQuad.AddComponent<MeshFilter>().mesh = quadMesh;
        MeshRenderer meshRenderer = lightQuad.AddComponent<MeshRenderer>();
        meshRenderer.material = lightShadowMaterial;

        // 设置光照区域的UV
        meshRenderer.material.mainTextureScale = new Vector2(1, 1);

        // 渲染光照区域
        lightQuad.SetActive(true);

        // 在下一帧中销毁临时对象
        Destroy(lightQuad, 0.1f);
    }

    private class MeshGenerator
    {
        public static Mesh CreateQuad(float width, float height)
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(-width / 2, -height / 2, 0),
                new Vector3(width / 2, -height / 2, 0),
                new Vector3(width / 2, height / 2, 0),
                new Vector3(-width / 2, height / 2, 0)
            };
            int[] triangles = new int[]
            {
                0, 1, 2,
                2, 3, 0
            };
            Vector2[] uvs = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}