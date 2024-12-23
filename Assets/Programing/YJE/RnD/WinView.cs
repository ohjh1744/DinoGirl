using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinView : BaseUI
{
    // Start is called before the first frame update
    private void Start()
    {
        Image image = GetUI("Button").GetComponent<Image>();
        image.color = Color.red;
    }

}
