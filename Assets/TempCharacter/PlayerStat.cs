using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �̰� ��ũ���ͺ� ������Ʈ�� ¥�°� ������
public class PlayerStat : MonoBehaviour
{
    [Header("�̵�")]
    public float moveSpeed;
    public float dashSpeed;
    public float dashDuration;
    public float dashCoolTime;

    [Header("������")]
    public int handAmount;
    public int stackAmount;
    public float stackinterval;

    [Header("���� �Ÿ�")]
    public float detectRange;

    [Header("���̾�")]
    public LayerMask blockLayer;
}
