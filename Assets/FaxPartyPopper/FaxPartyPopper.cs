
using UdonSharp;
using UnityEngine;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class FaxPartyPopper : UdonSharpBehaviour
{
    [Header("Read the file README.MD to learn more.")]
    
    [SerializeField, Range(1, 200)]
    [Tooltip("Number of particles emitted per click")]
    private int numberOfParticlesToEmit = 30;

    [SerializeField, Range(0,1)]
    [Tooltip("Randomization of audio pitch. 1 would double or halve the pitch. (Lower value is hyperbolic)")]
    private float audioSourcePitchRandomization = 0.25f;
    
    [SerializeField, Range(0,1)]
    [Tooltip("Minimum delay between each use of the party popper.")]
    private float cooldownBetweenUses = 0.1f;
    
    [SerializeField]
    [Tooltip("Enable this to prevent using the popper if it would reach the maximum particle limit.")]
    private bool ignoreMaxParticleLimit;

    private ParticleSystem partyParticleSystem;
    private AudioSource audioSource;
    private bool isOnCooldown;

    private void Start()
    {
        partyParticleSystem = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(OnUse));
    }

    public void OnUse()
    {
        if (isOnCooldown) return;

        Pop();

        isOnCooldown = true;
        SendCustomEventDelayedSeconds(nameof(_EndCooldown), cooldownBetweenUses);
    }
    
    public void _EndCooldown()
    {
        isOnCooldown = false;
    }

    private void Pop()
    {
        if (!ignoreMaxParticleLimit)
        {
            var wouldExceedMaxParticles = partyParticleSystem.particleCount + numberOfParticlesToEmit
                                         > partyParticleSystem.main.maxParticles;
            if (wouldExceedMaxParticles) return;
        }

        partyParticleSystem.Emit(numberOfParticlesToEmit);
        audioSource.pitch = GetRandomAudioPitch();
        audioSource.Play();
    }

    private float GetRandomAudioPitch()
    {
        return UnityEngine.Random.Range(1 / (audioSourcePitchRandomization + 1), 1 + audioSourcePitchRandomization);
    }
}
