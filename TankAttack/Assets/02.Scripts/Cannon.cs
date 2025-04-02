using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Resources����: ���ӿ� ���� ���Ǵ� ���ҽ��� �������� �ҷ��� ����� �� ������
public class Cannon : MonoBehaviour
{
    public float speed = 6000.0f;
    public GameObject expEffect; //����ȿ�� ������ ���� ������Ʈ
    CapsuleCollider _collider;
    Rigidbody _rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();

        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        //3�ʰ� ���� �� �ڵ����� �����ϴ� �ڷ�ƾ ����
        StartCoroutine(this.ExplosionCannon(3.0f));
    }
    IEnumerator ExplosionCannon(float tm)
    {
        //tm(3��)��
        yield return new WaitForSeconds(tm);
        //�浹 �ݹ� �Լ��� �߻����� �ʵ��� Collider�� ��Ȱ��ȭ
        _collider.enabled = false;
        //�������� ������ ���� �ʿ� ����
        _rigidbody.isKinematic = true;
        //���� ������ ���� ����
        GameObject obj =(GameObject)Instantiate(expEffect, transform.position, Quaternion.identity);
        //1�� �� ����ȿ�� �����
        Destroy(obj, 1.0f);
        //1�ʵ� ĳ�� �����(Trail Renderer�� �Ҹ�ɶ����� ��ٷ���
        Destroy(this.gameObject, 1.0f);
    }
    private void OnTriggerEnter(Collider other)
    {

        //���� �Ǵ� �� ��ũ�� �浹�� ��� ��� �����ϵ��� �ڷ�ƾ ����
        StartCoroutine(this.ExplosionCannon(0.0f));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
