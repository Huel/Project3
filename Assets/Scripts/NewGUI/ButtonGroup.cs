using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButtonGroup : MonoBehaviour {

    private List<dfButton> _items;
    public dfButton AutoFocus;
    public event Action BackButton;

	void Awake ()
	{
	    Init();
	}

    private void Init()
    {
        if (AutoFocus)
            AutoFocus.Focus();
        _items = GetComponentsInChildren<dfButton>().ToList();

        foreach (dfButton item in _items)
        {
            // Attach keyboard navigation events
            item.KeyDown += (dfControl sender, dfKeyEventArgs args) =>
            {
                if (args.Used)
                    return;

                if (args.KeyCode == KeyCode.DownArrow)
                {
                    SelectNext(sender, "Down");
                    args.Use();
                }
                else if (args.KeyCode == KeyCode.UpArrow)
                {
                    SelectNext(sender, "Up");
                    args.Use();
                }
                else if (args.KeyCode == KeyCode.LeftArrow)
                {
                    SelectNext(sender,"Left");
                    args.Use();
                }
                else if (args.KeyCode == KeyCode.RightArrow)
                {
                    SelectNext(sender,"Right");
                    args.Use();
                }
                else if (args.KeyCode == KeyCode.Space || args.KeyCode == KeyCode.Return)
                {
                    args.Use();
                }

            };

        }

       
    }

    private void SelectNext(dfControl item, string direction)
    {
        MenuButton button = item.GetComponent<MenuButton>();
        if (button)
        {
            dfButton nextButton = button[direction];
            if (nextButton)
                nextButton.Focus();
        }
    }


    void Update()
    {
        if (Input.GetButtonDown(InputTags.skill2) && BackButton != null)
            BackButton();
    }
}
