using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Tile : NetworkBehaviour
{
    bool hit = false;
    [SerializeField] LayerMask shipMask;

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_TryHit(BoardPlayer originPlayer)
    {
        if (hit)
            return;
        var owner = GetComponentInParent<BoardPlayer>();
        if(owner != originPlayer)
        {
            print("valid");
            Ray ray = new Ray(transform.position - Vector3.forward, Vector3.forward);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var ship = hit.collider.GetComponentInParent<Ship>();
                if (ship)
                {
                    originPlayer.points++;
                    GetComponentInChildren<MeshRenderer>().material.color = Color.red;
                    ToyManager.vibrationScore += 50;
                }
                else
                {
                    GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
                }
                this.hit = true;
                HornyShips.instance.AdvanceTurns();
            }
            else
                print("no hit");
        }
    }
}
