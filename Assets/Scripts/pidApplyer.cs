
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class pidApplyer : UdonSharpBehaviour
{
    public Material mat;
    public int avatarNumber;
    public statusBoard stb;
    
    void Start()
    {
        
    }

    public override void Interact()
    {
        base.Interact();

        //オーナーじゃないならオーナー権限譲渡
        if (!Networking.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }

        //どのアバターを使うかを登録,同期
        int pid = Networking.LocalPlayer.playerId;
        stb.pidStatuses[pid] = avatarNumber;
        Debug.Log($"me: {Networking.LocalPlayer.playerId}, pid: {pid}");
        RequestSerialization(); //いらん気もするおまじない

        //触ったことがわかるように色変更
        this.gameObject.GetComponent<Renderer>().material = mat;


        for(int i=0; i<stb.pidStatuses.Length; i++)
        {
            Debug.Log($"stb {i}, {stb.pidStatuses[i]}");
        }
    }
}
