using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    Transform tr; 
    Transform mainCameraTr;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        mainCameraTr = Camera.main.transform; //����ī�޶� ������Ʈ
        
    }

    // Update is called once per frame
    void Update()
    {
        tr.LookAt(mainCameraTr);
    }
}
