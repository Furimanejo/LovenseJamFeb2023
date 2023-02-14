using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class BoardPlayer : NetworkBehaviour
{
    public static BoardPlayer Local { get; private set; }
    [SerializeField] Camera cam;
    [SerializeField] List<Ship> ships;
    [SerializeField] UnityEngine.Events.UnityEvent onLocal = default;
    [SerializeField] LayerMask boardLayerMask;
    public Ship currentShip { get; set; }
    [Networked] public bool readyToPlay { get; set; }
    [SerializeField] GameObject setReadyBtn;
    public int points = 0;
    //bool mouse0;
    //bool mouse1;

    public override void Spawned()
    {
        base.Spawned();
        if (Object.HasInputAuthority)
        {
            Local = this;
            onLocal.Invoke();
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            foreach(var s in ships)
            {
                s.Hide();
            }
        }
    }

    //public override void FixedUpdateNetwork()
    //{
    //    base.FixedUpdateNetwork();
    //    if (GetInput(out PlayerInputData data))
    //    {
    //    }
    //}

    public void Update()
    {
        if (Local != this)
            return;
        if (HornyShips.instance.matchOngoing == false)
            SetupUpdate();
    }
    public void MatchUpdate()
    {
        if (Local != this)
            return;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            print("raycast");
            if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, boardLayerMask))
            {
                print(hit.collider.transform.parent.name);
                var tile = hit.collider.GetComponentInParent<Tile>();
                if (tile)
                    tile.RPC_TryHit(this);
            }
        }
    }
    public void SetupUpdate()
    {
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            //mouse1 = true;
            if (currentShip)
                currentShip.RPC_Rotate();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //mouse0 = true;
            if (currentShip != null)
            {
                currentShip.RPC_Drop(this);
            }
            else
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    var ship = hit.collider.GetComponentInParent<Ship>();
                    if (ship)
                        ship.RPC_Pick(this);
                }
            }
        }
        if (currentShip)
            currentShip.RPC_SetPosition(cam.ScreenToWorldPoint(Input.mousePosition));
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_TrySetReady()
    {
        bool allShipsArePlaced = true;
        foreach(var s in ships)
        {
            if (s.placed == false)
                allShipsArePlaced = false;
        }
        if (allShipsArePlaced)
        {
            readyToPlay = true;
            setReadyBtn.SetActive(false);
        }
        else
        {

        }
    }

    //public PlayerInputData GetInputData()
    //{
    //    var data = new PlayerInputData();
    //    data.mousePosition = Input.mousePosition;
    //    data.Mouse0 = mouse0;
    //    data.Mouse1 = mouse1;
    //    mouse0 = false;
    //    mouse1 = false;
    //    return data;
    //}
}