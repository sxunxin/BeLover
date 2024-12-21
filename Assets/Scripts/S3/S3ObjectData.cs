using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S3ObjectData : MonoBehaviour
{
    public int id; // 고유 ID
    public bool isTomb; // 묘지 여부
    public bool SkullTrue;
    public bool isActivated = false; // 상호작용 여부를 기록
    public GameObject linkedCandleFire; // 연결된 CandleFire 오브젝트
}
