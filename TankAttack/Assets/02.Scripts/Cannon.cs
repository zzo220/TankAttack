using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Resources폴더: 게임에 자주 사용되는 리소스를 동적으로 불러와 사용할 때 유용함
public class Cannon : MonoBehaviour
{
    public float speed = 6000.0f;
    public GameObject expEffect; //폭발효과 프리팹 연결 오브젝트
    CapsuleCollider _collider;
    Rigidbody _rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();

        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        //3초가 지난 후 자동으로 폭발하는 코루틴 실행
        StartCoroutine(this.ExplosionCannon(3.0f));
    }
    IEnumerator ExplosionCannon(float tm)
    {
        //tm(3초)뒤
        yield return new WaitForSeconds(tm);
        //충돌 콜백 함수가 발생하지 않도록 Collider를 비활성화
        _collider.enabled = false;
        //물리엔진 영향을 받을 필요 없음
        _rigidbody.isKinematic = true;
        //폭발 프리팹 동적 생성
        GameObject obj =(GameObject)Instantiate(expEffect, transform.position, Quaternion.identity);
        //1초 뒤 폭발효과 사라짐
        Destroy(obj, 1.0f);
        //1초뒤 캐논 사라짐(Trail Renderer가 소멸될때까지 기다려줌
        Destroy(this.gameObject, 1.0f);
    }
    private void OnTriggerEnter(Collider other)
    {

        //지면 또는 적 탱크에 충돌한 경우 즉시 폭발하도록 코루틴 실행
        StartCoroutine(this.ExplosionCannon(0.0f));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
