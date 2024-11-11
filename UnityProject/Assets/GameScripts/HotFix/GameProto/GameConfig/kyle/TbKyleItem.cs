
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;


namespace GameConfig.kyle
{
public partial class TbKyleItem
{
    private readonly System.Collections.Generic.Dictionary<int, kyle.TestItem> _dataMap;
    private readonly System.Collections.Generic.List<kyle.TestItem> _dataList;
    
    public TbKyleItem(ByteBuf _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, kyle.TestItem>();
        _dataList = new System.Collections.Generic.List<kyle.TestItem>();
        
        for(int n = _buf.ReadSize() ; n > 0 ; --n)
        {
            kyle.TestItem _v;
            _v = kyle.TestItem.DeserializeTestItem(_buf);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
    }

    public System.Collections.Generic.Dictionary<int, kyle.TestItem> DataMap => _dataMap;
    public System.Collections.Generic.List<kyle.TestItem> DataList => _dataList;

    public kyle.TestItem GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public kyle.TestItem Get(int key) => _dataMap[key];
    public kyle.TestItem this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}

