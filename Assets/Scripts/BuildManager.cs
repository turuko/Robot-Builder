using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    public List<RobotComponent> AvailableComponents;

    public RobotComponent SelectedComponent;

    [SerializeField]
    private GameObject ComponentMenu;
    [SerializeField]
    private GameObject Crosshair;
    [SerializeField]
    private ComponentSelector ComponentSelector;
    [SerializeField]
    private GameObject ComponentPreview;
    [SerializeField]
    private GameObject ComponentPreviewParent;

    [SerializeField] private RobotRoot Root;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Two BuildManagers");
        }
        Instance = this;
    }

    public void BuildComponent(Transform point, Quaternion rotation, int i)
    {
        Instantiate(SelectedComponent, point.position, rotation, point).name = SelectedComponent.name + " " + i;
        Root.Mass += SelectedComponent.Mass;
        point.GetComponent<Collider>().enabled = false;
    }

    public void RemoveComponent(RobotComponent component)
    {
        component.GetComponentInParent<Collider>().enabled = true;
        
        foreach (var robotComponent in component.GetComponentsInChildren<RobotComponent>())
        {
            robotComponent.transform.SetParent(component.transform.parent);
        }
        
        Destroy(component.gameObject);
    }

    public void OpenComponentMenu()
    {
        for (int i = 0; i < AvailableComponents.Count; i++)
        {
            var robotComponent = AvailableComponents[i];

            var preview = Instantiate(ComponentPreview, ComponentPreviewParent.transform);
            preview.transform.localPosition = new Vector3(0, 0, i * 10);
            preview.GetComponentInChildren<Camera>().targetTexture = new RenderTexture(256,256,32);
            

            var compGO = Instantiate(robotComponent, preview.transform);
            compGO.AddComponent<Rotator>().rotationSpeed = 30f;
            compGO.GetComponentInChildren<MeshFilter>().transform.localPosition = Vector3.zero;
            preview.name = compGO.name.Split(' ')[0];
            
            var selector = Instantiate(ComponentSelector, ComponentMenu.transform);

            selector.GetComponentInChildren<RawImage>().texture =
                preview.GetComponentInChildren<Camera>().targetTexture;
            
            selector.GetComponentInChildren<TextMeshProUGUI>().text = robotComponent.name.Split(' ')[0];
            selector.RobotComponent = robotComponent;
        }
        
        ComponentMenu.SetActive(true);
        Crosshair.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void CloseComponentMenu()
    {
        ComponentMenu.SetActive(false);
        Crosshair.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        foreach (Transform child in ComponentMenu.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in ComponentPreviewParent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}