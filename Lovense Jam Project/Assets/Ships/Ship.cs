using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Ship : NetworkBehaviour
{
    [SerializeField] List<Transform> points;
    Vector3 prevPosition;
    Quaternion prevRotation;
    [Networked] public bool placed { get; private set; }

    public void Hide()
    {
        GetComponentInChildren<Renderer>().enabled = false;
    }
    public void Show()
    {
        GetComponentInChildren<Renderer>().enabled = true;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void RPC_Pick(BoardPlayer player)
    {
        print("pick");
        player.currentShip = this;
        placed = false;
        prevPosition = transform.position;
        prevRotation = transform.rotation;
        GetComponentInChildren<Collider>().enabled = false;
    }
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void RPC_Drop(BoardPlayer player)
    {
        print("drop");
        player.currentShip = null;
        var pos = transform.position;
        pos.x = Mathf.RoundToInt(pos.x);
        pos.y = Mathf.RoundToInt(pos.y);
        transform.position = pos;

        bool outOfBounds = false;
        foreach(var p in points)
        {
            Ray ray = new Ray(p.position + Vector3.back, Vector3.forward);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var tile = hit.collider.GetComponentInParent<BoardPlayer>();
                if (tile == null)
                    outOfBounds = true;
                else
                    if (tile.GetComponentInParent<BoardPlayer>() != player)
                        outOfBounds = true;
                var ship = hit.collider.GetComponentInParent<Ship>();
                if(ship != null)
                    outOfBounds = true;
            }
            else
                outOfBounds = true;
        }
        if (outOfBounds)
        {
            print("drop fail");
            transform.position = prevPosition;
            transform.rotation = prevRotation;
        }
        else
        {
            placed = true;
        }
        GetComponentInChildren<Collider>().enabled = true;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void RPC_Rotate()
    {
        var rotation = transform.rotation.eulerAngles;
        rotation.z += 90;
        transform.rotation = Quaternion.Euler(rotation);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void RPC_SetPosition(Vector3 position)
    {
        position.z = 0;
        gameObject.transform.position = position;
    }
}
