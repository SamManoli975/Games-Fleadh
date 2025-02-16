using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour, IHitable
{
    public UnityEvent<int> onCurHealthUpdate;
    public UnityEvent onDied;

    [SerializeField] int maxHealth = 2;

    NetworkVariable<int> curHealth = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        curHealth.OnValueChanged += (int previous, int current) => onCurHealthUpdate.Invoke(current);

        if (IsOwner)
            SetCurHealth(maxHealth);
    }

    void SetCurHealth(int newValue)
    {
        curHealth.Value = newValue;
    }

    void Die()
    {
        Debug.Log("Died");
        onDied.Invoke();
    }

    public void GetHit()
    {
        if (!IsServer)
            return;

        SetCurHealth(curHealth.Value - 1);
        if (curHealth.Value <= 0)
            Die();
    }
}
