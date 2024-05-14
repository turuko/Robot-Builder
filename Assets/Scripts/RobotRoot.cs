using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JustAssets.ColliderUtilityRuntime;
using UnityEngine;

public class RobotRoot : MonoBehaviour
{
    public float Mass
    {
        get;
        set;
    }

    private void Start()
    {
        Mass = 1f;
    }

    private BoxCollider[,,] Colliders;
    private bool[,,] colliderMarked;
    
    //TODO remove:
    private BoxCollider temp;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(CreateRobot());
        }
    }

    private IEnumerator CreateRobot()
    {
        StartCoroutine(DestroyOverlappingColliders());
        StartCoroutine(DestroyDisabledColliders());
        yield return new WaitUntil(() => GetComponentsInChildren<BoxCollider>().All(col => col.enabled == true));
        CombineColliders();
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.mass = Mass;
        yield return null;
    }

    private IEnumerator DestroyOverlappingColliders()
    {
        Dictionary<string, Collider> knownPositions = new Dictionary<string, Collider>();
        foreach (var robotComponent in GetComponentsInChildren<RobotComponent>())
        {
            foreach (var boxCollider in robotComponent.GetComponentsInChildren<BoxCollider>())
            {
                if (boxCollider.transform.parent != robotComponent.transform)
                {
                    continue;
                }

                var key = boxCollider.transform.position.ToString();

                if (knownPositions.ContainsKey(key))
                {
                    Destroy(boxCollider);
                    Destroy(knownPositions[key]);
                    continue;
                }
                knownPositions.Add(key, boxCollider);

                yield return null;
            }
        }
    }

    private IEnumerator DestroyDisabledColliders()
    {
        foreach (var colliderInChild in GetComponentsInChildren<BoxCollider>())
        {
            if (colliderInChild.enabled ==false)
                Destroy(colliderInChild);
            yield return null;
        }
    }

    private void FloodFillColliders(BoxCollider initCollider)
    {
        var empty = new GameObject();
        empty.transform.SetParent(transform);

        var indexOfCollider = Colliders.IndexOf(initCollider);

        Stack<BoxCollider> stack = new Stack<BoxCollider>();
        stack.Push(initCollider);
        while (stack.Count > 0)
        {
            var n = stack.Pop();
            n.transform.SetParent(empty.transform);
            colliderMarked[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3] = true;
            if (n.size.x == 0)
            {
                if(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1] != null)
                    stack.Push(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1]);
                
                if(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3+1] != null)
                    stack.Push(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1]);
                
                if(Colliders[indexOfCollider.Item1, indexOfCollider.Item2-1, indexOfCollider.Item3] != null)
                    stack.Push(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1]);
                
                if(Colliders[indexOfCollider.Item1, indexOfCollider.Item2+1, indexOfCollider.Item3] != null)
                    stack.Push(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1]);
            }
            else if (n.size.y == 0)
            {
                if(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1] != null)
                    stack.Push(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1]);
                
                if(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3+1] != null)
                    stack.Push(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1]);
                
                if(Colliders[indexOfCollider.Item1-1, indexOfCollider.Item2, indexOfCollider.Item3] != null)
                    stack.Push(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1]);
                
                if(Colliders[indexOfCollider.Item1+1, indexOfCollider.Item2, indexOfCollider.Item3] != null)
                    stack.Push(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1]);
            }
            else if (n.size.z == 0)
            {
                if(Colliders[indexOfCollider.Item1, indexOfCollider.Item2-1, indexOfCollider.Item3] != null)
                    stack.Push(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1]);
                
                if(Colliders[indexOfCollider.Item1, indexOfCollider.Item2+1, indexOfCollider.Item3] != null)
                    stack.Push(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1]);
                
                if(Colliders[indexOfCollider.Item1-1, indexOfCollider.Item2, indexOfCollider.Item3] != null)
                    stack.Push(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1]);
                
                if(Colliders[indexOfCollider.Item1+1, indexOfCollider.Item2, indexOfCollider.Item3] != null)
                    stack.Push(Colliders[indexOfCollider.Item1, indexOfCollider.Item2, indexOfCollider.Item3-1]);
            }
        }
    }

    private void CombineColliders()
    {
        InitializeCollidersArray();
        
        foreach (var boxCollider in GetComponentsInChildren<BoxCollider>())
        {
            var index = Colliders.IndexOf(boxCollider);
            Debug.Log("Collider: "+ boxCollider.name + ", marked: " + colliderMarked[index.Item1, index.Item2, index.Item3]);
            if (!colliderMarked[index.Item1, index.Item2, index.Item3])
                FloodFillColliders(boxCollider);
        }
    }

    private void InitializeCollidersArray()
    {
        float minX = float.PositiveInfinity, minY = float.PositiveInfinity, minZ = float.PositiveInfinity;
        List<float> xes = new(), yes = new(), zes = new();
        Dictionary<string, BoxCollider> posToCollider = new Dictionary<string, BoxCollider>();
        var colliders = GetComponentsInChildren<BoxCollider>();
        temp = colliders[0];
        foreach (var colliderInChild in colliders)
        {
            var position = colliderInChild.transform.position;
            if (position.x < minX)
                minX = position.x;

            if (position.y < minY)
                minY = position.y;

            if (position.z < minZ)
                minZ = position.z;


            xes.Add(position.x);
            yes.Add(position.y);
            zes.Add(position.z);
            posToCollider.Add(position.ToString(), colliderInChild);
        }

        for (int i = 0; i < xes.Count; i++)
        {
            xes[i] -= minX;
            xes[i] *= 2;
            yes[i] -= minY;
            yes[i] *= 2;
            zes[i] -= minZ;
            zes[i] *= 2;
        }

        int xMax = Mathf.FloorToInt(xes.Max()), yMax = Mathf.FloorToInt(yes.Max()), zMax = Mathf.FloorToInt(zes.Max());
        var intXes = xes.Select(Mathf.FloorToInt).ToList();
        var intYes = yes.Select(Mathf.FloorToInt).ToList();
        var intZes = zes.Select(Mathf.FloorToInt).ToList();
        Colliders = new BoxCollider[xMax + 1, yMax + 1, zMax + 1];
        colliderMarked = new bool[xMax + 1, yMax + 1, zMax + 1];

        for (int x = 0; x < intXes.Count; x++)
        {
            for (int y = 0; y < intYes.Count; y++)
            {
                for (int z = 0; z < intZes.Count; z++)
                {
                    //TODO Colliders[x, y, z] = ?
                    var pos = new Vector3(xes[x] / 2 + minX, yes[y] / 2 + minY, zes[z] / 2 + minZ);
                    posToCollider.TryGetValue(pos.ToString(), out var col);

                    Colliders[intXes[x], intYes[y], intZes[z]] = col;
                }
            }
        }

        for (int i = 0; i < colliderMarked.GetLength(0); i++)
        {
            for (int j = 0; j < colliderMarked.GetLength(1); j++)
            {
                for (int k = 0; k < colliderMarked.GetLength(2); k++)
                {
                    colliderMarked[i, j, k] = false;
                }
            }
        }
    }
}
