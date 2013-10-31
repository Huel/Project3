using System;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    public event Action Back;
    private dfControl _control;
    public bool Enabled = false;
    public bool NeedFocus = true;

    void Awake()
    {
        _control = GetComponent<dfControl>();
    }

    void Update()
    {
        if (Enabled && (!NeedFocus || _control.HasFocus) && Input.GetButtonDown(InputTags.skill2) && Back != null)
        {
            Debug.Log("Back");
            Back();
        }
    }

    public void Enable()
    {
        Enabled = true;
    }

    public void Disable()
    {
        Enabled = false;
    }

    public void Use()
    {
        if (Enabled && (!NeedFocus || _control.HasFocus) && Back != null)
            Back();
    }
}
