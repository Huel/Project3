using UnityEngine;

public class TextureChanger : MonoBehaviour
{
    public Texture texture1;
    public Texture texture2;

    // Use this for initialization
    void Start()
    {
        int tex = (int)(Random.value * 2);
        if (tex == 0)
            renderer.material.SetTexture("_MainTex", texture1);
        else
            renderer.material.SetTexture("_MainTex", texture2);
    }
}
