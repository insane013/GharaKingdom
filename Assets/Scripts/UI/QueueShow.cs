using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueShow : MonoBehaviour
{
    [SerializeField] private int showedQueueItemsNumber = 5;

    [SerializeField] private GameObject queueItemTemplate;
    [SerializeField] private GameObject queueItemsContainer;

    private List<GameObject> queueItems;

    private void OnEnable()
    {
        EventManager.OnQueueUpdated.AddListener(ShowQueue);
    }

    private void OnDisable()
    {
        EventManager.OnQueueUpdated.RemoveListener(ShowQueue);
    }

    public void Initialize()
    {
        queueItems = new List<GameObject>();
        //EventManager.OnQueueUpdated.AddListener(ShowQueue);
    }
    public void ShowQueue(Queue<Unit> queue)
    {
        if (queueItems.Count > 0)
        {
            foreach (var item in queueItems)
            {
                Destroy(item);
            }
            queueItems.Clear();
        }

        for (int i = 0; i < showedQueueItemsNumber; i++)
        {
            if (queue.Count > 0)
            {
                GameObject t = Instantiate(queueItemTemplate, queueItemsContainer.transform);
                t.GetComponent<QueueItemUI>().SetUnit(queue.Dequeue());
                queueItems.Add(t);
            }
            else
            {
                break;
            }
        }

        if (queueItems.Count > 0)
        {
            RectTransform rect = queueItems[0].GetComponent<RectTransform>();
            var width = rect.rect.width;
            var height = rect.rect.height;

            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * 1.4f);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * 1.4f);
        }
    }
}
