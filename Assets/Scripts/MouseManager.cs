using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private Vector3 _mouseOrigin;
    public float panSpeed = 1;
    public float zoomSpeed = 10;
    public float minZoom = 20;
    public float maxZoom = 100;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // get mousePosition on rightclick
        if (Input.GetMouseButtonDown(1))
        {
            _mouseOrigin = Input.mousePosition;
        }

        // move camera while richtclick pressed
        if (Input.GetMouseButton(1))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);
            Vector3 panTarget = Camera.main.transform.position + new Vector3(-pos.y * panSpeed, 0, pos.x * panSpeed);
            if (OverTerrain(panTarget))
            {
                Camera.main.transform.position = panTarget;
            }
        }

        // zoom camera with mousewheel
        if (Input.mouseScrollDelta.y != 0)
        {
            Vector3 pos = Camera.main.transform.position;
            float scroll = Input.mouseScrollDelta.y * zoomSpeed;
            var zoomTarget = Camera.main.transform.position + Camera.main.transform.forward * scroll;
            if (zoomTarget.y > minZoom && zoomTarget.y < maxZoom)
            {
                Camera.main.transform.position = zoomTarget;
            }
        }

        // get tilename on left click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxZoom + 100, 1 << LayerMask.NameToLayer("Tiles")))
            {
                Debug.Log(hit.collider.gameObject.name);
            }
        }
    }
    
    /// <summary>
    /// Checks if a given position is over a tile
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <returns>Returns true if position is over a tile, otherwise false</returns>
    private bool OverTerrain(Vector3 position)
    {
        // Debug.DrawRay(position, new Vector3(0, -1000, 0), Color.red, 20);
        return Physics.Raycast(position, new Vector3(0, -1, 0), maxZoom + 100, 1 << LayerMask.NameToLayer("Tiles"));
    }
}