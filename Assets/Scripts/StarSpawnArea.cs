using UnityEngine;
using System.Collections.Generic;

public class StarSpawnArea : MonoBehaviour
{
    public static StarSpawnArea Instance;

    [Header("Spawn")]
    public int maxStars = 5;

    private Collider2D areaCollider;
    private List<StarCollectible> activeStars = new List<StarCollectible>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        areaCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        for (int i = 0; i < maxStars; i++) SpawnStar();
    }

    public void SpawnStar()
    {
        if (activeStars.Count >= maxStars) return;

        StarCollectible star = StarObjectPool.Instance.GetStar();
        star.transform.position = GetRandomPoint();
        star.gameObject.SetActive(true);
        activeStars.Add(star);
    }

    public void OnStarCollected(StarCollectible star)
    {
        activeStars.Remove(star);
        StarObjectPool.Instance.ReturnStar(star);
        SpawnStar();
    }

    Vector2 GetRandomPoint()
    {
        Bounds b = areaCollider.bounds;
        return new Vector2(
            Random.Range(b.min.x + 0.5f, b.max.x - 0.5f),
            Random.Range(b.min.y + 0.5f, b.max.y - 0.5f)
        );
    }
}