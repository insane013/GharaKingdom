using UnityEngine;

public class SpriteOrdering : MonoBehaviour
{
    public bool useForChildren = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float yPos = transform.position.y;
        if (!useForChildren)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-yPos + 100);
        } else
        {
            int parentOrder = spriteRenderer.sortingOrder;
            SpriteRenderer[] children = GetComponentsInChildren<SpriteRenderer>();
            foreach (var item in children)
            {
                item.sortingOrder = Mathf.RoundToInt(-yPos + 100);
            }
            spriteRenderer.sortingOrder = parentOrder;
        }
    }
}
