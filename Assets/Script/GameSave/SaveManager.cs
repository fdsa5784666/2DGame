using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("配置")]
    [SerializeField]
    private string saveFileName = "save.json";
    [SerializeField]
    private bool useEncryption = false; // 是否使用加密
    [SerializeField]
    private string encryptionKey = "my_secret_key"; // 加密密钥

    private string savePath;
    private GameSave currentSave;

    public event Action<GameSave> OnSaveLoaded;
    public event Action<GameSave> OnSaveSaved;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        Debug.Log($"存档路径:{savePath}");
        LoadOrCreateSave();

    }

    private void Start()
    {
        
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveGame();
    }
    private void OnApplicationQuit()
    {
        SaveGame();

    }
    /// <summary>
    /// 加载或创建存档 （游戏启动时调用）
    /// </summary>
    public void LoadOrCreateSave()
    {
        if (File.Exists(savePath))
            LoadSave();
        else
            CreateNewSave();
    }
    /// <summary>
    /// 保存当前数据
    /// </summary>
    public void SaveGame()
    {
        if (currentSave == null) return;
            
        currentSave.lastSaveTime = DateTime.Now;
        try
        {
            string json = JsonConvert.SerializeObject(currentSave, Formatting.Indented);
            //string json = JsonUtility.ToJson(currentSave, true);
            if (useEncryption) json = Encrypt(json);
            File.WriteAllText(savePath, json, Encoding.UTF8);
            OnSaveSaved?.Invoke(currentSave);
            Debug.Log("游戏已保存");

        }
        catch(Exception e)
        {
            Debug.LogError($"保存失败: {e.Message}");
        }
    }
    private void LoadSave()
    {
        try
        {
            string json = File.ReadAllText(savePath, Encoding.UTF8);
            if (useEncryption) json = Decrypt(json);
            currentSave = JsonConvert.DeserializeObject<GameSave>(json);
            //currentSave = JsonUtility.FromJson<GameSave>(json);
            //版本兼容处理
            if (currentSave.version != "0.1.0")
                MigrateSave(currentSave);
        
            Debug.Log($"加载存档成功: {currentSave.lastSaveTime}");
            OnSaveLoaded?.Invoke(currentSave);
        }
        catch(Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
            CreateNewSave();
        }
    }
    private void CreateNewSave()
    {
        currentSave = new GameSave
        {
            version = "0.1.0",
            lastSaveTime = DateTime.Now,
            gold = 0,
            characters = new List<CharacterSaveData>(),
            totalKill = 0,
            totalGamesPlayed = 0,
            totalPlayTimeSeconds = 0
        };
        SaveGame();
        Debug.Log("创建新存档");
    }

    private void MigrateSave(GameSave save)
    {
        //可以在这里给旧版本迁移时加默认值
        save.version = "0.1.0";
        SaveGame();
    }

    private string Encrypt(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] key = Encoding.UTF8.GetBytes(encryptionKey);
        for(int i = 0;i<bytes.Length; i++)
        {
            bytes[i] ^= key[i % key.Length];
        }
        return Convert.ToBase64String(bytes);
    }

    private string Decrypt(string input)
    {
        byte[] bytes = Convert.FromBase64String(input);
        byte[] key = Encoding.UTF8.GetBytes(encryptionKey);
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] ^= key[i % key.Length];
        return Encoding.UTF8.GetString(bytes);
    }

    // =========== 外部接口==========
    public GameSave GetCurrentSave() => currentSave;

    /// <summary>
    /// 修改数据并自动保存
    /// </summary>
    /// <param name="action"></param>
    public void Modify(Action<GameSave> action)
    {
        action?.Invoke(currentSave);
        Debug.Log("数据已修改，正在保存...");
        SaveGame();
    }

    /// <summary>
    /// 重置所有文档
    /// </summary>
    public void ResetAllData()
    {
        if(File.Exists(savePath)) File.Delete(savePath);
        
        CreateNewSave();
    }

    public bool IsSaveLoaded => currentSave != null;

    /// <summary>
    /// 保存结算内容
    /// </summary>
    public void AddRewardOnGameEnd(int earnedGold, int totalKill)
    {
        Modify(save =>
        {
            save.gold += earnedGold;
            save.totalKill += totalKill;
            save.totalGamesPlayed++;
        });
    }
}
