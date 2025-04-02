using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankDamage : MonoBehaviour
{
    MeshRenderer[] renderers;
    GameObject expEffect = null;

    int initHp = 100; //��ũ�� �ʱ� ����ġ
    int currHp = 0; //��ũ�� ���� ����ġ

    public Canvas hudCanvas; //��ũ�� Canvas ������Ʈ
    public Image hpBar; //Filed Ÿ���� Image ������Ʈ(hpBar)
    // Start is called before the first frame update
    void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        currHp = initHp;
        expEffect = Resources.Load<GameObject>("Exploson10");
        hpBar.color = Color.green; //ó�� hpBar�� ���
    }
    private void OnTriggerEnter(Collider other)
    {
        if (currHp > 0 && other.tag == "CANNON")//��ź�� �浹 ��
        {
            currHp -= 20; // ü���� 20 ����
            //���� ����ġ ����� = (���� ü��) / (�ִ� ü��)
            hpBar.fillAmount = (float)currHp / (float)initHp;
            if (hpBar.fillAmount <= 0.4f) hpBar.color = Color.red;
            else if(hpBar.fillAmount <= 0.6f) hpBar.color = Color.yellow;

            if (currHp <= 0) // ü���� 0���϶��
            {
                //����ó�� �ڷ�ƾ �Լ� ����
                StartCoroutine(ExplosionTank());
            }
        }
    }
    IEnumerator ExplosionTank()
    {
        Debug.Log("�ڷ�ƾ �Լ� ����");
        //��ũ ����ȿ�� ������ ����
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 3.0f); //����ȿ�� 3�ʵ� �����
        hudCanvas.enabled = false; //��ũ HUD ����ȭ
        SetTankVisible(false); // ��ũ ����ȭ
        yield return new WaitForSeconds(3.0f); //3�� �� ���
        hpBar.fillAmount = 1.0f;
        hpBar.color = Color.green;
        hudCanvas.enabled = true;
       currHp = initHp; //ü�� ����
        SetTankVisible(true); //��ũ ���̰�..
        Debug.Log("�ڷ�ƾ �Լ� ����");

    }
    void SetTankVisible(bool isVisible)
    {
        foreach (MeshRenderer _renderer in renderers)
        {
            _renderer.enabled = isVisible;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
