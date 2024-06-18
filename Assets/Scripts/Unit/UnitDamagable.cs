using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UnitDamagable : MonoBehaviour, IDamagable
{
    public UnityEvent<int, int> OnHpUpdated = new UnityEvent<int, int>();
    public UnityEvent Death = new UnityEvent();
    public UnityEvent HitMiss = new UnityEvent();

    [Range(0, 1)][SerializeField] private float _unitCurrentHPPercentage;

    private int _unitHP;
    private int _unitCurrentHP;
    private int _unitDefence;
    private float _unitEvation;

    private bool _isDeathAnimFinished;

    public int Defence => _unitDefence;
    public int MaxHP => _unitHP;
    public float UnitEvation => _unitEvation;
    public int CurrentHP 
    { 
        get => _unitCurrentHP;
        set
        {
            if (value <= 0) 
            {
                _unitCurrentHP = 0;
                Kill();
            } else
            {
                _unitCurrentHP = value;
            }
            OnHpUpdated?.Invoke(_unitCurrentHP, _unitHP);
        }
    }

    private bool _isDead = false;

    public bool IsDead => _isDead;
    public Vector2 Position 
    {
        get { return this.transform.position; }
    }

    public void Initialize(UnitDataScriptableObject data)
    {
        _unitHP = data.UnitHP;
        _unitDefence = data.UnitDefence;
        _unitEvation  = data.UnitEvation;

        _unitCurrentHP = (int)(MaxHP * _unitCurrentHPPercentage);
    }

    public void Kill()
    {
        _isDead = true;
        StartCoroutine(KillProcess());
    }

    IEnumerator KillProcess()
    {
        Death?.Invoke();

        UnitActionsController.Instance.KillUnit(gameObject.GetComponent<Unit>());
        while (!_isDeathAnimFinished) yield return null;
        _isDeathAnimFinished = false;
        gameObject.SetActive(false);
        EventManager.TriggerOnUnitDeath(gameObject.GetComponent<Unit>());
    }

    private void DeathAnimationFinishedProcessing()
    {
        _isDeathAnimFinished = true;
    }

    public void TakeDamage(int damage)
    {
        float hitRoll = Random.Range(0f, 1f);

        if (hitRoll > UnitEvation)
        {
            CurrentHP -= Mathf.FloorToInt(damage * (1 - (float)Defence / 10));
            GetComponent<UnitAnimations>().AnimateHurt();
        }
        else
        {
            HitMiss?.Invoke();
        }
    }
}
