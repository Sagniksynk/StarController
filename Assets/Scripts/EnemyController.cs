using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing, Returning }

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float waypointReachThreshold = 0.2f;

    [Header("Chase")]
    public float chaseSpeed = 4f;
    public float detectionRadius = 3f;

    [Header("Directional Sprites (same sheet)")]
    public Sprite[] southFrames;
    public Sprite[] swFrames;
    public Sprite[] westFrames;
    public Sprite[] nwFrames;
    public Sprite[] northFrames;

    [Header("Animation")]
    public float frameRate = 8f;

    private EnemyState currentState = EnemyState.Patrolling;
    private int currentPatrolIndex = 0;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Sprite[] currentFrames;
    private int currentFrame = 0;
    private float frameTimer = 0f;

    private bool isChasing = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentFrames = southFrames;
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        if (patrolPoints != null && patrolPoints.Length > 0)
            transform.position = patrolPoints[0].position;
    }

    void Update()
    {
        if (player == null) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrolling:
                isChasing = false;
                if (dist <= detectionRadius) currentState = EnemyState.Chasing;
                else Patrol();
                break;

            case EnemyState.Chasing:
                isChasing = true;
                if (dist > detectionRadius) currentState = EnemyState.Returning;
                else Chase();
                break;

            case EnemyState.Returning:
                isChasing = false;
                if (dist <= detectionRadius) currentState = EnemyState.Chasing;
                else ReturnToNearestPoint();
                break;
        }

        sr.color = isChasing ? Color.red : Color.white;

        TickAnimation(rb.linearVelocity);
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        MoveTowards(patrolPoints[currentPatrolIndex].position, patrolSpeed);
        if (Vector2.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < waypointReachThreshold)
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void Chase() => MoveTowards(player.position, chaseSpeed);

    void ReturnToNearestPoint()
    {
        int nearest = 0;
        float minDist = float.MaxValue;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float d = Vector2.Distance(transform.position, patrolPoints[i].position);
            if (d < minDist) { minDist = d; nearest = i; }
        }
        MoveTowards(patrolPoints[nearest].position, patrolSpeed);
        if (Vector2.Distance(transform.position, patrolPoints[nearest].position) < waypointReachThreshold)
        {
            currentPatrolIndex = nearest;
            currentState = EnemyState.Patrolling;
        }
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector2 dir = ((Vector2)target - (Vector2)transform.position).normalized;
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

        if (angle >= 255f && angle < 285f)
            currentFrames = southFrames;
        else if (angle >= 285f && angle < 345f)
        { currentFrames = swFrames; sr.flipX = true; }
        else if (angle >= 345f || angle < 15f)
        { currentFrames = westFrames; sr.flipX = true; }
        else if (angle >= 15f && angle < 75f)
        { currentFrames = nwFrames; sr.flipX = true; }
        else if (angle >= 75f && angle < 105f)
            currentFrames = northFrames;
        else if (angle >= 105f && angle < 165f)
            currentFrames = nwFrames;
        else if (angle >= 165f && angle < 210f)
            currentFrames = westFrames;
        else if (angle >= 210f && angle < 255f)
            currentFrames = swFrames;
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
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}