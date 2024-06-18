using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Custom/Create Unit Data")]
public class UnitDataScriptableObject : ScriptableObject
{
    public enum ATTACK_TYPE {MELLEE, RANGE}

    [SerializeField] private int _unitID = 0;
    [SerializeField] private string _unitName = "DUMB";
    [SerializeField] private Sprite _unitMiniature;
    [TextArea(3, 6)]
    [SerializeField] private string _unitDescription;

    [Range(1, 30)][SerializeField] private int _initiative = 1;
    [SerializeField] private int _unitMovingDistance;
    [SerializeField] private int _unitHP;
    [Range(1, 10)][SerializeField] private int _unitDefence;
    [Range(0, 1)][SerializeField] private float _unitEvation;
    [SerializeField] private ATTACK_TYPE _unitAttackType;
    [SerializeField] private int _unitDamage;
    [SerializeField] private int _unitDamageRange;

    public int UnitID { get { return _unitID; } }
    public string UnitName { get { return _unitName; } }
    public string UnitDescription { get { return _unitDescription; } }
    public Sprite UnitMiniature { get { return _unitMiniature; } }
    public int UnitInitiative { get { return _initiative; } }
    public int UnitMovingDistance { get { return _unitMovingDistance; } }
    public int UnitHP { get { return _unitHP; } }
    public int UnitDefence { get { return _unitDefence; } }
    public float UnitEvation { get { return _unitEvation; } }

    public ATTACK_TYPE UnitAttackType { get { return _unitAttackType; } }
    public int UnitDamage { get { return _unitDamage; } }
    public int UnitDamageRange { get { return _unitDamageRange; } }
}
