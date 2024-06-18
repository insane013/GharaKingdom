using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverheadHpBar : MonoBehaviour
{
    [SerializeField] Image backgroundImage;
    [SerializeField] Image fillImage;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hp">0 < x < 1 </param>
    public void SetHp(float hp)
    {
        float maxW = backgroundImage.rectTransform.rect.width;
        fillImage.rectTransform.localScale = new Vector3(maxW * hp, 1, 1);
    }
}
