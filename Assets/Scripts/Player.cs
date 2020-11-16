using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this) Destroy(gameObject);
    }

    private int _classicFansCount = 0;
    private int _jazzFansCount = 0;
    private int _streetFansCount = 0;

    private int _money = 0;

    private List<Dance> _storage = null;
    private List<Dance> _asset = null;
}
