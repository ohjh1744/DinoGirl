using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class whsTEST : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text; 
    private StringBuilder sb = new StringBuilder();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerDataManager.Instance.PlayerData.OnItemChanged += ShowItemValue;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            TEST1();
        }
    }

    public void ShowItemValue(TextMeshProUGUI text, int num)
    {
        sb.Clear();
        sb.Append(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin]);

        text.SetText(sb);
        Debug.Log("TEST");
    }

    public void TEST1()
    {
        PlayerDataManager.Instance.PlayerData.SetItem(text,(int)E_Item.Coin, 1);
        Debug.Log("111");
    }

}
