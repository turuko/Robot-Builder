using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public enum MouseMode
{
    Build,
    Menu,
    Combat
}
public class MouseManager : MonoBehaviour
{
    public MouseMode Mode
    {
        get;
        private set;
    }

    private Camera mainCamera;

    private int i = 0;
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        switch (Mode)
        {
            case MouseMode.Build:
                HandleBuildMouse();
                break;
            case MouseMode.Menu:
                HandleMenuMouse();
                break;
            case MouseMode.Combat:
                HandleCombatMouse();
                break;
            default:
                Debug.Log("Unexpected mouse mode");
                break;
        }
    }

    private void HandleBuildMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("Snap Points");
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, mask))
            {
                
                var roboComponent = hit.transform.parent.GetComponent<RobotComponent>();
                if ( roboComponent == null)
                {
                    var roboRoot = hit.transform.parent.GetComponent<RobotRoot>();
                    if(roboRoot == null)
                        return;
                }

                var spawnPoint = hit.transform;

                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                
                BuildManager.Instance.BuildComponent(spawnPoint, spawnRotation, i);
                i++;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var roboComponent = hit.transform.parent.GetComponent<RobotComponent>();
                
                if ( roboComponent == null)
                {
                    return;
                }
                
                BuildManager.Instance.RemoveComponent(roboComponent);
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            Mode = MouseMode.Menu;
            BuildManager.Instance.OpenComponentMenu();
        }

        CameraController.Instance.GetMousePos(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
    
    private void HandleMenuMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            
            EventSystem.current.RaycastAll(eventData,results);

            foreach (var result in results)
            {
                var selector = result.gameObject.GetComponent<ComponentSelector>();
                if ( selector != null)
                {
                    selector.HandleClick();
                    Mode = MouseMode.Build;
                }
            }
        }
    }
    
    private void HandleCombatMouse()
    {
        
    }
}
