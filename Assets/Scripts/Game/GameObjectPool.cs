using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{    
    [SerializeField] private GameObject root;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int count;        

    private List<GameObject> items = new List<GameObject>();
    
    public GameObject Get()
    {
        for( int i = 0; i < items.Count; i++ ) {
            if (!items[i].activeSelf)
            {
                items[i].SetActive(true);
                return items[ i ];
            }
        }

        items.Add(AddChild(root, prefab));
        items[items.Count - 1].SetActive(true);
        return items[ items.Count - 1 ];
    }

    public void HideAll()
    {
        for( int i = 0; i < items.Count; i++ ) {
            items[i].SetActive( false );
        }
    }

    private void Start()
    {
        if (count > 0) Generate();
    }

    private void Generate()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i]);
        }
        items.Clear();

        for (int i = 0; i < count; i++)
        {
            var obj = AddChild(root, prefab);
            obj.SetActive(false);
            items.Add(obj);
        }
    }

    private GameObject AddChild(GameObject parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab, parent.transform) as GameObject;

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }
        return go;
    }
}