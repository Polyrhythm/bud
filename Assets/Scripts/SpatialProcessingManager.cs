using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

/// <summary>
/// The SpatialProcessingTest class allows applications to scan the environment for a specified amount of time 
/// and then process the Spatial Mapping Mesh (find planes, remove vertices) after that time has expired.
/// </summary>
public class SpatialProcessingManager : Singleton<SpatialProcessingManager>
{
    [Tooltip("How much time (in seconds) that the SurfaceObserver will run after being started; used when 'Limit Scanning By Time' is checked.")]
    public float scanTime = 30.0f;

    [Tooltip("Material to use when rendering Spatial Mapping meshes while the observer is running.")]
    public Material defaultMaterial;

    [Tooltip("Optional Material to use when rendering Spatial Mapping meshes after the observer has been stopped.")]
    public Material secondaryMaterial;

    [Tooltip("Minimum number of floor planes required in order to exit scanning/processing mode.")]
    public uint minimumFloors = 1;

    [Tooltip("Navigation object to inform when mesh processing is complete.")]
    public GameObject navObj;

    /// <summary>
    /// Indicates if processing of the surface meshes is complete.
    /// </summary>
    private bool meshesProcessed = false;

    /// <summary>
    /// GameObject initialization.
    /// </summary>
    private void Start()
    {
        // Update surfaceObserver and storedMeshes to use the same material during scanning.
        SpatialMappingManager.Instance.SetSurfaceMaterial(defaultMaterial);

        // Register for the MakePlanesComplete event.
        SurfaceMeshesToPlanes.Instance.MakePlanesComplete += SurfaceMeshesToPlanes_MakePlanesComplete;
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void StartProcessing()
    {
        // Call CreatePlanes() to generate planes.
        CreatePlanes();

        // Set meshesProcessed to true.
        meshesProcessed = true;
    }

    // Logic to run after processing is finished.
    void OnProcessingComplete()
    {
        SpatialMappingManager.Instance.drawVisualMeshes = false;
        GameController.Instance.SendMessage("OnMeshProcessingComplete");
    }

    /// <summary>
    /// Handler for the SurfaceMeshesToPlanes MakePlanesComplete event.
    /// </summary>
    /// <param name="source">Source of the event.</param>
    /// <param name="args">Args for the event.</param>
    private void SurfaceMeshesToPlanes_MakePlanesComplete(object source, System.EventArgs args)
    {
        // Collection of floor planes that we can use to set horizontal items on.
        List<GameObject> floors = new List<GameObject>();
        floors = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Floor);

        // Check to see if we have enough floors (minimumFloors) to start processing.
        if (floors.Count >= minimumFloors)
        {
            // Set floor planes on Ground layer for pathfinding.
            foreach (GameObject floor in floors)
            {
                floor.layer = 8;
            }

            // Set wall planes to Obstacle layer for navigation avoidance.
            List<GameObject> walls = new List<GameObject>();
            walls = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Wall);   

            if (walls.Count > 0)
            {
                foreach (GameObject wall in walls)
                {
                    wall.layer = 9;
                }
            }

            // Reduce our triangle count by removing any triangles
            // from SpatialMapping meshes that intersect with active planes.
            RemoveVertices(SurfaceMeshesToPlanes.Instance.ActivePlanes);

            // After scanning is over, switch to the secondary (occlusion) material.
            SpatialMappingManager.Instance.SetSurfaceMaterial(secondaryMaterial);
            OnProcessingComplete();
        }
        else
        {
            // Re-enter scanning mode so the user can find more surfaces before processing.
            SpatialMappingManager.Instance.StartObserver();

            // Re-process spatial data after scanning completes.
            meshesProcessed = false;
        }
    }

    /// <summary>
    /// Creates planes from the spatial mapping surfaces.
    /// </summary>
    private void CreatePlanes()
    {
        // Generate planes based on the spatial map.
        SurfaceMeshesToPlanes surfaceToPlanes = SurfaceMeshesToPlanes.Instance;
        if (surfaceToPlanes != null && surfaceToPlanes.enabled)
        {
            surfaceToPlanes.MakePlanes();
        }
    }

    /// <summary>
    /// Removes triangles from the spatial mapping surfaces.
    /// </summary>
    /// <param name="boundingObjects"></param>
    private void RemoveVertices(IEnumerable<GameObject> boundingObjects)
    {
        RemoveSurfaceVertices removeVerts = RemoveSurfaceVertices.Instance;
        if (removeVerts != null && removeVerts.enabled)
        {
            removeVerts.RemoveSurfaceVerticesWithinBounds(boundingObjects);
        }
    }

    /// <summary>
    /// Called when the GameObject is unloaded.
    /// </summary>
    private void OnDestroy()
    {
        if (SurfaceMeshesToPlanes.Instance != null)
        {
            SurfaceMeshesToPlanes.Instance.MakePlanesComplete -= SurfaceMeshesToPlanes_MakePlanesComplete;
        }
    }
}