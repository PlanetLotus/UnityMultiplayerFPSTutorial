using UnityEngine;
using System.Collections;

public class TeamMember : MonoBehaviour {
    public int TeamId { get { return teamId; } }

    [RPC]
    private void SetTeamId(int id) {
        teamId = id;
        SkinnedMeshRenderer mySkin = this.transform.GetComponentInChildren<SkinnedMeshRenderer>();

        if (teamId == 1)
            mySkin.material.color = Color.red;
        else if (teamId == 2)
            mySkin.material.color = new Color(.5f, 1f, .5f);
    }

    private int teamId = 0;
}
