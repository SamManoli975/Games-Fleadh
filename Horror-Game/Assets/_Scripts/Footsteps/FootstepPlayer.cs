using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class FootstepPlayer : NetworkBehaviour
{
    [SerializeField] private float rayLength = 3f;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource footstepSweetenerAudioSource;

    [SerializeField] private float mainSourceBasePitch = 1f;
    [SerializeField] private float mainSourcePitchVariation = 0.1f;

    [SerializeField] private float sweetenerSourceBasePitch = 1f;
    [SerializeField] private float sweetenerSourcePitchVariation = 0.1f;


    [ServerRpc]
    void PlayFootstepServerRpc()
    {
        PlayFootstepClientRpc();
    }
    [ClientRpc]
    void PlayFootstepClientRpc()
    {
        SurfaceType surfaceType = SurfaceType.standard;

        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.transform.position, -raycastOrigin.transform.up, out hit, rayLength, layerMask))
        {
            FootstepSurface footstepSurface = hit.collider.GetComponent<FootstepSurface>();
            if (footstepSurface != null)
            {
                surfaceType = footstepSurface.GetSufraceType();
            }
            else
            {
                Debug.LogError(hit.collider.name + " object is on the footstep surface layer but it does not have 'FootstepSurface' script");
            }
        }

        SurfaceFootstepAudio surfaceFootstepAudio = FootstepsManager.instance.GetSurfaceAudioClips(surfaceType);
        if (surfaceFootstepAudio == null)
            return;

        if (surfaceFootstepAudio.mainAudioClip != null)
        {
            footstepAudioSource.clip = surfaceFootstepAudio.mainAudioClip;
            footstepAudioSource.pitch = mainSourceBasePitch + mainSourcePitchVariation * Random.Range(-1f, 1f);
            footstepAudioSource.Play();
        }

        if (footstepSweetenerAudioSource != null && surfaceFootstepAudio.sweetenerAudioClip != null)
        {
            footstepSweetenerAudioSource.clip = surfaceFootstepAudio.sweetenerAudioClip;
            footstepSweetenerAudioSource.pitch = sweetenerSourceBasePitch + sweetenerSourcePitchVariation * Random.Range(-1f, 1f);
            footstepSweetenerAudioSource.Play();
        }
    }

    public void PlayFootstep()
    {
        if (IsOwner)
            PlayFootstepServerRpc();
    }
}
