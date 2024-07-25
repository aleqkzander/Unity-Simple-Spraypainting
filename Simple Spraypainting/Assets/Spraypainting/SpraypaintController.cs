using UnityEngine;

public class SpraypaintController : MonoBehaviour
{
    public Camera FreeflowCamera;
    public RenderTexture SpraypaintingTexture;
    public Color brushColor = Color.white;
    public int brushSize = 10;

    private Texture2D drawingTexture;

    void Start()
    {
        // Initialize the drawing texture
        drawingTexture = new Texture2D(SpraypaintingTexture.width, SpraypaintingTexture.height, TextureFormat.RGBA32, false);

        // Clear the render texture
        ClearRenderTexture();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DrawOnTexture();
        }
    }

    void ClearRenderTexture()
    {
        RenderTexture.active = SpraypaintingTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;
    }

    void DrawOnTexture()
    {
        Vector3 mousePos = Input.mousePosition;

        // Convert mouse position to texture coordinates
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

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

                    int x = (int)(uv.x * SpraypaintingTexture.width);
                    int y = (int)(uv.y * SpraypaintingTexture.height);

                    // Draw a brush at the mouse position
                    DrawBrush(x, y);

                    // Copy the drawing texture to the render texture
                    RenderTexture.active = SpraypaintingTexture;
                    Graphics.Blit(drawingTexture, SpraypaintingTexture);
                    RenderTexture.active = null;
                }
            }
            else
            {
                Debug.LogAssertion($"Please assign the Paintable-Script to {hit.collider.gameObject.name} to make it paintable");
            }
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
}