using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class PlaneColorChanger : MonoBehaviour
{
    public Material customMaterial; // Material que será aplicado nos planos

    private ARPlaneManager planeManager;

    void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
    }

    void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (ARPlane plane in args.added)
        {
            ApplyMaterial(plane);
        }

        foreach (ARPlane plane in args.updated)
        {
            ApplyMaterial(plane);
        }
    }

    void ApplyMaterial(ARPlane plane)
    {
        var renderer = plane.GetComponent<MeshRenderer>();
        if (renderer != null && customMaterial != null)
        {
            renderer.material = customMaterial;
        }
    }
}
