using GameBase;
using GameConfig.item;

/// <summary>
/// 道具配置表管理器。
/// </summary>
public class ItemConfigMgr: Singleton<ItemConfigMgr>
{
    /// <summary>
    /// 道具Table。
    /// </summary>
    private TbItem TbItem => ConfigSystem.Instance.Tables.TbItem;

    /// <summary>
    /// 获取道具配置表。
    /// </summary>
    /// <param name="itemId">道具Id。</param>
    /// <returns>道具配置表。</returns>
    public Item GetItemConfig(int itemId)
    {
        TbItem.DataMap.TryGetValue(itemId, out var config);
        return config;
    }
}