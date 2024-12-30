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
    public LineRenderer trajectoryLineRenderer; // 추가: 궤적을 그릴 라인 렌더러
    public int trajectoryPointCount = 30;       // 추가: 궤적에서 몇 개의 점을 보여줄지
    public float trajectoryTimeStep = 0.05f;    // 추가: 점 사이 시간 간격

    void Start()
    {
        // 슬링샷 양쪽 고무줄
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        // 궤적 라인 렌더러 초기화
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

            // 탄성으로 인해 최대 길이 제한
            currentPosition = center.position +
                Vector3.ClampMagnitude(currentPosition - center.position, maxLength);

            // 수직 방향(bottomBoundary) 이하로 못 가도록
            currentPosition = ClampBoundary(currentPosition);

            // 고무줄 세팅
            SetStrips(currentPosition);

            // 새의 Collider 활성
            if (birdCollider)
            {
                birdCollider.enabled = true;
            }

            // ----- [추가] 드래그 중 궤적 표시 -----
            if (trajectoryLineRenderer != null)
            {
                // “지금 발사한다면” 적용될 속도
                Vector3 projectedVelocity = (currentPosition - center.position) * force * -1;

                // 새가 실제 위치하게 될 곳 (birdRigidbody.position)
                ShowTrajectory(birdRigidbody.position, projectedVelocity);
            }
        }
        else
        {
            ResetStrips();
            // 드래그가 끝났으므로 궤적 비활성화
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
    /// [추가] 드래그 중, "지금 발사한다면" 의 궤적을 보여주는 함수
    /// </summary>
    /// <param name="startPos">새가 시작되는 위치</param>
    /// <param name="startVelocity">예상 초기 속도</param>
    void ShowTrajectory(Vector3 startPos, Vector3 startVelocity)
    {
        if (!trajectoryLineRenderer) return;

        // 점 개수를 설정해 준다
        trajectoryLineRenderer.positionCount = trajectoryPointCount;

        // 물리 중력(2D)을 그대로 가져다 쓰는 경우
        Vector3 gravity = Physics2D.gravity;

        for (int i = 0; i < trajectoryPointCount; i++)
        {
            float t = i * trajectoryTimeStep;

            // 위치 = 시작위치 + (속도 * t) + (0.5 * g * t^2)
            Vector3 trajectoryPos = startPos +
                                    startVelocity * t +
                                    0.5f * gravity * (t * t);

            trajectoryLineRenderer.SetPosition(i, trajectoryPos);
        }

        // Trajectory 표시
        trajectoryLineRenderer.enabled = true;
    }
}
