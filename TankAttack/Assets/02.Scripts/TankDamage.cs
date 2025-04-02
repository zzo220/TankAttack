using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankDamage : MonoBehaviour
{
    MeshRenderer[] renderers;
    GameObject expEffect = null;

    int initHp = 100; //탱크의 초기 생명치
    int currHp = 0; //탱크의 현재 생명치

    public Canvas hudCanvas; //탱크의 Canvas 오브젝트
    public Image hpBar; //Filed 타입의 Image 오브젝트(hpBar)
    // Start is called before the first frame update
    void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        currHp = initHp;
        expEffect = Resources.Load<GameObject>("Exploson10");
        hpBar.color = Color.green; //처음 hpBar는 녹색
    }
    private void OnTriggerEnter(Collider other)
    {
        if (currHp > 0 && other.tag == "CANNON")//포탄과 충돌 시
        {
            currHp -= 20; // 체력을 20 감소
            //현재 생명치 백분율 = (현재 체력) / (최대 체력)
            hpBar.fillAmount = (float)currHp / (float)initHp;
            if (hpBar.fillAmount <= 0.4f) hpBar.color = Color.red;
            else if(hpBar.fillAmount <= 0.6f) hpBar.color = Color.yellow;

            if (currHp <= 0) // 체력이 0이하라면
            {
                //폭발처리 코루틴 함수 실행
                StartCoroutine(ExplosionTank());
            }
        }
    }
    IEnumerator ExplosionTank()
    {
        Debug.Log("코루틴 함수 실행");
        //탱크 폭발효과 프리팹 생성
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 3.0f); //폭발효과 3초뒤 사라짐
        hudCanvas.enabled = false; //탱크 HUD 투명화
        SetTankVisible(false); // 탱크 투명화
        yield return new WaitForSeconds(3.0f); //3초 간 대기
        hpBar.fillAmount = 1.0f;
        hpBar.color = Color.green;
        hudCanvas.enabled = true;
       currHp = initHp; //체력 충전
        SetTankVisible(true); //탱크 보이게..
        Debug.Log("코루틴 함수 종료");

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
