using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MgrBase
{
    
    private static DataManager _instance;
    private ReadOnlyData _readOnly;
    private WirteOnlyData _writeOnly;
    private Dictionary<string, DataBase> _dict;
    public static DataManager getInstance() {
        if (_instance == null) {
            _instance = new DataManager();
        }
        return _instance;
    }
    override
    public void start() {
        base.start();
        _dict = new Dictionary<string, DataBase>();
        _readOnly = new ReadOnlyData();
        _readOnly.delegateGetDataMethod(getDataByName);
        _writeOnly = new WirteOnlyData();
        _writeOnly.delegateGetDataMethod(getDataByName);
        _dict["GameData"] = new GameData();
        _dict["ItemData"] = new ItemData();
        _dict["PlayerData"] = new PlayerData();
    }
    private DataBase getDataByName(string name) {
        if (_dict.ContainsKey(name)) {
            return _dict[name];
        }
        return null;
    }

    public ReadOnlyData getReadOnly() {
        return _readOnly;
    }

    public WirteOnlyData getWirteOnly() {
        return _writeOnly;
    }

}
