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

        if (waypoints == null)
            waypoints = GameObject.FindObjectsOfType<Waypoint>();

        destination = GetClosestWaypoint();
    }

    // Update is called once per frame
    void Update() {
        if (destination != null) {
            if (Vector3.Distance(destination.transform.position, transform.position) <= waypointTargetDist) {
                // Is this *ever* null? We should make this impossible.
                if (destination.connectedWPs != null && destination.connectedWPs.Length > 0) {
                    destination = destination.connectedWPs[Random.Range(0, destination.connectedWPs.Length)];
                }
            }
        }

        if (destination != null) {
            netChar.Direction = destination.transform.position - transform.position;
            netChar.Direction.y = 0;
            netChar.Direction.Normalize();

            transform.rotation = Quaternion.LookRotation(netChar.Direction);
        } else {
            // I believe this is redundant with just setting Direction to zero above...
            netChar.Direction = Vector3.zero;
        }
    }

    private Waypoint GetClosestWaypoint() {
        Waypoint closestWaypoint = null;
        float dist = 0f;

        foreach (Waypoint waypoint in waypoints) {
            if (closestWaypoint == null || Vector3.Distance(transform.position, waypoint.transform.position) < dist) {
                closestWaypoint = waypoint;
                dist = Vector3.Distance(transform.position, waypoint.transform.position);
            }
        }

        return closestWaypoint;
    }

    private NetworkCharacter netChar;
    private static Waypoint[] waypoints;
    private Waypoint destination;
    private float waypointTargetDist = 1f;
}
