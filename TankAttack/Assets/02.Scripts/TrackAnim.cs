using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackAnim : MonoBehaviour
{
    float scrollSpeed = 1.0f;
    Renderer _renderer; //������ ������Ʈ
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //���Ʒ�Ű �Է¹޾� offset���� Ȱ��
        float offset = Time.time * scrollSpeed * Input.GetAxisRaw("Vertical");
        //�⺻ �ؽ�ó�� Y ������ �� ����
        _renderer.material.SetTextureOffset("_MainTex", new Vector2(0, offset));
        //�븻 �ؽ����� Y ������ �� ����
        _renderer.material.SetTextureOffset("_BumpMap", new Vector2(0, offset));
    }
}
