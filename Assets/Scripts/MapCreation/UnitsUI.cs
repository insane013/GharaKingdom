using UnityEngine;
using UnityEngine.UI;

public class UnitsUI : MonoBehaviour
{
    [SerializeField] private EditorSelectButton _editorButton;
    [SerializeField] private Transform container;
    void Start()
    {
        if (MapCreation.UnitTemplates != null)
        {
            foreach (var c in MapCreation.UnitTemplates)
            {
                GameObject cell = Instantiate(_editorButton.gameObject, container);
                cell.GetComponent<EditorSelectButton>().ButtonType = 1;
                Image cImg = cell.transform.GetChild(0).gameObject.GetComponent<Image>();

                SpriteRenderer tImg = c.GetComponent<SpriteRenderer>();

                cImg.sprite = c.UnitData.UnitMiniature;
                cImg.color = tImg.color;

                cell.name = "c " + c.UnitData.UnitID;
            }
        }
    }
}
