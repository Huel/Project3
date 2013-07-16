using UnityEngine;
using System.Collections;

public class CameraHandling : MonoBehaviour
{
    private Transform Arrow;
    private Minion Minion1;
    private Minion Minion2;
    private Minion Minion3;
    private Minion Minion4;
    private Minion Minion5;
    private int RowSelected;
    private Minion[] Minions;
    private Transform GUI;
	// Use this for initialization
	void Start ()
    {
        GUI = transform.FindChild("GUI");
        RowSelected = 1;
        Minions = new Minion[5];
        Arrow = transform.FindChild("GUI").FindChild("Arrow");
        Minion1 = new Minion();
        Minion1.Content = transform.FindChild("GUI").FindChild("Minion1");
        Minion1.LaneSelected = 1;
        Minions[0] = Minion1;
        Minion2 = new Minion();
        Minion2.Content = transform.FindChild("GUI").FindChild("Minion2");
        Minion2.LaneSelected = 1;
        Minions[1] = Minion2;
        Minion3 = new Minion();
        Minion3.Content = transform.FindChild("GUI").FindChild("Minion3");
        Minion3.LaneSelected = 1;
        Minions[2] = Minion3;
        Minion4 = new Minion();
        Minion4.Content = transform.FindChild("GUI").FindChild("Minion4");
        Minion4.LaneSelected = 1;
        Minions[3] = Minion4;
        Minion5 = new Minion();
        Minion5.Content = transform.FindChild("GUI").FindChild("Minion5");
        Minion5.LaneSelected = 1;
        Minions[4] = Minion5;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(Input.GetKeyDown(KeyCode.F))
        {
            if (transform.FindChild("GUI").gameObject.activeInHierarchy)
            {
                transform.FindChild("GUI").gameObject.SetActive(false);
            }
            else
            {
                transform.FindChild("GUI").gameObject.SetActive(true);
            }
        }
        if (transform.FindChild("GUI").gameObject.activeInHierarchy)
        {

            if(Input.GetKeyDown(KeyCode.W) && RowSelected>1)
            {
                RowSelected--;
                Arrow.localPosition = new Vector3(Arrow.localPosition.x, 
                                                  transform.FindChild("GUI").FindChild("Minion"+RowSelected).localPosition.y, 
                                                  Arrow.localPosition.z);
            }

            if (Input.GetKeyDown(KeyCode.S) && RowSelected < 5)
            {
                RowSelected++;
                Arrow.localPosition = new Vector3(Arrow.localPosition.x,
                                                  transform.FindChild("GUI").FindChild("Minion" + RowSelected).localPosition.y,
                                                  Arrow.localPosition.z);
            }

            if (Input.GetKeyDown(KeyCode.D) && Minions[RowSelected - 1].LaneSelected < 3)
            {
                Minions[RowSelected - 1].LaneSelected++;
                Minions[RowSelected - 1].Content.localPosition = new Vector3(transform.FindChild("GUI").FindChild("Lane" + Minions[RowSelected - 1].LaneSelected).localPosition.x,
                                                                             Minions[RowSelected - 1].Content.localPosition.y,
                                                                             Minions[RowSelected - 1].Content.localPosition.z);
            }

            if (Input.GetKeyDown(KeyCode.A) && Minions[RowSelected - 1].LaneSelected > 1)
            {
                Minions[RowSelected - 1].LaneSelected--;
                Minions[RowSelected - 1].Content.localPosition = new Vector3(transform.FindChild("GUI").FindChild("Lane" + Minions[RowSelected - 1].LaneSelected).localPosition.x,
                                                                             Minions[RowSelected - 1].Content.localPosition.y,
                                                                             Minions[RowSelected - 1].Content.localPosition.z);
            }
            Debug.Log("Current RowSelected: "+RowSelected+", Current LaneSelected: "+Minions[0].LaneSelected+", "+Minions[1].LaneSelected+", "+Minions[2].LaneSelected+", "+Minions[3].LaneSelected+", "+Minions[4].LaneSelected);
        }
	}

    public struct Minion
    {
        public Transform Content;
        public int LaneSelected;
    }
}
