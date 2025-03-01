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
    [SerializeField] AudioSource hitSource;

    NetworkVariable<int> curHealth = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        curHealth.OnValueChanged += (int previous, int current) => onCurHealthUpdate.Invoke(current);

        if (IsServer)
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

    [ClientRpc]
    void PlayHitSoundClientRpc() {
        if(IsServer)
            return;

        hitSource.Play();
    }

    [ServerRpc (RequireOwnership = false)]
    public void GetHitServerRpc()
    {
        SetCurHealth(curHealth.Value - 1);
        hitSource.Play();
        PlayHitSoundClientRpc();
        if (curHealth.Value <= 0)
            Die();
    }

    public void GetHit()
    {
        GetHitServerRpc();
    }
}
