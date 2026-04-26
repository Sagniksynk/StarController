using UnityEngine;
using System.Collections.Generic;

public class StarObjectPool : MonoBehaviour
{
    public static StarObjectPool Instance;

    [Header("Pool")]
    public GameObject starPrefab;
    public int poolSize = 10;

    private Queue<StarCollectible> pool = new Queue<StarCollectible>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        for (int i = 0; i < poolSize; i++)
        {
            var go = Instantiate(starPrefab, transform);
            go.SetActive(false);
            pool.Enqueue(go.GetComponent<StarCollectible>());
        }
    }

    public StarCollectible GetStar()
    {
        if (pool.Count > 0) return pool.Dequeue();

        var go = Instantiate(starPrefab, transform);
        go.SetActive(false);
        return go.GetComponent<StarCollectible>();
    }

    public void ReturnStar(StarCollectible star)
    {
        star.ResetStar();
        pool.Enqueue(star);
    }
}