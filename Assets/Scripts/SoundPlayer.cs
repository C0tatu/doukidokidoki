
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SoundPlayer : UdonSharpBehaviour
{
    public capsuleApplyer capsuleApplyer;
    public calcFuncs calc;
    public statusBoard stb;

    public AudioSource[] FrictionSources;
    public AudioSource[] CrumpingSources;

    private string[][] crumpingPairs = new string[][]
    {
        new string[] {"LeftShoulder", "LeftUpperArm"},
        new string[] {"LeftUpperArm", "LeftLowerArm"},
        new string[] {"LeftUpperLeg", "LeftLowerLeg"},

        new string[] {"RightShoulder", "RightUpperArm"},
        new string[] {"RightUpperArm", "RightLowerArm"},
        new string[] {"RightUpperLeg", "RightLowerLeg"},
    };
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        //対他人
        for(int i=1; i<=VRCPlayerApi.GetPlayerCount(); i++)
        {
            //カプセルついてないやついたら次へ
            if (stb.pidStatuses[i] != 0) {
                for (int j = VRCPlayerApi.GetPlayerCount(); j > i; j--)
                {
                    //カプセルついてないやついたら次へ
                    if (stb.pidStatuses[j] != 0)
                    {
                        //つけてるアバターによって計算に使うカプセルを変える
                        GameObject[] capsules1;
                        GameObject[] capsules2;
                        Collider[] colliders1;
                        Collider[] colliders2;
                        switch (stb.pidStatuses[i])
                        {
                            case 1:
                                capsules1 = capsuleApplyer.ruruneCapsules[i];
                                colliders1 = capsuleApplyer.ruruneColliders[i];
                                break;
                            default:
                                capsules1 = new GameObject[(int)HumanBodyBones.LeftToes];
                                colliders1 = new Collider[(int)HumanBodyBones.LeftToes];
                                break;
                        }
                        switch (stb.pidStatuses[j])
                        {
                            case 1:
                                capsules2 = capsuleApplyer.ruruneCapsules[j];
                                colliders2 = capsuleApplyer.ruruneColliders[j];
                                break;
                            default:
                                capsules2 = new GameObject[(int)HumanBodyBones.LeftToes];
                                colliders2 = new Collider[(int)HumanBodyBones.LeftToes];
                                break;
                        }

                        for (int k = 0; k < capsules1.Length; k++)
                        {
                            for (int l = 0; l < capsules2.Length; l++)
                            {
                                GameObject tar1 = capsules1[k];
                                GameObject tar2 = capsules2[l];
                                Collider c1 = colliders1[k];
                                Collider c2 = colliders2[l];
                                //重なってるか
                                if (calc.isBounds(tar1, tar2,c1,c2))
                                {
                                    Vector3 v1 = capsuleApplyer.CapsuleVelocities[i][k];
                                    Vector3 v2 = capsuleApplyer.CapsuleVelocities[j][l];
                                    Vector3 angv1 = capsuleApplyer.CapsuleAngVelocities[i][k];
                                    Vector3 angv2 = capsuleApplyer.CapsuleAngVelocities[j][l];
                                    Vector3 frictionV = calc.FrictionCalc(tar1, tar2, v1, v2, angv1, angv2);

                                    //閾値以上なら音関連の計算をする
                                    float threshold = 0.25f;
                                    if(frictionV.magnitude > threshold)
                                    {
                                        Vector3 soundPoint = (c1.ClosestPoint(c2.transform.position) + c2.ClosestPoint(c1.transform.position)) / 2f;

                                        //ここで音を鳴らす
                                        if (!isActiveAuidoNearby(true, soundPoint))//近くに再生中音源がないなら
                                        {
                                            if (frictionV.magnitude > threshold) // && frictionTop < frictionPairs.Length
                                            {
                                                int audioSourceNumber = getAvailableAudioSource(FrictionSources);
                                                if (audioSourceNumber != -1)
                                                {
                                                    playSound(FrictionSources[audioSourceNumber], soundPoint, frictionV, true);
                                                }
                                                else
                                                {
                                                    Debug.Log("friction -> -1");
                                                }
                                            }
                                        }

                                    }
                                    
                                }
                            }
                        }
                    }
                }
            }
        }
        //自分自身
        for(int i=1; i<=VRCPlayerApi.GetPlayerCount(); i++)
        {
            //ボタン押してるやつなら
            if (stb.pidStatuses[i] != 0)
            {
                //カプセルを持ってきます
                GameObject[] capsules;
                Collider[] colliders;
                switch (stb.pidStatuses[i])
                {
                    case 1:
                        capsules = capsuleApplyer.ruruneCapsules[i];
                        colliders = capsuleApplyer.ruruneColliders[i];
                        break;
                    default:
                        capsules = new GameObject[(int)HumanBodyBones.LeftToes];
                        colliders = new Collider[(int)HumanBodyBones.LeftToes];
                        break;
                }


                for (int j = 0; j < capsules.Length; j++)
                {
                    for (int k = capsules.Length - 1; k > j; k--)
                    {
                        GameObject tar1 = capsules[j];
                        GameObject tar2 = capsules[k];
                        Collider c1 = colliders[j];
                        Collider c2 = colliders[k];
                        //ねじれ
                        if (isCrumpingPair(tar1.name, tar2.name))
                        {
                            Vector3 angv1;
                            Vector3 angv2;
                            if (tar1.name.Contains("Shoulder"))
                            {
                                angv1 = capsuleApplyer.CapsuleAngVelocities[i][j];
                                angv2 = capsuleApplyer.CapsuleAngVelocities[i][k];
                            }
                            else if (tar2.name.Contains("Shoulder"))
                            {
                                angv2 = capsuleApplyer.CapsuleAngVelocities[i][j];
                                angv1 = capsuleApplyer.CapsuleAngVelocities[i][k];
                            }
                            else if (tar1.name.Contains("Upper"))
                            {
                                angv1 = capsuleApplyer.CapsuleAngVelocities[i][j];
                                angv2 = capsuleApplyer.CapsuleAngVelocities[i][k];
                            }
                            else
                            {
                                angv2 = capsuleApplyer.CapsuleAngVelocities[i][j];
                                angv1 = capsuleApplyer.CapsuleAngVelocities[i][k];
                            }
                            Vector3 crumpingV = calc.CrumpingCalc(angv1, angv2);

                            //閾値以上の速度なら音を鳴らす
                            float threshold = 0f;
                            if(crumpingV.magnitude > threshold)
                            {
                                Vector3 soundPoint = (c1.ClosestPoint(c2.transform.position) + c2.ClosestPoint(c1.transform.position)) / 2f;
                                //ここで音を鳴らす


                                //音再生
                                if (!isActiveAuidoNearby(false, soundPoint))
                                {
                                    if (crumpingV.magnitude > threshold)
                                    {
                                        int audioSourceNumber = getAvailableAudioSource(CrumpingSources);
                                        if (audioSourceNumber != -1)
                                        {
                                            playSound(CrumpingSources[audioSourceNumber], soundPoint, crumpingV, false);
                                        }
                                        else
                                        {
                                            Debug.Log("crumping -> -1");
                                        }
                                    }

                                }

                            }


                            
                        }
                        else
                        {
                            if (calc.isBounds(tar1, tar2,c1,c2))//こすれ
                            {

                                Vector3 v1 = capsuleApplyer.CapsuleVelocities[i][j];
                                Vector3 v2 = capsuleApplyer.CapsuleVelocities[i][k];
                                Vector3 angv1 = capsuleApplyer.CapsuleAngVelocities[i][j];
                                Vector3 angv2 = capsuleApplyer.CapsuleAngVelocities[i][k];
                                Vector3 frictionV = calc.FrictionCalc(tar1, tar2, v1, v2, angv1, angv2);

                                //閾値以上なら音関連の計算をする
                                float threshold = 0.25f;
                                if(frictionV.magnitude > threshold)
                                {
                                    Vector3 soundPoint = (c1.ClosestPoint(c2.transform.position) + c2.ClosestPoint(c1.transform.position)) / 2f;

                                    //ここで音を鳴らす
                                    if (!isActiveAuidoNearby(true, soundPoint))//近くに再生中音源がないなら
                                    {
                                        if (frictionV.magnitude > threshold) // && frictionTop < frictionPairs.Length
                                        {
                                            int audioSourceNumber = getAvailableAudioSource(FrictionSources);
                                            if (audioSourceNumber != -1)
                                            {
                                                playSound(FrictionSources[audioSourceNumber], soundPoint, frictionV, true);
                                            }
                                            else
                                            {
                                                Debug.Log("friction -> -1");
                                            }
                                        }
                                    }

                                }

                            }
                        }
                    }
                }

            }

            
        }
    }

    private bool isCrumpingPair(string name1, string name2)
    {
        for (int i = 0; i < crumpingPairs.Length; i++)
        {
            if (crumpingPairs[i][0] == name1 && crumpingPairs[i][1] == name2) return true;
            if (crumpingPairs[i][1] == name1 && crumpingPairs[i][0] == name2) return true;
        }
        return false;
    }

    private void playSound(AudioSource source, Vector3 point, Vector3 Speed, bool isFriction)
    {
        if (isFriction)
        {
            float maxVolume = 0.6f;
            float maxPitch = 3f;
            //float param = Speed.magnitude / frictionMaxSpeed;
            float param = Speed.magnitude;
            source.volume = param * maxVolume;
            source.pitch = param * maxPitch;
            //Debug.Log($"friction, param: {param}");
        }
        else
        {
            float maxVolume = 0.4f;
            float maxPitch = 3f;
            //float param = Speed.magnitude / crumpingMaxSpeed;
            float param = Speed.magnitude;
            source.volume = param * maxVolume;
            source.pitch = param * maxPitch;
            //Debug.Log($"crumping, param: {param}");
        }
        source.transform.position = point;
        source.Play();

    }

    private int getAvailableAudioSource(AudioSource[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].isPlaying == false) return i;
        }
        return -1;
    }

    private bool isActiveAuidoNearby(bool isFriction, Vector3 pos)
    {
        float threshhold = 0.15f;
        if (isFriction)
        {
            for (int i = 0; i < FrictionSources.Length; i++)
            {
                if (Vector3.Distance(pos, FrictionSources[i].transform.position) < threshhold)
                {

                    if (FrictionSources[i].isPlaying) return true;
                }
            }
        }
        else
        {
            for (int i = 0; i < CrumpingSources.Length; i++)
            {
                if (Vector3.Distance(pos, CrumpingSources[i].transform.position) < threshhold)
                {
                    if (CrumpingSources[i].isPlaying) return true;
                }
            }
        }
        return false;
    }
}
