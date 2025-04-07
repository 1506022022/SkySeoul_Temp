using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scanner))]
public class ParkourController : MonoBehaviour
{
    public bool isDebugMode;

    Scanner scanner;

    public float DetectionRange = 12f;

    private float height = 1.6f;
    private float radius = 0.28f;

    Scanner.HitData data;
    private bool canZip;
    private Vector3 zipPoint;
    public Vector3 AdjustedZipPoint;
    public Vector3 hudPoint;
    public AnimationCurve movementCurve;

    public Action<Vector3,Vector3,float> OnZipMove;

    public Image zipMoveHud;

    private void Awake()
    {
        scanner = GetComponent<Scanner>();
    }

    private void Start()
    {
        OnZipMove += ZipLocationMove;
    }

    private void Update()
    {
        if (FindZipPoint() && canZip)
        {
            zipMoveHud.enabled = true;
            MoveToWorldPosition(hudPoint);
            

            if(Input.GetKeyDown(KeyCode.F))
            {
                canZip = false;
                OnZipMove?.Invoke(transform.position, AdjustedZipPoint, 1);
            }
        }
        else
        {
            zipMoveHud.enabled = false;
        }
    }

    private bool FindZipPoint()
    {
        Scanner.HitData data = scanner.ScanLayer();

        if (data.foundHitwithCorrectionNormal)
        {
            if (IsHitInsideObject(data.forwardHit.normal, data.heightHit.normal))
            {
                Debug.Log("감지 범위보다 적은 범위이므로 이동하기에 부적합합니다.");
                canZip = false;
                return false;
            }
            else
            {
                canZip = true;
                zipPoint = data.heightHit.point;
                AdjustedZipPoint = data.heightHit.point + Vector3.up;
                hudPoint = data.heightHit.point;
                return true;
            }
        }
        else // 3번째 로직을 수행한다.
        {
            canZip = false;
            return false;
        }
    }

    // 벡터의 범위가 감지 범위보다 크지 않으면 false
    private bool IsHitInsideObject(Vector3 inputVector1, Vector3 inputVector2)
    {
        Vector3 calcEnoughSpace = inputVector1 - inputVector2;

        return calcEnoughSpace.magnitude >= DetectionRange;
    }

    private bool CapsuleToZipPoint(Vector3 forwardHitVector)
    {
        return false;
    }

    private void OnDrawGizmos()
    {
       if(Application.isPlaying)
        {
            if(canZip)
                DrawSphere(zipPoint + Vector3.up);
        }
    }

    private void DrawSphere(Vector3 startPos)
    {
        if (!isDebugMode) return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(startPos + Vector3.up * (height / 2), radius);
        Gizmos.DrawWireSphere(startPos - Vector3.up * (height / 2), radius);

        Vector3 top = startPos + Vector3.up * (height / 2);
        Vector3 bottom = startPos - Vector3.up * (height / 2);

        // 원형 부분을 연결하는 선 그리기
        for (int i = 0; i < 360; i += 10) // 10도 간격으로 선을 그립니다.
        {
            float angle1 = i * Mathf.Deg2Rad;
            float angle2 = (i + 10) * Mathf.Deg2Rad;

            Vector3 point1Top = top + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * radius;
            Vector3 point2Top = top + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * radius;

            Vector3 point1Bottom = bottom + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * radius;
            Vector3 point2Bottom = bottom + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * radius;

            // 상단 원 연결
            Gizmos.DrawLine(point1Top, point2Top);
            // 하단 원 연결
            Gizmos.DrawLine(point1Bottom, point2Bottom);
            // 상단과 하단 연결
            Gizmos.DrawLine(point1Top, point1Bottom);
        }
    }

    private void ZipLocationMove(Vector3 start, Vector3 end, float time)
    {
        StartCoroutine(MoveBetweenPoints(transform.position, AdjustedZipPoint, 1));
    }

    public IEnumerator MoveBetweenPoints(Vector3 start, Vector3 end, float time)
    {
        float elapsedTime = 0f;
        while(elapsedTime <time)
        {
            elapsedTime += Time.deltaTime;

            // 보간 비율 계산 (0에서 1 사이)
            float t = elapsedTime / time;
            float curveValue = movementCurve.Evaluate(t);

            // 벡터 보간
            transform.position = Vector3.Lerp(start, end, curveValue);

            // 다음 프레임까지 대기
            yield return null;
        }

        transform.position = end;
    }

    public void MoveToWorldPosition(Vector3 target)
    {
        zipMoveHud.transform.position = scanner.cam.WorldToScreenPoint(target);
       
    }
}
