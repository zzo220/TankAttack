using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackAnim : MonoBehaviour
{
    float scrollSpeed = 1.0f;
    Renderer _renderer; //렌더러 컴포넌트
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //위아래키 입력받아 offset으로 활용
        float offset = Time.time * scrollSpeed * Input.GetAxisRaw("Vertical");
        //기본 텍스처의 Y 오프셋 값 변경
        _renderer.material.SetTextureOffset("_MainTex", new Vector2(0, offset));
        //노말 텍스터의 Y 오프셋 값 변경
        _renderer.material.SetTextureOffset("_BumpMap", new Vector2(0, offset));
    }
}
