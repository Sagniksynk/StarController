using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing, Returning }

    [Header("Patrol Zone")]
    public BoxCollider2D patrolZone;
    public float patrolSpeed = 2f;

    [Header("Chase")]
    public float chaseSpeed = 4f;
    public float returnSnapDistance = 0.3f;

    [Header("Directional Sprites")]
    public Sprite[] southFrames;
    public Sprite[] swFrames;
    public Sprite[] westFrames;
    public Sprite[] nwFrames;
    public Sprite[] northFrames;

    [Header("Animation")]
    public float frameRate = 8f;

    private EnemyState currentState = EnemyState.Patrolling;

    private Vector2[] corners;
    private float[] segmentLengths;
    private float totalPerimeter;
    private float perimeterT;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Sprite[] currentFrames;
    private int currentFrame;
    private float frameTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentFrames = southFrames;
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        BuildPerimeter();
        perimeterT = GetClosestPerimeterT((Vector2)transform.position);
    }

    void BuildPerimeter()
    {
        if (patrolZone == null)
        {
            Debug.LogWarning("[EnemyController] No patrolZone assigned!", this);
            return;
        }

        Bounds b = patrolZone.bounds;
        corners = new Vector2[4]
        {
            new Vector2(b.min.x, b.min.y),
            new Vector2(b.max.x, b.min.y),
            new Vector2(b.max.x, b.max.y),
            new Vector2(b.min.x, b.max.y)
        };

        segmentLengths = new float[4];
        totalPerimeter = 0f;
        for (int i = 0; i < 4; i++)
        {
            segmentLengths[i] = Vector2.Distance(corners[i], corners[(i + 1) % 4]);
            totalPerimeter += segmentLengths[i];
        }
    }

    float GetClosestPerimeterT(Vector2 pos)
    {
        float bestT = 0f;
        float bestDist = float.MaxValue;
        float accum = 0f;

        for (int i = 0; i < 4; i++)
        {
            Vector2 a = corners[i];
            Vector2 b = corners[(i + 1) % 4];
            Vector2 ab = b - a;
            float segLen = segmentLengths[i];
            float t = Mathf.Clamp01(Vector2.Dot(pos - a, ab) / (segLen * segLen));
            float dist = Vector2.Distance(pos, a + ab * t);

            if (dist < bestDist)
            {
                bestDist = dist;
                bestT = accum + t * segLen;
            }

            accum += segLen;
        }

        return bestT;
    }

    Vector2 GetPositionAtT(float t)
    {
        t = ((t % totalPerimeter) + totalPerimeter) % totalPerimeter;

        for (int i = 0; i < 4; i++)
        {
            if (t <= segmentLengths[i])
                return Vector2.Lerp(corners[i], corners[(i + 1) % 4], t / segmentLengths[i]);
            t -= segmentLengths[i];
        }

        return corners[0];
    }

    void Update()
    {
        if (player == null || corners == null) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        bool playerInZone = patrolZone != null && patrolZone.OverlapPoint((Vector2)player.position);

        switch (currentState)
        {
            case EnemyState.Patrolling:
                if (playerInZone)
                    currentState = EnemyState.Chasing;
                else
                    PatrolPerimeter();
                break;

            case EnemyState.Chasing:
                if (!playerInZone)
                {
                    perimeterT = GetClosestPerimeterT((Vector2)transform.position);
                    currentState = EnemyState.Returning;
                }
                else
                {
                    MoveTowards(player.position, chaseSpeed);
                }
                break;

            case EnemyState.Returning:
                if (playerInZone)
                {
                    currentState = EnemyState.Chasing;
                }
                else
                {
                    Vector2 closestOnEdge = GetPositionAtT(perimeterT);
                    MoveTowards(closestOnEdge, patrolSpeed);

                    if (Vector2.Distance(transform.position, closestOnEdge) < returnSnapDistance)
                        currentState = EnemyState.Patrolling;
                }
                break;
        }

        sr.color = currentState == EnemyState.Chasing ? Color.red : Color.white;
        TickAnimation(rb.linearVelocity);
    }

    void PatrolPerimeter()
    {
        float lookahead = patrolSpeed * Time.deltaTime;
        perimeterT = (perimeterT + lookahead) % totalPerimeter;
        Vector2 target = GetPositionAtT(perimeterT);
        MoveTowards(target, patrolSpeed);
    }

    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * speed;
    }

    void TickAnimation(Vector2 velocity)
    {
        bool moving = velocity.sqrMagnitude > 0.01f;

        if (moving)
            UpdateDirectionFromVelocity(velocity.normalized);

        if (!moving)
        {
            if (currentFrames != null && currentFrames.Length > 0)
                sr.sprite = currentFrames[0];
            currentFrame = 0;
            frameTimer = 0f;
            return;
        }

        frameTimer += Time.deltaTime;
        if (frameTimer >= 1f / frameRate)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % currentFrames.Length;
            sr.sprite = currentFrames[currentFrame];
        }
    }

    void UpdateDirectionFromVelocity(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        sr.flipX = false;

        if      (angle >= 255f && angle < 285f) currentFrames = southFrames;
        else if (angle >= 285f && angle < 345f) { currentFrames = swFrames;   sr.flipX = true; }
        else if (angle >= 345f || angle < 15f)  { currentFrames = westFrames; sr.flipX = true; }
        else if (angle >= 15f  && angle < 75f)  { currentFrames = nwFrames;   sr.flipX = true; }
        else if (angle >= 75f  && angle < 105f)   currentFrames = northFrames;
        else if (angle >= 105f && angle < 165f)   currentFrames = nwFrames;
        else if (angle >= 165f && angle < 210f)   currentFrames = westFrames;
        else if (angle >= 210f && angle < 255f)   currentFrames = swFrames;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerController>()?.Die();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
            col.gameObject.GetComponent<PlayerController>()?.Die();
    }

    void OnDrawGizmosSelected()
    {
        if (patrolZone == null) return;
        Gizmos.color = new Color(0f, 1f, 0.4f, 0.25f);
        Gizmos.DrawCube(patrolZone.bounds.center, patrolZone.bounds.size);
        Gizmos.color = new Color(0f, 1f, 0.4f, 0.9f);
        Gizmos.DrawWireCube(patrolZone.bounds.center, patrolZone.bounds.size);
    }
}