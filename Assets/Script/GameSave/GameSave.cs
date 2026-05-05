using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存档数据根对象 - 新增字段在此添加，旧版本需要做兼容处理
/// </summary>
public class GameSave 
{
    public string version = "0.1.0";  // 存档版本号，便于未来兼容处理
    public DateTime lastSaveTime;  // 存档时间

    // ============ 资源 ============
    public int gold = 0;

    // ============ 所有角色信息 ============
    public List<CharacterSaveData> characters = new List<CharacterSaveData>();

    // ============ 其他全局数据（如关卡进度、已解锁内容等） ============
    public int totalKill = 0;
    public int totalGamesPlayed = 0;    
    public int totalPlayTimeSeconds = 0;

    public bool IsValid() => !string.IsNullOrEmpty(version);
}
public class CharacterSaveData
{
    public string characterName;  // 角色名称
    public int level;            // 角色等级
    public float health;         // 角色当前生命值
    public float maxHealth;      // 角色最大生命值
    public float attack;         // 角色攻击力
    public float defense;        // 角色防御力
    public List<string> equippedWeapons = new List<string>(); // 已装备武器列表（存储武器ID或名称）
}