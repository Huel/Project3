using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DamageMaterial : MonoBehaviour 
{
    public List<GameObject> meshes = new List<GameObject>();
    public List<SkinnedMeshRenderer> goRenderer = new List<SkinnedMeshRenderer>();
    
    private Material _damageMaterial;
    private Material _standardMaterial;

    private const float changingTime = 0.3f;

    private float _counter;

    void Awake()
    {
        _damageMaterial = new Material(Shader.Find("Diffuse"));
        _damageMaterial.color = new Color(1, 0, 0);

        _standardMaterial = meshes[0].GetComponent<SkinnedMeshRenderer>().material;

        foreach (GameObject mesh in meshes)
        {
            goRenderer.Add(mesh.GetComponent<SkinnedMeshRenderer>());
        }
    }

    [RPC]
    public void addDamageMaterial()
    {
        if (networkView.isMine)
        {
            foreach (SkinnedMeshRenderer meshRenderer in goRenderer)
                meshRenderer.material = _damageMaterial;

            _counter = 0;
        }
        else
            networkView.RPC("addDamageMaterial", RPCMode.OthersBuffered);
    }

    [RPC]
    private void deleteDamageMaterial()
    {
        if (networkView.isMine)
            foreach (SkinnedMeshRenderer meshRenderer in goRenderer)
                meshRenderer.material = _standardMaterial;
        else
            networkView.RPC("deleteDamageMaterial", RPCMode.Others);
    }
	
	//Update is called once per frame
    void Update ()
    {
        if (_counter < changingTime)
            _counter += Time.deltaTime;
        else
            deleteDamageMaterial();
    }
}
