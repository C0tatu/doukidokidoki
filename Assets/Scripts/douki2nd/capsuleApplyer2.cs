
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class capsuleApplyer2 : UdonSharpBehaviour
{
    public SyncTable stb;
    public GameObject ruruneCapsuleParent;
    public GameObject[][] ruruneCapsules;
    public Collider[][] ruruneColliders;

    //速度計算用
    public Vector3[][] CapsuleVelocities = new Vector3[10][];
    public Vector3[][] CapsuleAngVelocities = new Vector3[10][];
    private Vector3[][] CapsulesPreviousPos = new Vector3[10][];
    private Quaternion[][] capsulesPreviousRotation = new Quaternion[10][];


    void Start()
    {
        //カプセルをクローン
        ruruneCapsules = setCapsulesAllay(ruruneCapsuleParent);
        //コライダーをクローン
        ruruneColliders = new Collider[10][];
        for (int i = 0; i < ruruneCapsules.Length; i++)
        {
            ruruneColliders[i] = new Collider[(int)HumanBodyBones.LeftToes];
            for (int j = 0; j < ruruneCapsules[i].Length; j++)
            {
                ruruneColliders[i][j] = ruruneCapsules[i][j].GetComponent<Collider>();
            }
        }

        for (int i = 0; i < 10; i++)
        {
            CapsuleVelocities[i] = new Vector3[(int)HumanBodyBones.LeftToes];
            CapsuleAngVelocities[i] = new Vector3[(int)HumanBodyBones.LeftToes];
            CapsulesPreviousPos[i] = new Vector3[(int)HumanBodyBones.LeftToes];
            capsulesPreviousRotation[i] = new Quaternion[(int)HumanBodyBones.LeftToes];
        }
    }

    private void FixedUpdate()
    {
        //追従させます
        for (int i = 1; i <= VRCPlayerApi.GetPlayerCount(); i++)
        {
            switch (stb.pidStatuses[i])
            {
                case 0: //つけない
                    break;
                case 1: //rurune
                    VRCPlayerApi player = VRCPlayerApi.GetPlayerById(i);
                    FollowCapsules(player, ruruneCapsules[i]);
                    break;
                default:
                    break;
            }
        }
        CapsuleRecorder();
    }

    /// <summary>
    /// カプセルを追従させる。それだけ
    /// </summary>
    private void FollowCapsules(VRCPlayerApi player, GameObject[] Capsules)
    {
        for (int i = 0; i < (int)HumanBodyBones.LeftToes; i++)
        {
            //Enum内のi番目のbone
            HumanBodyBones bone = (HumanBodyBones)i;

            //座標と回転を取得
            Vector3 pos = player.GetBonePosition(bone);
            Quaternion ang = player.GetBoneRotation(bone);

            //カプセル表示
            /*Debug.Log(i + " " + bone + " " + pos.ToString());*/
            Vector3 diff = new Vector3(1, 0, 0);//デバッグ用の右にずらすやつ
            Vector3 posDiff = new Vector3(0, 0, 0);
            Quaternion angleDiff = Quaternion.identity;

            //部位ごとの場合分け             boneの根本にカプセルを出しても意味ないからboneの中央に出るようにするよ
            Quaternion zRotation = Quaternion.AngleAxis(90f, Vector3.forward); //z軸90度
            GameObject obj = Capsules[i]; //いまいじってるカプセル
            float moveScale = 1f; //ずらす距離

            switch (bone.ToString())
            {
                case "Hips":
                    angleDiff = zRotation;
                    moveScale = obj.transform.localScale.z;
                    posDiff = ang * Vector3.down * (0.5f * moveScale);
                    break;
                case "Spine":
                    angleDiff = zRotation;

                    break;
                case "Chest":
                    angleDiff = zRotation;
                    moveScale = obj.transform.localScale.z;
                    posDiff = ang * Vector3.up * (0.5f * moveScale);
                    break;
                case "Neck":
                case "LeftHand":
                case "RightHand":
                case "LeftFoot":
                case "RightFoot":
                    //正直消してもいいけどいったんおいとく
                    break;
                default:
                    moveScale = obj.transform.localScale.y;
                    posDiff = ang * Vector3.up * (1f * moveScale);
                    break;

            }

            Capsules[i].transform.position = (pos + posDiff);
            Capsules[i].transform.rotation = ang * angleDiff;
        }
    }

    /// <summary>
    /// 親からカプセルを取り出す関数
    /// </summary>
    private GameObject[] extractCapsules(GameObject parent)
    {
        GameObject[] ret = new GameObject[(int)HumanBodyBones.LeftToes];
        for (int i = 0; i < (int)HumanBodyBones.LeftToes; i++)
        {
            for (int j = 0; j < parent.transform.childCount; j++)
            {
                //parentの子の中で名前が今探してるbone名と同じやつがあったら戻り値配列にいれる
                GameObject child = parent.transform.GetChild(j).gameObject;
                if ((((HumanBodyBones)i).ToString()) == child.name)
                {
                    ret[i] = child;
                }
            }
        }

        return ret;
    }


    /// <summary>
    /// アバターごとのカプセルのコピーを人数分つくっとく
    /// </summary>
    private GameObject[][] setCapsulesAllay(GameObject parent)
    {
        GameObject[] tmp = extractCapsules(parent);
        GameObject[][] ret = new GameObject[10][];
        for (int i = 0; i < ret.Length; i++)
        {
            ret[i] = new GameObject[(int)HumanBodyBones.LeftToes];
            for (int j = 0; j < ret[i].Length; j++)
            {
                ret[i][j] = Instantiate(tmp[j]);
                ret[i][j].GetComponent<OnTriggerDetector>().playerNumber = j;
                ret[i][j].transform.position -= new Vector3(0, 2, 0);
            }
        }
        return ret;
    }

    /// <summary>
    /// 計算用にいろいろ残しとく
    /// </summary>
    private void CapsuleRecorder()
    {
        float deltaTime = Time.fixedDeltaTime;
        for (int i = 1; i <= VRCPlayerApi.GetPlayerCount(); i++)
        {
            if (stb.pidStatuses[i] != 0)
            {
                VRCPlayerApi player = VRCPlayerApi.GetPlayerById(i);
                for (int j = 0; j < (int)HumanBodyBones.LeftToes; j++)
                {
                    HumanBodyBones bone = (HumanBodyBones)j;
                    Vector3 pos = player.GetBonePosition(bone);
                    Quaternion ang = player.GetBoneRotation(bone);
                    CapsuleVelocities[i][j] = (pos - CapsulesPreviousPos[i][j]) / deltaTime;
                    CapsulesPreviousPos[i][j] = pos;
                    Quaternion deltaRotation = ang * Quaternion.Inverse(capsulesPreviousRotation[i][j]);
                    deltaRotation.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
                    Vector3 angularVelocity = rotationAxis * angleInDegrees * Mathf.Deg2Rad / deltaTime;
                    CapsuleAngVelocities[i][j] = angularVelocity;
                    capsulesPreviousRotation[i][j] = ang;
                }
            }
        }
    }
}
