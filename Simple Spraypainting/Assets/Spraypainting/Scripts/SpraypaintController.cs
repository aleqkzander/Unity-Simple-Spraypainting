using System.IO;
using UnityEngine;

public class SpraypaintController : MonoBehaviour
{
    public Camera FreeflowCamera;
    public RenderTexture SpraypaintTexture;
    public Color brushColor = Color.white;
    public int brushSize = 1;

    private Texture2D drawingTexture;

    void Start()
    {
        // Initialize the drawing texture
        drawingTexture = new Texture2D(SpraypaintTexture.width, SpraypaintTexture.height, TextureFormat.RGBA32, false);

        // Clear the render texture
        ClearRenderTexture();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DrawOnTexture();
        }

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    SaveDrawing();
        //}
    }

    void ClearRenderTexture()
    {
        RenderTexture.active = SpraypaintTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;
    }

    void DrawOnTexture()
    {
        Vector3 mousePos = Input.mousePosition;

        // Convert mouse position to texture coordinates
        Ray ray = FreeflowCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the object has the Paintable script attached
            if (hit.collider.gameObject.GetComponent<Paintable>() != null)
            {
                Renderer renderer = hit.collider.GetComponent<Renderer>();
                MeshCollider meshCollider = hit.collider as MeshCollider;

                if (renderer != null && renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture != null && meshCollider != null)
                {
                    Vector2 uv = hit.textureCoord;

                    int x = (int)(uv.x * SpraypaintTexture.width);
                    int y = (int)(uv.y * SpraypaintTexture.height);

                    //Debug.Log($"Mouse Position: {mousePos}, UV: {uv}, Texture Coordinates: ({x}, {y})");

                    // Draw a brush at the mouse position
                    DrawBrush(x, y);

                    // Copy the drawing texture to the render texture
                    RenderTexture.active = SpraypaintTexture;
                    Graphics.Blit(drawingTexture, SpraypaintTexture);
                    RenderTexture.active = null;
                }
                else
                {
                    Debug.LogWarning("Renderer or MeshCollider missing, or material does not have a main texture.");
                }
            }
            else
            {
                Debug.LogWarning("Object is not paintable.");
            }
        }
        else
        {
            Debug.LogWarning("Raycast did not hit any object.");
        }
    }

    void DrawBrush(int x, int y)
    {
        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                if (i * i + j * j <= brushSize * brushSize) // Circle brush
                {
                    int pixelX = Mathf.Clamp(x + i, 0, drawingTexture.width - 1);
                    int pixelY = Mathf.Clamp(y + j, 0, drawingTexture.height - 1);
                    drawingTexture.SetPixel(pixelX, pixelY, brushColor);
                }
            }
        }
        drawingTexture.Apply();
    }

    public void SaveDrawing()
    {
        RenderTexture.active = SpraypaintTexture;
        Texture2D savedTexture = new(SpraypaintTexture.width, SpraypaintTexture.height, TextureFormat.RGBA32, false);
        savedTexture.ReadPixels(new Rect(0, 0, SpraypaintTexture.width, SpraypaintTexture.height), 0, 0);
        savedTexture.Apply();
        RenderTexture.active = null;

        byte[] bytes = savedTexture.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, "Drawing.png");
        File.WriteAllBytes(path, bytes);

        Debug.Log($"Saved drawing to {path}");
    }
}