using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimConfig : MonoBehaviour
{
    [SerializeField]
    public static float moveAnimAccel = 0.2f;
    public static float maxMoveSpeed = 2;

    public static float camAnimAccel = 0.1f;
    public static float camMaxMoveSpeed = 1;
}
