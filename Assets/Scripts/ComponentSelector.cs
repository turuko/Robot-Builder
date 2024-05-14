using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComponentSelector : MonoBehaviour
{
    public RobotComponent RobotComponent;
    // Start is called before the first frame update
    public void HandleClick()
    {
        BuildManager.Instance.SelectedComponent = RobotComponent;
        BuildManager.Instance.CloseComponentMenu();
    }
}
