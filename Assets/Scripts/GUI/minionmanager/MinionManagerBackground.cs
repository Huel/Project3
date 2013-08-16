using UnityEngine;
using System.Collections;

public class MinionManagerBackground : MonoBehaviour
{
    [SerializeField] 
    private GameObject backgroundContent; 

    private UISprite[] blueProgressBars = new UISprite[3];
    private UISprite[] bombs = new UISprite[3];
    private UISprite redProgressBar;
    private UISprite background;

    private class MinionManagerTags
    {
        public const string background = "minionmanager_background_map";
        public const string bomb1 = "minionmanager_bomb1";
        public const string bomb2 = "minionmanager_bomb2";
        public const string bomb3 = "minionmanager_bomb3";
        public const string redProgressBar = "red_progress";
        public const string blueProgressBar1 = "blue_progress_lane1";
        public const string blueProgressBar2 = "blue_progress_lane2";
        public const string blueProgressBar3 = "blue_progress_lane3";
    }

	void Start ()
    {
        background = backgroundContent.transform.FindChild(MinionManagerTags.background).GetComponent<UISprite>();
        bombs[0] = backgroundContent.transform.FindChild(MinionManagerTags.bomb1).GetComponent<UISprite>();
        bombs[1] = backgroundContent.transform.FindChild(MinionManagerTags.bomb2).GetComponent<UISprite>();
        bombs[2] = backgroundContent.transform.FindChild(MinionManagerTags.bomb3).GetComponent<UISprite>();
        redProgressBar = backgroundContent.transform.FindChild(MinionManagerTags.redProgressBar).GetComponent<UISprite>();
        blueProgressBars[0] = backgroundContent.transform.FindChild(MinionManagerTags.blueProgressBar1).GetComponent<UISprite>();
        blueProgressBars[1] = backgroundContent.transform.FindChild(MinionManagerTags.blueProgressBar2).GetComponent<UISprite>();
        blueProgressBars[2] = backgroundContent.transform.FindChild(MinionManagerTags.blueProgressBar3).GetComponent<UISprite>();
	}
	
	void Update ()
	{
	    if (background == null) return;
	    if (redProgressBar.transform.position != background.transform.position)
	    {
	        redProgressBar.transform.position = background.transform.position;
	        redProgressBar.transform.localScale = background.transform.localScale;
	    }
        foreach (UISprite bomb in bombs) bomb.alpha = 0;
	    foreach (UISprite blueProgressBar in blueProgressBars) blueProgressBar.alpha = 0;
	}
}
