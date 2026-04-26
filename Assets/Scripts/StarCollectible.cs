using UnityEngine;

public class StarCollectible : MonoBehaviour
{
    private bool collected = false;

    void OnEnable()
    {
        collected = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (other.CompareTag("Player"))
        {
            collected = true;
            GameManager.Instance.AddScore();
            StarSpawnArea.Instance.OnStarCollected(this);
        }
    }

    public void ResetStar()
    {
        collected = false;
        gameObject.SetActive(false);
    }
}