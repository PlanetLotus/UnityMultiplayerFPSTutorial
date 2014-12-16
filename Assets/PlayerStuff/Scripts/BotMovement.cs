using UnityEngine;
using System.Collections;

/// <summary>
/// Only the local client will have this enabled. This probably means the Master client, who is probably responsible for spawning bots.
/// Remote bots will have this script disabled.
/// In practice this could probably be combined with the PlayerMovement script but will keep them separate for now.
/// </summary>
public class BotMovement : MonoBehaviour {
    // Use this for initialization
    void Start() {
        netChar = GetComponent<NetworkCharacter>();
    }

    // Update is called once per frame
    void Update() {
        netChar.IsJumping = true;
    }

    private NetworkCharacter netChar;
}
