using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static MouseHover instance = null;
    public bool isUIHover = false;
    private void Awake()
    {
        instance = this;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isUIHover = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isUIHover = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
