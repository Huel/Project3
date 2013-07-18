using UnityEngine;
using System.Collections;

public interface IContactBehaviour
{
    void OnContact(GameObject minion, GameObject target);
}
