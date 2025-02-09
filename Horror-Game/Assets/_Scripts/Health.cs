using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IHitable
{
    public UnityEvent<int> onCurHealthUpdate;
    public UnityEvent onDied;

    [SerializeField] int maxHealth = 2;

    public int curHealth = 0;

    void Start()
    {
        SetCurHealth(maxHealth);
    }

    void SetCurHealth(int newValue)
    {
        curHealth = newValue;
        onCurHealthUpdate.Invoke(curHealth);
    }

    void Die()
    {
        Debug.Log("Died");
        onDied.Invoke();
    }

    public void GetHit()
    {
        SetCurHealth(curHealth - 1);
        if (curHealth <= 0)
            Die();
    }
}
