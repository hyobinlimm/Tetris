using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FristSelectUI : MonoBehaviour
{
    private void OnEnable()
    {
        // ���� ���� �� �Ҹ��� �Լ�
        var ES = FindObjectOfType<EventSystem>().firstSelectedGameObject;
        ES = this.gameObject; 
    }
}
