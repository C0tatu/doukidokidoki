
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class statusBoard : UdonSharpBehaviour
{
    [UdonSynced] public int[] pidStatuses = new int[10];
    /*
     * index = pid
     * pidStatus[pid] = capsuleをつけるかどうか,どのアバターを使うかどうか
     * 0:つけない,1:rurune
     */

    public void ReqSerialization()
    {
        RequestSerialization();
    }

    private void Update()
    {
        
    }
}
