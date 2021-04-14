using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForceSelect : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    private Selectable selectable;

    private void Start()
    {
        selectable = GetComponent<Selectable>();
    }

    private void Update()
    {
        if(eventSystem.currentSelectedGameObject == null)
        {
            selectable.Select();
        }
        
    }
}
