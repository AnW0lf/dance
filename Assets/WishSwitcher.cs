using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WishSwitcher : MonoBehaviour
{
    public bool wishEnabled;

    // Start is called before the first frame update
    void Start()
    {
        if (!wishEnabled)
        {
            GetComponentInChildren<FanController>()._chancePerSecond = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
