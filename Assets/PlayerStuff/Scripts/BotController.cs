using UnityEngine;
using System.Collections;

/// <summary>
/// Responsible for all bot AI.
/// Only the local client will have this enabled. This probably means the Master client, who is probably responsible for spawning bots.
/// Remote bots will have this script disabled.
/// In practice this could probably be combined with the PlayerMovement script but will keep them separate for now.
/// </summary>
public class BotController : MonoBehaviour {
    // Use this for initialization
    void Start() {
        netChar = GetComponent<NetworkCharacter>();

        if (waypoints == null)
            waypoints = GameObject.FindObjectsOfType<Waypoint>();

        destination = GetClosestWaypoint();
    }

    // Update is called once per frame
    void Update() {
        SetDestination();
        SetDirection();

        TeamMember closestTarget = GetUnitTarget();
        SetRotation(closestTarget);

        DoFire(closestTarget);
    }

    private void SetDestination() {
        if (destination != null) {
            if (Vector3.Distance(destination.transform.position, transform.position) <= waypointTargetDist) {
                // Is this *ever* null? We should make this impossible.
                if (destination.connectedWPs != null && destination.connectedWPs.Length > 0) {
                    destination = destination.connectedWPs[Random.Range(0, destination.connectedWPs.Length)];
                }
            }
        }
    }

    private void SetDirection() {
        if (destination != null) {
            netChar.Direction = destination.transform.position - transform.position;
            netChar.Direction.y = 0;
            netChar.Direction.Normalize();
        } else {
            // I believe this is redundant with just setting Direction to zero above...
            netChar.Direction = Vector3.zero;
        }
    }

    private TeamMember GetUnitTarget() {
        // Target cooldown both makes the AI (intentionally) dumber and reduces frequency of expensive FindObjectsOfType calls
        targettingCooldown -= Time.deltaTime;
        if (targettingCooldown > 0)
            return null;

        targettingCooldown = 0.5f;

        // If we have an enemy target in range, look that way instead
        TeamMember closest = null;
        float dist = 0f;

        foreach (TeamMember teamMember in GameObject.FindObjectsOfType<TeamMember>()) { // SLOW!
            if (teamMember == GetComponent<TeamMember>())
                continue;

            // Enemy check
            if (teamMember.TeamId == 0 || teamMember.TeamId != GetComponent<TeamMember>().TeamId) {
                // Range check
                float d = Vector3.Distance(teamMember.transform.position, transform.position);
                if (d <= aggroRange) {

                    // TODO: Do a raycast to make sure we have line of sight. Shouldn't detect enemy through walls!

                    if (closest == null || d < dist) {
                        closest = teamMember;
                        dist = d;
                    }
                }
            }
        }
        return closest;
    }

    private void SetRotation(TeamMember closest) {
        // Default: Look where we're going
        Vector3 lookDirection = netChar.Direction;

        if (closest != null) {
            lookDirection = closest.transform.position - transform.position;
        }

        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        lookRotation.eulerAngles = new Vector3(0, lookRotation.eulerAngles.y, 0);
        transform.rotation = lookRotation;

        if (closest != null) {
            // Figure out the relative vertical angle to our target and adjust AimAngle
            Vector3 localLookDirection = transform.InverseTransformPoint(closest.transform.position);
            float targetAimAngle = Mathf.Atan2(localLookDirection.y, localLookDirection.z) * Mathf.Rad2Deg;
            netChar.AimAngle = targetAimAngle;
        } else {
            // We don't have a target, so just aim straight
            netChar.AimAngle = 0;
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

    private void DoFire(TeamMember target) {
        if (target == null)
            return;

        // Ignore vertical height for determining if we should shoot
        Vector3 targetPos = target.transform.position;
        targetPos.y = transform.position.y;

        if (Vector3.Angle(transform.forward, targetPos - transform.position) < targetAngleCriteria) {
            // First, get our direction in local space
            Vector3 fireDirection = Quaternion.Euler(netChar.AimAngle, 0, 0) * Vector3.forward;

            // Add a bit of inaccuracy for bots
            // Currently buggy as of latest tutorial video
            /*
            Vector3 inaccuracyAngle = new Vector3(Random.Range(-targetInaccuracy, targetInaccuracy), Random.Range(-targetInaccuracy, targetInaccuracy), 0);
            fireDirection = Quaternion.Euler(inaccuracyAngle) * fireDirection;
            */

            // Convert to global space
            fireDirection = transform.TransformDirection(fireDirection);

            // Hack: Make the shot come out of the bot's chest, not feet
            Vector3 botBodyOffset = new Vector3(0, 1.5f, 0);
            netChar.FireWeapon(transform.position + botBodyOffset, fireDirection);
        }
    }

    private NetworkCharacter netChar;
    private static Waypoint[] waypoints;
    private Waypoint destination;
    private float waypointTargetDist = 1f;
    private float aggroRange = 100000f;
    private float targettingCooldown = 0f;
    private float targetInaccuracy = 20f;

    // Angle at which our target needs to be for us to start shooting at it
    private const float targetAngleCriteria = 10f;
}
