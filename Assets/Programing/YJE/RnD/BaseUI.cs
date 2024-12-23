using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UI Binding 진행
// 다른 클래스에서 BaseUI 상속하는 경우 UI의 내용을 전부 포함할 수 있도록 제작
// ex) public class TestUI : BaseUI
public class BaseUI : MonoBehaviour
{
    // 자식으로 있을 GameObject의 Dictionary 
    private Dictionary<string, GameObject> gameObjectDic;
    // 자식으로 있을 GameObject의 Component를 미리 받아 저장하는 Dictionary
    private Dictionary<string, Component> componentDic;


    // private void Awake()로 사용하는 경우
    // 상속하고 있는 자식에서 Bind()가 일어나지 않을 수 있음
    protected virtual void Awake() 
    {
        // UI가 실행되자마자 Binding과정을 진행
        Bind();
    }

    /// <summary>
    /// UI에 있는 모든 자식들을 확인하여 보관
    /// </summary>
    private void Bind()
    {
        // Transform Componenet가 없는 GameObject는 없으므로 Transform기준으로 불러오기
        //                                                          (true) => 비활성화 된 Component까지 불러오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        // transforms.Length << 1 : Dictionary의 크기는 transforms배열의 2배 << 2 = 4배
        // 너무 과하게 설정하는 경우 메모리적으로 좋지 않음
        gameObjectDic = new Dictionary<string, GameObject>(transforms.Length << 2);
        componentDic = new Dictionary<string, Component>();
        // transforms의 배열의 GameObject를 전부 gameObjectDic에 저장
        foreach(Transform t in transforms)
        {
            // 동일한 이름이 있을 수 있으니 TryAdd() 사용
            gameObjectDic.TryAdd(t.gameObject.name, t.gameObject);
        }
    }

    /// <summary>
    /// 원하는 UI를 찾는 기능을 구현
    /// 1. GetUI(name) : name이름의 게임오브젝트 가져오기
    /// 2. GetUI<T>(name) : 이름이 name인 UI에서 컴포넌트 T를 가져오기 2222222 
    ///     ex) GetUI<Image>("name") : 이름이 name인 오브젝트에서 컴포넌트 T를 가져오기
    /// </summary>
    public GameObject GetUI(in string name)
    {
        // 이름으로 가져온 Dictionary에서 이름으로 GameObject를 찾아서 반환
        gameObjectDic.TryGetValue(name, out GameObject obj);
        // name이 없는 경우 null로 반환
        return obj;
    }
    
    public T GetUI<T>(in string name) where T : Component
    {
        string key = $"{name}_{typeof(T).Name}";

        // 1. componentDic에 없는 경우 : 찾은 후 딕셔너리에 추가 후 반환
        componentDic.TryGetValue(key, out Component component);
        if(component != null) return component as T;

        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        if (gameObject = null) return null;

        // 2. componentDic에 이미 있는 경우 (= 이미 찾아본 적이 있는 경우) : 찾았던 것을 반환
        component = gameObject.GetComponent<T>();
        if (component == null) return null;
        componentDic.TryAdd(key, component);
        return component as T;



        // 1. componentDic에 없는 경우 : 찾은 후 딕셔너리에 추가 후 반환
        /*if (componentDic.ContainsKey(key))
        {
            return componentDic[key] as T;
        }*/
        // 2. componentDic에 이미 있는 경우 (= 이미 찾아본 적이 있는 경우) : 찾았던 것을 반환
        /*else
        {
            T component = gameObjectDic[name].GetComponent<T>();
            if(component != null)
            {
                componentDic.TryAdd(key, component);
                return component;
            }
            else
            {
                return null;
            }
        }*/



    }
}
