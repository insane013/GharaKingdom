using System.Collections;
using UnityEngine;

public class UnitAnimationSync : MonoBehaviour
{
    [SerializeField] private float delay = 0f;

    private void AttackAnimationPerformed()
    {
        //StartCoroutine(WaitDelay("AttackAnimationFinishedProcessing", delay));
    }

    private void DeathAnimationPerformed()
    {
        StartCoroutine(WaitDelay("DeathAnimationFinishedProcessing", 0));
    }

    IEnumerator WaitDelay(string msg, float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SendMessage(msg);
    }
}
