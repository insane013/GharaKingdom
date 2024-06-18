using UnityEngine;

public class ConstantScale : MonoBehaviour
{
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void LateUpdate()
    {
        Vector3 parentScale = transform.parent.localScale;

        transform.localScale = new Vector3(
            initialScale.x / parentScale.x,
            initialScale.y / parentScale.y,
            initialScale.z / parentScale.z
        );
    }
}
