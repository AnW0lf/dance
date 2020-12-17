using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else if (Instance != this) Destroy(gameObject);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    #region KEYS
    private const string LEVEL_NUMBER_KEY = "level_number_key";
    private const string LAST_LEVEL_KEY = "last_level_key";
    private const string CLASSIC_FANS_COUNT_KEY = "classic_fans_count_key";
    private const string STREET_FANS_COUNT_KEY = "street_fans_count_key";
    private const string JAZZ_FANS_COUNT_KEY = "jazz_fans_count_key";
    private const string MONEY_COUNT_KEY = "money_count_key";
    private const string PRICE_KEY = "price_key";
    private const string DANCE_BOUGHT_KEY = "dance_bought_key";
    private const string STORAGE_KEY = "storage_key";
    private const string ASSET_KEY = "asset_key";
    #endregion KEYS

    public const int MAX_ASSET_LENGTH = 8;
    public const string DEFAULT_ASSET_SET = "Chicken Dance|Gang Style|Nonstop Hip Hop|Quick Cancan|Maca- rena|Break Dance|Let's go Swing|Sunny Twist";

    [SerializeField] private List<Dance> _allDances = null;

    private int _classicFansCount = 0;
    private int _jazzFansCount = 0;
    private int _streetFansCount = 0;

    private int _money = 0;
    private int _price = 0;
    private int _danceBought = 0;

    private int _levelNumber = 1;
    private string _lastLevel = "Level1";

    #region Options
    public int LevelNumber
    {
        get => _levelNumber;
        set
        {
            if (_levelNumber != value)
            {
                _levelNumber = value;
                PlayerPrefs.SetInt(LEVEL_NUMBER_KEY, _levelNumber);
            }
        }
    }

    public string LastLevel
    {
        get => _lastLevel;
        set
        {
            if (_lastLevel != value)
            {
                _lastLevel = value;
                PlayerPrefs.SetString(LAST_LEVEL_KEY, _lastLevel);
            }
        }
    }

    public int Money
    {
        get => _money;
        set
        {
            if (_money != value)
            {
                _money = Mathf.Max(0, value);
                PlayerPrefs.SetInt(MONEY_COUNT_KEY, _money);
                OnMoneyChanged?.Invoke(Money);
            }
        }
    }

    public int Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                _price = value;
                PlayerPrefs.SetInt(PRICE_KEY, _price);
                OnPriceChanged?.Invoke(_price);
            }
        }
    }

    public int DanceBought
    {
        get => _danceBought;
        set
        {
            if (_danceBought != value)
            {
                _danceBought = value;
                PlayerPrefs.SetInt(DANCE_BOUGHT_KEY, _danceBought);
            }
        }
    }

    public int ClassicFansCount
    {
        get => _classicFansCount;
        set
        {
            if (_classicFansCount != value)
            {
                _classicFansCount = Mathf.Max(1, value);
                PlayerPrefs.SetInt(CLASSIC_FANS_COUNT_KEY, _classicFansCount);
                OnClassicFansCountChanged?.Invoke(ClassicFansCount);
            }
        }
    }

    public int JazzFansCount
    {
        get => _jazzFansCount;
        set
        {
            if (_jazzFansCount != value)
            {
                _jazzFansCount = Mathf.Max(1, value);
                PlayerPrefs.SetInt(JAZZ_FANS_COUNT_KEY, _jazzFansCount);
                OnJazzFansCountChanged?.Invoke(JazzFansCount);
            }
        }
    }

    public int StreetFansCount
    {
        get => _streetFansCount;
        set
        {
            if (_streetFansCount != value)
            {
                _streetFansCount = Mathf.Max(1, value);
                PlayerPrefs.SetInt(STREET_FANS_COUNT_KEY, _streetFansCount);
                OnStreetFansCountChanged?.Invoke(StreetFansCount);
            }
        }
    }

    public int AssetCount => Asset.Count;

    public int StorageCount => Storage.Count;

    public List<Dance> Asset { get; private set; } = null;

    public List<Dance> Storage { get; private set; } = null;

    public void SetAsset(List<Dance> asset)
    {
        Asset = new List<Dance>(asset);
        SaveAsset();
        OnAssetChanged?.Invoke(Asset);
    }

    public void SetStorage(List<Dance> storage)
    {
        Storage = new List<Dance>(storage);
        SaveStorage();
        OnStorageChanged?.Invoke(Storage);
    }
    #endregion Options

    #region Actions
    public UnityAction<int> OnMoneyChanged { get; set; } = null;
    public UnityAction<int> OnPriceChanged { get; set; } = null;
    public UnityAction<int> OnClassicFansCountChanged { get; set; } = null;
    public UnityAction<int> OnJazzFansCountChanged { get; set; } = null;
    public UnityAction<int> OnStreetFansCountChanged { get; set; } = null;
    public UnityAction<List<Dance>> OnAssetChanged { get; set; } = null;
    public UnityAction<List<Dance>> OnStorageChanged { get; set; } = null;
    #endregion Actions

    private void Load()
    {
        _classicFansCount = GetIntOrDefault(CLASSIC_FANS_COUNT_KEY, 1);
        _jazzFansCount = GetIntOrDefault(JAZZ_FANS_COUNT_KEY, 1);
        _streetFansCount = GetIntOrDefault(STREET_FANS_COUNT_KEY, 1);

        _money = GetIntOrDefault(MONEY_COUNT_KEY, 0);
        _price = GetIntOrDefault(PRICE_KEY, 10);
        _danceBought = GetIntOrDefault(DANCE_BOUGHT_KEY, 0);

        _levelNumber = GetIntOrDefault(LEVEL_NUMBER_KEY, 1);
        _lastLevel = GetStringOrDefault(LAST_LEVEL_KEY, "Level1");

        string asset_string = GetStringOrDefault(ASSET_KEY, DEFAULT_ASSET_SET);
        string storage_string = GetStringOrDefault(STORAGE_KEY, string.Empty);

        Asset = StringToDanceList(asset_string);
        Storage = StringToDanceList(storage_string);
    }

    private void Save()
    {
        PlayerPrefs.SetInt(CLASSIC_FANS_COUNT_KEY, _classicFansCount);
        PlayerPrefs.SetInt(JAZZ_FANS_COUNT_KEY, _jazzFansCount);
        PlayerPrefs.SetInt(STREET_FANS_COUNT_KEY, _streetFansCount);

        PlayerPrefs.SetInt(MONEY_COUNT_KEY, _money);
        PlayerPrefs.SetInt(PRICE_KEY, _price);
        PlayerPrefs.SetInt(DANCE_BOUGHT_KEY, _danceBought);

        PlayerPrefs.SetInt(LEVEL_NUMBER_KEY, _levelNumber);
        PlayerPrefs.SetString(LAST_LEVEL_KEY, _lastLevel);

        SaveAsset();
        SaveStorage();
    }

    public void CompleteLevel()
    {
        LevelNumber++;
    }

    #region Save/Load Tools
    private int GetIntOrDefault(string key, int @default)
    {
        if (!PlayerPrefs.HasKey(key))
            PlayerPrefs.SetInt(key, @default);
        return PlayerPrefs.GetInt(key);
    }
    private float GetFloatOrDefault(string key, float @default)
    {
        if (!PlayerPrefs.HasKey(key))
            PlayerPrefs.SetFloat(key, @default);
        return PlayerPrefs.GetFloat(key);
    }
    private string GetStringOrDefault(string key, string @default)
    {
        if (!PlayerPrefs.HasKey(key))
            PlayerPrefs.SetString(key, @default);
        return PlayerPrefs.GetString(key);
    }

    private List<Dance> StringToDanceList(string input)
    {
        if (string.IsNullOrEmpty(input)) return new List<Dance>();
        List<Dance> dances = new List<Dance>();
        string[] labels = input.Split('|');
        foreach (var label in labels)
        {
            Dance dance = _allDances.Find((d) => d.LabelText == label);
            if (dance != null) dances.Add(dance);
        }
        return dances;
    }

    private string DanceListToString(List<Dance> dances)
    {
        if (dances == null || dances.Count == 0) return string.Empty;
        StringBuilder sb = new StringBuilder();
        foreach (var dance in dances)
        {
            if (dance == null) continue;
            sb.Append(dance.LabelText);
            sb.Append("|");
        }
        return sb.ToString().TrimEnd('|');
    }

    private void SaveList(string key, List<Dance> list)
    {
        string list_string = DanceListToString(list);
        PlayerPrefs.SetString(key, list_string);
    }

    private void SaveAsset() => SaveList(ASSET_KEY, Asset);

    private void SaveStorage() => SaveList(STORAGE_KEY, Storage);
    #endregion Save/Load Tools

    public Dance RandomDance
    {
        get
        {
            var dances = _allDances.Where((dance) => dance.Level == 1).ToArray();
            return dances[Random.Range(0, dances.Length)];
        }
    }
}
