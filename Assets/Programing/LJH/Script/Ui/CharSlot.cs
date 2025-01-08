using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharSlot : MonoBehaviour
{
    [SerializeField] public int charId;
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Level;
    [SerializeField] Image gridimage;
    [SerializeField] Image charimage;

    [SerializeField] GameObject Atk;
    [SerializeField] TMP_Text atkTxt;
    [SerializeField] GameObject Def;
    [SerializeField] TMP_Text defTxt;
    [SerializeField] GameObject Hp;
    [SerializeField] TMP_Text hpTxt;
    [SerializeField] GameObject Cool;
    [SerializeField] TMP_Text coolTxt;


    private int _buffCount;

    public int buffCount
    {
        get { return _buffCount; }
        set
        {
            if (_buffCount != value)
            {
                _buffCount = value;

                onBuffDatas();
            }
        }
    }


    public void setCharSlotData(int id, string name, string level /*Image gridImage*/, Sprite charImage)
    {
        charId = id;
        Name.text = name;
        Level.text = level;
        //gridimage = gridImage;
        charimage.sprite = charImage;
    }

    public void onBuffDatas()
    {
        int atkNum = 0;
        int defNum = 0;
        int hpNum = 0;
        int coolNum = 0;
        Atk.SetActive(false);
        Def.SetActive(false);
        Hp.SetActive(false);
        Cool.SetActive(false);
        if (gameObject.GetComponent<UnitStat>().buffs.Count != 0)
        {
            for (int i = 0; i < gameObject.GetComponent<UnitStat>().buffs.Count; i++)
            {
                Debug.Log($"캐릭터 : {charId}는   x{gameObject.GetComponent<UnitStat>().buffs[i].x} y {gameObject.GetComponent<UnitStat>().buffs[i].y} z {gameObject.GetComponent<UnitStat>().buffs[i].z}의 버프를 가지고 있음");
                switch (gameObject.GetComponent<UnitStat>().buffs[i].y)
                {
                    case 1:            
                        Hp.SetActive(true);
                        hpNum += gameObject.GetComponent<UnitStat>().buffs[i].z;
                        hpTxt.text = hpNum.ToString() + "%";
                        break;
                    case 2:                       
                        Atk.SetActive(true);
                        atkNum += gameObject.GetComponent<UnitStat>().buffs[i].z;
                        atkTxt.text = atkNum.ToString() + "%";
                        break;
                    case 3:
                        Def.SetActive(true);
                        defNum += gameObject.GetComponent<UnitStat>().buffs[i].z;
                        defTxt.text = defNum.ToString() + "%";
                        break;
                    case 4:                        
                        Cool.SetActive(true);
                        coolNum += gameObject.GetComponent<UnitStat>().buffs[i].z;
                        coolTxt.text = coolNum.ToString() + "%";
                        break;
                }
            }       
        }
    }
    public void offBuffDatas()
    {
        Atk.SetActive(false);
        Def.SetActive(false);
        Hp.SetActive(false);
        Cool.SetActive(false);
    }
}
