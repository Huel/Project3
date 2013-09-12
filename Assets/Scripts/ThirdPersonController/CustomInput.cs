using UnityEngine;
using System.Collections;

public class CustomInput {

    private static bool triggerOneDown = false;
    private static string triggerOneTag;
    private static bool triggerTwoDown = false;
    private const float TRIGGER_THRESHOLD = 0.01f;

    public static bool GetTriggerDown(string triggerTag)
    {
        if (triggerOneTag == null)
            triggerOneTag = triggerTag;

        if (Input.GetAxis(triggerTag) <= TRIGGER_THRESHOLD)
        {
            if (triggerOneTag == triggerTag)
            {
                triggerOneDown = false;
            }
            else
                triggerTwoDown = false;
            return false;
        }

        if (triggerOneTag == triggerTag)
        {
            if (!triggerOneDown)
            {
                triggerOneDown = true;
                return true;
            }
        }
        else if (!triggerTwoDown)
        {
            triggerTwoDown = true;
            return true;
        }
        return false;

    }

    public static bool GetTrigger(string triggerTag)
    {
        return Input.GetAxis(triggerTag) >= TRIGGER_THRESHOLD;
    }
}
