using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForceSelect : MonoBehaviour
{
    [SerializeField] private bool _isEnableForceSelect;
    [SerializeField] private EventSystem eventSystem;
    private Selectable selectable;

    private void Start()
    {
        selectable = GetComponent<Selectable>();
    }

    private void OnEnable()
    {
        if (_isEnableForceSelect) {
            if(selectable == null) {
                selectable = GetComponent<Selectable>();
            }
            selectable.Select();
        }
    }

    private void Update()
    {
        if(eventSystem.currentSelectedGameObject == null && gameObject.activeSelf)
        {
            selectable.Select();
        }
        
    }
}
