using UnityEngine;

public class BouncingBall : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float gravityScale = 1.2f;
    [SerializeField] private Color ballColor = new Color(1f, 0.3f, 0.3f, 1f);

    private void Start()
    {
        CreateCamera();
        CreateBall();
        CreateBounds();
    }

    private void CreateCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cam = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }

        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
    }

    private void CreateBall()
    {
        GameObject ballObject = new GameObject("Bouncing Ball");
        ballObject.transform.position = Vector3.zero;

        SpriteRenderer spriteRenderer = ballObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateCircleSprite(256, ballColor);
        spriteRenderer.sortingOrder = 10;

        CircleCollider2D circleCollider = ballObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = 0.5f;
        circleCollider.sharedMaterial = new PhysicsMaterial2D { friction = 0f, bounciness = 1f };

        Rigidbody2D rb = ballObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.mass = 1f;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.velocity = new Vector2(speed, speed);
    }

    private void CreateBounds()
    {
        CreateBoundary("Left Wall", new Vector2(-8.2f, 0f), new Vector2(0.2f, 8.5f));
        CreateBoundary("Right Wall", new Vector2(8.2f, 0f), new Vector2(0.2f, 8.5f));
        CreateBoundary("Top Wall", new Vector2(0f, 4.8f), new Vector2(8.5f, 0.2f));
        CreateBoundary("Bottom Wall", new Vector2(0f, -4.8f), new Vector2(8.5f, 0.2f));
    }

    private void CreateBoundary(string name, Vector2 position, Vector2 size)
    {
        GameObject boundaryObject = new GameObject(name);
        boundaryObject.transform.position = position;

        BoxCollider2D boxCollider = boundaryObject.AddComponent<BoxCollider2D>();
        boxCollider.size = size;

        SpriteRenderer spriteRenderer = boundaryObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateBoxSprite(Color.gray);
        spriteRenderer.color = Color.gray;
        spriteRenderer.sortingOrder = 1;
    }

    private Sprite CreateCircleSprite(int resolution, Color color)
    {
        Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Bilinear;

        Color[] pixels = new Color[resolution * resolution];
        float radius = resolution * 0.5f - 4f;
        float center = resolution * 0.5f;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                pixels[y * resolution + x] = distance <= radius ? color : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), 100f);
    }

    private Sprite CreateBoxSprite(Color color)
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        texture.SetPixels(new[] { color, color, color, color });
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 100f);
    }
}
