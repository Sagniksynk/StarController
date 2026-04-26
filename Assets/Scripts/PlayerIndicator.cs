using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerIndicator : MonoBehaviour
{
    [Header("Position")]
    public float heightAbovePlayer = 1.2f;

    [Header("Bob")]
    public float bobAmplitude = 0.15f;
    public float bobSpeed = 3f;

    [Header("Pulse")]
    public Color colorA = new Color(0.2f, 0.8f, 1f, 1f);
    public Color colorB = new Color(1f, 1f, 0f, 1f);
    public float pulseSpeed = 2f;

    [Header("Arrow Shape")]
    public float arrowWidth = 0.12f;
    public float arrowHeight = 0.35f;

    private LineRenderer lr;
    private Transform playerTransform;
    private float bobTimer;
    private float pulseTimer;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 8;
        lr.loop = true;
        lr.useWorldSpace = true;
        lr.startWidth = 0.06f;
        lr.endWidth = 0.06f;
        lr.sortingOrder = 10;
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        bobTimer += Time.deltaTime * bobSpeed;
        pulseTimer += Time.deltaTime * pulseSpeed;

        float bob = Mathf.Sin(bobTimer) * bobAmplitude;
        Vector3 origin = playerTransform.position + new Vector3(0f, heightAbovePlayer + bob, 0f);

        UpdateArrowShape(origin);
        UpdateColour();
    }

    void UpdateArrowShape(Vector3 origin)
    {
        float hw = arrowWidth * 0.5f;
        float ah = arrowHeight;
        float sh = ah * 0.45f;
        float sw = hw * 0.5f;

        Vector3[] points = new Vector3[8]
        {
            origin + new Vector3(0f,    0f,   0f),
            origin + new Vector3( hw,   sh,   0f),
            origin + new Vector3( sw,   sh,   0f),
            origin + new Vector3( sw,   ah,   0f),
            origin + new Vector3(-sw,   ah,   0f),
            origin + new Vector3(-sw,   sh,   0f),
            origin + new Vector3(-hw,   sh,   0f),
            origin + new Vector3(0f,    0f,   0f),
        };

        lr.SetPositions(points);
    }

    void UpdateColour()
    {
        float t = (Mathf.Sin(pulseTimer) + 1f) * 0.5f;
        Color c = Color.Lerp(colorA, colorB, t);
        lr.startColor = c;
        lr.endColor = c;
    }
}
