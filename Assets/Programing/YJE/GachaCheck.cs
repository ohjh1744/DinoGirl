using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaCheck : MonoBehaviour
{
    GachaBtn gachaBtn;
    GachaSceneController gachaSceneController;

    private void Awake()
    {
        gachaBtn = gameObject.GetComponent<GachaBtn>();
        gachaSceneController = gameObject.GetComponent<GachaSceneController>();
    }


}
