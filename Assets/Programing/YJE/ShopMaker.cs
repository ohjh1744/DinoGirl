using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMaker : MonoBehaviour
{
    [SerializeField] GachaSceneController gachaSceneController;
    private Dictionary<int, ShopChar> charDictionary;
    private RectTransform characterContent; // 구매 캐릭터 프리팹이 생성 될 위치
    [SerializeField] GameObject shopCharPrefab;

    public void ShopCharMaker()
    {
        // 캐릭터 리스트를 받아서 모두 BuyCharacter 프리팹으로 생성하기
        charDictionary = gachaSceneController.CharDictionary; // 캐릭터 전체 리스트 설정
        characterContent = gachaSceneController.GetUI<RectTransform>("CharacterContent"); // 구매 캐릭터 프리팹이 생성 될 위치 설정
        
        for(int i = 1; i <= charDictionary.Count; i++)
        {
            GameObject shopCharUI = Instantiate(shopCharPrefab, characterContent);
            ShopChar shopChar = shopCharUI.GetComponent<ShopChar>();
            charDictionary.TryGetValue(i, out shopChar);

            shopCharUI = shopChar.SetShopCharUI(shopChar, shopCharUI);
        }
    }


}
