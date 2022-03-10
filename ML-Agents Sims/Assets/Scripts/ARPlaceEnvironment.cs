using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class ARPlaceEnvironment : MonoBehaviour
{
    [SerializeField] private GameObject _prefabToPlace;
    private ARRaycastManager _raycastManager;
    private ARAnchorManager _anchorManager;
    private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();

    protected void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
        _anchorManager = GetComponent<ARAnchorManager>();
        //_planeManager = GetComponent<ARPlaneManager>();
    }

    protected void Update()
    {
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began) { return; }

        const TrackableType trackableTypes =
            TrackableType.FeaturePoint |
            TrackableType.PlaneWithinPolygon;

        if (_raycastManager.Raycast(touch.position, Hits, trackableTypes))
        {
            CreateAnchor(Hits[0]);
        }


        ARAnchor CreateAnchor(in ARRaycastHit hit)
        {
            ARAnchor anchor;
            if (hit.trackable is ARPlane hitPlane)
            {
                var oldPrefab = _anchorManager.anchorPrefab;
                _anchorManager.anchorPrefab = _prefabToPlace;
                anchor = _anchorManager.AttachAnchor(hitPlane, hit.pose);
                _anchorManager.anchorPrefab = oldPrefab;

                Debug.Log($"Created anchor attachment for plane (id: {anchor.nativePtr}).");
            }
            else
            {
                var instantiatedObject = Instantiate(_prefabToPlace, hit.pose.position, hit.pose.rotation);

                // Make sure the new GameObject has an ARAnchor component.
                if (!instantiatedObject.TryGetComponent<ARAnchor>(out anchor))
                {
                    anchor = instantiatedObject.AddComponent<ARAnchor>();
                }
                Debug.Log($"Created regular anchor (id: {anchor.nativePtr}).");
            }

            return anchor;
        }
    }
}