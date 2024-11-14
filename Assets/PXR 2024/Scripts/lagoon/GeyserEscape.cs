
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GeyserEscape : UdonSharpBehaviour
{
    public GeyserAnimationTriggers GeyserAnimationTriggers1;
    public GeyserAnimationTriggers GeyserAnimationTriggers2;
    public override void Interact()
    {
        // Send a networked event to teleport all players
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(TriggercorrectGeyser));
    }
    private void TriggercorrectGeyser()
    {
        GeyserAnimationTriggers1.PlayAnimationSet1();
        GeyserAnimationTriggers2.PlayAnimationSet1();
    }
}
