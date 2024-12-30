using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Slingshot Strips")]
    public LineRenderer[] lineRenderers;
    public Transform[] stripPositions;
    public Transform center;
    public Transform idlePostion;

    public Vector3 currentPosition;

    public float maxLength;
    public float bottomBoundary;
    bool isMouseDown;

    [Header("Bird Settings")]
    public GameObject birdPrefab;
    public float birdPositionOffset;
    Rigidbody2D birdRigidbody;
    Collider2D birdCollider;

    [Header("Shooting")]
    public float force;

    [Header("Trajectory Settings")]
    public LineRenderer trajectoryLineRenderer; // �߰�: ������ �׸� ���� ������
    public int trajectoryPointCount = 30;       // �߰�: �������� �� ���� ���� ��������
    public float trajectoryTimeStep = 0.05f;    // �߰�: �� ���� �ð� ����

    void Start()
    {
        // ������ ���� ����
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        // ���� ���� ������ �ʱ�ȭ
        if (trajectoryLineRenderer != null)
        {
            trajectoryLineRenderer.positionCount = 0;
            trajectoryLineRenderer.enabled = false;
        }

        CreateBird();
    }

    void CreateBird()
    {
        birdRigidbody = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
        birdCollider = birdRigidbody.GetComponent<Collider2D>();
        birdCollider.enabled = false;

        birdRigidbody.isKinematic = true;

        ResetStrips();
    }

    void Update()
    {
        if (isMouseDown)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;
            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // ź������ ���� �ִ� ���� ����
            currentPosition = center.position +
                Vector3.ClampMagnitude(currentPosition - center.position, maxLength);

            // ���� ����(bottomBoundary) ���Ϸ� �� ������
            currentPosition = ClampBoundary(currentPosition);

            // ���� ����
            SetStrips(currentPosition);

            // ���� Collider Ȱ��
            if (birdCollider)
            {
                birdCollider.enabled = true;
            }

            // ----- [�߰�] �巡�� �� ���� ǥ�� -----
            if (trajectoryLineRenderer != null)
            {
                // ������ �߻��Ѵٸ顱 ����� �ӵ�
                Vector3 projectedVelocity = (currentPosition - center.position) * force * -1;

                // ���� ���� ��ġ�ϰ� �� �� (birdRigidbody.position)
                ShowTrajectory(birdRigidbody.position, projectedVelocity);
            }
        }
        else
        {
            ResetStrips();
            // �巡�װ� �������Ƿ� ���� ��Ȱ��ȭ
            if (trajectoryLineRenderer != null)
                trajectoryLineRenderer.enabled = false;
        }
    }

    private void OnMouseDown()
    {
        isMouseDown = true;
    }

    private void OnMouseUp()
    {
        isMouseDown = false;
        Shoot();
    }

    void Shoot()
    {
        birdRigidbody.isKinematic = false;
        Vector3 birdForce = (currentPosition - center.position) * force * -1;
        birdRigidbody.velocity = birdForce;

        birdRigidbody = null;
        birdCollider = null;
        Invoke("CreateBird", 2);
    }

    void ResetStrips()
    {
        currentPosition = idlePostion.position;
        SetStrips(currentPosition);
    }

    void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);

        if (birdRigidbody)
        {
            Vector3 dir = position - center.position;
            birdRigidbody.transform.position = position + dir.normalized * birdPositionOffset;
            birdRigidbody.transform.right = -dir.normalized;
        }
    }

    Vector3 ClampBoundary(Vector3 vector)
    {
        vector.y = Mathf.Clamp(vector.y, bottomBoundary, 1000);
        return vector;
    }

    /// <summary>
    /// [�߰�] �巡�� ��, "���� �߻��Ѵٸ�" �� ������ �����ִ� �Լ�
    /// </summary>
    /// <param name="startPos">���� ���۵Ǵ� ��ġ</param>
    /// <param name="startVelocity">���� �ʱ� �ӵ�</param>
    void ShowTrajectory(Vector3 startPos, Vector3 startVelocity)
    {
        if (!trajectoryLineRenderer) return;

        // �� ������ ������ �ش�
        trajectoryLineRenderer.positionCount = trajectoryPointCount;

        // ���� �߷�(2D)�� �״�� ������ ���� ���
        Vector3 gravity = Physics2D.gravity;

        for (int i = 0; i < trajectoryPointCount; i++)
        {
            float t = i * trajectoryTimeStep;

            // ��ġ = ������ġ + (�ӵ� * t) + (0.5 * g * t^2)
            Vector3 trajectoryPos = startPos +
                                    startVelocity * t +
                                    0.5f * gravity * (t * t);

            trajectoryLineRenderer.SetPosition(i, trajectoryPos);
        }

        // Trajectory ǥ��
        trajectoryLineRenderer.enabled = true;
    }
}
