using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DamageMaterial : MonoBehaviour 
{
    public List<GameObject> meshes = new List<GameObject>();
    private List<SkinnedMeshRenderer> goRenderer = new List<SkinnedMeshRenderer>();
    
    private Color Red = new Color(1, 0, 0);
    private Color White = new Color(1, 1, 1);

    private bool normal;

    private const float changingTime = 0.3f;

    private float _counter;

    //void Awake()
    //{
    //    foreach (GameObject mesh in meshes)
    //    {
    //        goRenderer.Add(mesh.GetComponent<SkinnedMeshRenderer>());
    //    }
    //}

    //[RPC]
    //public void addDamageMaterial()
    //{
    //    if (networkView.isMine)
    //    {
    //        foreach (SkinnedMeshRenderer meshRenderer in goRenderer)
    //            meshRenderer.material.color = Red;

    //        _counter = 0;
    //        normal = false;
    //    }
    //    else
    //        networkView.RPC("addDamageMaterial", RPCMode.OthersBuffered);
    //}

    //[RPC]
    //private void deleteDamageMaterial()
    //{
    //    if (networkView.isMine)
    //    {
    //        foreach (SkinnedMeshRenderer meshRenderer in goRenderer)
    //            meshRenderer.material.color = White;
    //        normal = true;
    //    }
    //    else
    //        networkView.RPC("deleteDamageMaterial", RPCMode.OthersBuffered);
    //}

    ////Update is called once per frame
    //void Update()
    //{
    //    if (_counter < changingTime)
    //        _counter += Time.deltaTime;
    //    else if (!normal)
    //        deleteDamageMaterial();
    //}
}
