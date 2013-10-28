using System.Collections.Generic;
using UnityEngine;

public class BombGUI : MonoBehaviour
{
    public GameObject bombLane01;
    public GameObject bombLane02;
    public GameObject bombLane03;

    public GameObject mapArrowsLane01;
    public GameObject mapArrowsLane02;
    public GameObject mapArrowsLane03;

    public List<UISprite> guiProgressLanes = new List<UISprite>();
    public List<UISprite> guiPointerBombs = new List<UISprite>();
    public List<GameObject> bombs = new List<GameObject>();
    public List<GameObject> mapArrowsLanes = new List<GameObject>();

    private float team01X = 323;
    private float team02X = -70;

    // Use this for initialization
    void Awake()
    {
        guiProgressLanes.Add(transform.FindChild("blue_progress_lane01").GetComponent<UISprite>());
        guiProgressLanes.Add(transform.FindChild("blue_progress_lane02").GetComponent<UISprite>());
        guiProgressLanes.Add(transform.FindChild("blue_progress_lane03").GetComponent<UISprite>());
        guiPointerBombs.Add(transform.FindChild("bomb01").GetComponent<UISprite>());
        guiPointerBombs.Add(transform.FindChild("bomb02").GetComponent<UISprite>());
        guiPointerBombs.Add(transform.FindChild("bomb03").GetComponent<UISprite>());

        bombs.Add(bombLane01);
        bombs.Add(bombLane02);
        bombs.Add(bombLane03);
        mapArrowsLanes.Add(mapArrowsLane01);
        mapArrowsLanes.Add(mapArrowsLane02);
        mapArrowsLanes.Add(mapArrowsLane03);
    }

    // Update is called once per frame
    void Update()
    {
        ShowBombDirektion();
    }

    private void ShowBombDirektion()
    {
        for (int i = 0; i < bombs.Count; i++)
        {
            ShowArrows(bombs[i], mapArrowsLanes[i]);
            SetPosition(bombs[i], guiProgressLanes[i], guiPointerBombs[i]);
        }
    }

    private void ShowArrows(GameObject bomb, GameObject mapArrowsLane)
    {
        if (bomb == null) return;
        List<GameObject> valvesTeam01 = bomb.GetComponent<Bomb>().valvesA;
        List<GameObject> valvesTeam02 = bomb.GetComponent<Bomb>().valvesB;
        int closedValvesTeam01 = 0;
        int closedValvesTeam02 = 0;

        for (int i = 0; i < valvesTeam01.Count; i++)
        {
            if (valvesTeam01[i].GetComponent<Valve>().ValveState == ValveStates.Closed)
            {
                closedValvesTeam01++;
            }

            if (valvesTeam02[i].GetComponent<Valve>().ValveState == ValveStates.Closed)
            {
                closedValvesTeam02++;
            }
        }

        #region show Arrows

        int closedValves = closedValvesTeam01 - closedValvesTeam02;

        if (closedValves > 0)
        {
            switch (closedValves)
            {
                case 3:
                    {
                        for (int i = 0; i < closedValves; i++)
                        {
                            mapArrowsLane.GetComponent<LaneArrows>().blueArrows[i].gameObject.SetActive(true);
                            mapArrowsLane.GetComponent<LaneArrows>().redArrows[i].gameObject.SetActive(false);
                        }

                        break;
                    }
                case 2:
                    {
                        for (int i = 0; i < closedValves; i++)
                        {
                            mapArrowsLane.GetComponent<LaneArrows>().blueArrows[i].gameObject.SetActive(true);
                            mapArrowsLane.GetComponent<LaneArrows>().redArrows[i].gameObject.SetActive(false);
                        }

                        break;
                    }
                case 1:
                    {
                        for (int i = 0; i < closedValves; i++)
                        {
                            mapArrowsLane.GetComponent<LaneArrows>().blueArrows[i].gameObject.SetActive(true);
                            mapArrowsLane.GetComponent<LaneArrows>().redArrows[i].gameObject.SetActive(false);
                        }

                        break;
                    }
            }
        }
        else if (closedValves < 0)
        {
            closedValves *= -1;

            switch (closedValves)
            {
                case 3:
                    {
                        for (int i = 0; i < closedValves; i++)
                        {
                            mapArrowsLane.GetComponent<LaneArrows>().blueArrows[i].gameObject.SetActive(true);
                            mapArrowsLane.GetComponent<LaneArrows>().redArrows[i].gameObject.SetActive(false);
                        }

                        break;
                    }
                case 2:
                    {
                        for (int i = 0; i < closedValves; i++)
                        {
                            mapArrowsLane.GetComponent<LaneArrows>().blueArrows[i].gameObject.SetActive(true);
                            mapArrowsLane.GetComponent<LaneArrows>().redArrows[i].gameObject.SetActive(false);
                        }

                        break;
                    }
                case 1:
                    {
                        for (int i = 0; i < closedValves; i++)
                        {
                            mapArrowsLane.GetComponent<LaneArrows>().blueArrows[i].gameObject.SetActive(true);
                            mapArrowsLane.GetComponent<LaneArrows>().redArrows[i].gameObject.SetActive(false);
                        }

                        break;
                    }
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                mapArrowsLane.GetComponent<LaneArrows>().blueArrows[i].gameObject.SetActive(false);
                mapArrowsLane.GetComponent<LaneArrows>().redArrows[i].gameObject.SetActive(false);
            }
        }
        #endregion
    }

    private void SetPosition(GameObject bomb, UISprite guiProgressLanes, UISprite guiPointerBombs)
    {
        if (bomb.transform.position.x <= team01X && bomb.transform.position.x >= team02X)
        {
            float ratio = 1 - (bomb.transform.position.x - team02X) / (team01X - team02X);

            guiProgressLanes.fillAmount = ratio;
            guiPointerBombs.transform.localPosition = new Vector3(((ratio * 416) - 208 - 20), guiPointerBombs.transform.localPosition.y, (bomb.transform.localPosition.z - 14));
        }
        else
        {
            //gui._arrowBlue.gameObject.SetActive(false);
            //gui._arrowRed.gameObject.SetActive(false);

            bombs.Remove(bomb);
            this.guiProgressLanes.Remove(guiProgressLanes);
            this.guiPointerBombs.Remove(guiPointerBombs);
        }
    }
}
