using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserPointer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    private RaycastHit[] rayCastResultsBuffer = new RaycastHit[20];
    private Vector3[] _lineRendererPositions = { Vector3.zero, Vector3.forward * 9999f };
    void Awake()
    {
        /*
        if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetPositions(_lineRendererPositions);
        _lineRenderer.startWidth = 0.05f;
        _lineRenderer.endWidth = 0.05f;
        */
    }

    /*
void UpdateLineRendererPoints()
{
    Vector3 startPos = this.transform.position;
    Vector3 endPos = startPos + this.transform.forward * 9999f;
    Ray ray = new Ray(startPos, this.transform.forward);
    int hitCount = Physics.RaycastNonAlloc(ray, rayCastResultsBuffer, 9999f, -1, QueryTriggerInteraction.Ignore);

    if (hitCount > 0)
    {
        var closestHit = rayCastResultsBuffer.Take(hitCount).OrderBy(hit => hit.distance).First();
        endPos = closestHit.point;
    }

    _lineRendererPositions[0] = startPos;
    _lineRendererPositions[1] = endPos;
    _lineRenderer.SetPositions(_lineRendererPositions);
}
        */
}
