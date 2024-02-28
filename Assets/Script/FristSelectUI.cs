using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FristSelectUI : MonoBehaviour
{
    private void OnEnable()
    {
        // 껏다 켜질 때 불리는 함수
        var ES = FindObjectOfType<EventSystem>().firstSelectedGameObject;
        ES = this.gameObject; 
    }
}
