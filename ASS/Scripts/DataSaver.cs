using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DataSaver
{
    //Save Data
    public static void SaveData<T>(T dataToSave, string path)
    {
        // string tempPath = Path.Combine(Application.persistentDataPath, "data");
        // tempPath = Path.Combine(tempPath, dataFileName + ".txt");
        //string tempPath = GetFullPath(localPath, fileName);
        var fullPath = Application.dataPath + "/Resources/" + path + ".json";

        //Convert To Json then to bytes
        string jsonData = JsonUtility.ToJson(dataToSave, true);
        byte[] jsonByte = Encoding.UTF8.GetBytes(jsonData);

        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
        {
            Debug.Log("Create Directory : " + fullPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        }

        //try save
        try
        {
            File.WriteAllBytes(fullPath, jsonByte);
            Debug.Log("Saved Data to: " + fullPath.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To PlayerInfo Data to: " + fullPath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    //Load Data
    public static T LoadData<T>(string path)
    {

        string jsonData = Resources.Load<TextAsset>(path).text;
        
        
        //Convert to Object
        object resultValue = JsonUtility.FromJson<T>(jsonData);
        return (T)Convert.ChangeType(resultValue, typeof(T));
    }

    public static bool DeleteData(string localPath, string fileName)
    {
        bool success = false;

        //Load Data
        // string tempPath = Path.Combine(Application.persistentDataPath, "data");
        // tempPath = Path.Combine(tempPath, dataFileName + ".txt");
        string tempPath = GetFullPath(localPath, fileName);

        //Exit if Directory or File does not exist
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Debug.LogWarning("Directory does not exist");
            return false;
        }

        if (!File.Exists(tempPath))
        {
            Debug.Log("File does not exist");
            return false;
        }

        try
        {
            File.Delete(tempPath);
            Debug.Log("Data deleted from: " + tempPath.Replace("/", "\\"));
            success = true;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Delete Data: " + e.Message);
        }

        return success;
    }


    private static string GetFullPath(string localPath, string fileName)
    {
        string tempPath = Path.Combine(Application.persistentDataPath, localPath);
        tempPath = Path.Combine(tempPath, fileName);
        return tempPath;
    }

    private static string IntArrToStr(int[] _ints)
    {
        string str = "";
        for (int i = 0; i < _ints.Length; i++)
        {
            str += _ints[i];
            if (i != _ints.Length - 1) str += ",";
        }

        return str;
    }

    private static int[] StrToIntArr(string _str)
    {
        string[] strArr = _str.Split(',');
        int[] intArr = new int[strArr.Length];
        for (int i = 0; i < intArr.Length; i++)
        {
            intArr[i] = System.Convert.ToInt32(strArr[i]);
        }

        return intArr;
    }

    private static string FloatArrToStr(float[] _floats)
    {
        string str = "";
        for (int i = 0; i < _floats.Length; i++)
        {
            str += _floats[i];
            if (i != _floats.Length - 1) str += ",";
        }

        return str;
    }

    private static float[] StrToFloatArr(string _str)
    {
        string[] strArr = _str.Split(',');
        float[] floatArr = new float[strArr.Length];
        for (int i = 0; i < floatArr.Length; i++)
        {
            floatArr[i] = System.Convert.ToSingle(strArr[i]);
        }

        return floatArr;
    }

    public static void SaveIntArrToPlayerPrefs(string _name, int[] _ints)
    {
        string str = IntArrToStr(_ints);
        PlayerPrefs.SetString(_name, str);
        PlayerPrefs.Save();
    }

    public static int[] LoadIntArrFromPlayerPrefs(string _name)
    {
        string str = PlayerPrefs.GetString(_name);
        if (str == "") return new int[0];
        return StrToIntArr(str);
    }

    public static void SaveFloatArrToPlayerPrefs(string _name, float[] _floats)
    {
        string str = FloatArrToStr(_floats);
        PlayerPrefs.SetString(_name, str);
        PlayerPrefs.Save();
    }

    public static float[] LoadFloatArrFromPlayerPrefs(string _name)
    {
        string str = PlayerPrefs.GetString(_name);
        if (str == "") return new float[0];
        return StrToFloatArr(str);
    }

    public static int[] GetWholeStageClearInfo()
    {
        int[] data = LoadIntArrFromPlayerPrefs("StageClearInfo");
        if (data.Length == 0)
        {
            SaveIntArrToPlayerPrefs("StageClearInfo", new int[10]);
            data = LoadIntArrFromPlayerPrefs("StageClearInfo");
        }
        else if (data.Length < 10)
        {
            int[] newData = new int[10];
            for (int i = 0; i < data.Length; i++)
            {
                newData[i] = data[i];
            }
            SaveIntArrToPlayerPrefs("StageClearInfo", newData);
            data = LoadIntArrFromPlayerPrefs("StageClearInfo");
        }

        return data;
    }

    public static bool GetStageClearInfo(int _stage)
    {
        int[] data = GetWholeStageClearInfo();
        int arrIdx = _stage / 32;
        int bitIdx = _stage % 32;
        int clear = (data[arrIdx] & (1 << bitIdx)) >> bitIdx;
        if (clear == 1) return true;
        else return false;
    }

    public static void SaveStageClearInfo(int _stage, bool _clear)
    {
        int[] data = GetWholeStageClearInfo();
        int arrIdx = _stage / 32;
        int bitIdx = _stage % 32;
        data[arrIdx] = (data[arrIdx] & ~(1 << bitIdx)) | ((_clear ? 1 : 0) << bitIdx);
        SaveIntArrToPlayerPrefs("StageClearInfo", data);
    }

    public static int[] GetWholeStageHeartInfo()
    {
        int[] data = LoadIntArrFromPlayerPrefs("StageHeartInfo");
        if (data.Length == 0)
        {
            SaveIntArrToPlayerPrefs("StageHeartInfo", new int[20]);
            data = LoadIntArrFromPlayerPrefs("StageHeartInfo");
        }
        else if (data.Length < 20)
        {
            int[] newData = new int[20];
            for (int i = 0; i < data.Length; i++)
            {
                newData[i] = data[i];
            }

            SaveIntArrToPlayerPrefs("StageHeartInfo", newData);
            data = LoadIntArrFromPlayerPrefs("StageHeartInfo");
        }

        return data;
    }

    public static int GetStageHeartInfo(int _stage)
    {
        int[] data = GetWholeStageHeartInfo();
        int arrIdx = _stage * 2 / 32;
        int bitIdx = _stage * 2 % 32;
        int heart = (data[arrIdx] & (3 << bitIdx)) >> bitIdx;
        return heart;
    }

    public static void SaveStageHeartInfo(int _stage, int _count)
    {
        if(_count > 3) Debug.LogError("[DataSaver] You have saved more than 3 hearts. Stage : " + _stage);
        int[] data = GetWholeStageHeartInfo();
        int arrIdx = _stage * 2 / 32;
        int bitIdx = _stage * 2 % 32;
        int target = data[arrIdx];
        data[arrIdx] = (data[arrIdx] & ~(3 << bitIdx)) | (_count << bitIdx);
        SaveIntArrToPlayerPrefs("StageHeartInfo", data);
    }

    public static float[] GetWholeStageTimeInfo()
    {
        float[] data = LoadFloatArrFromPlayerPrefs("StageTimeInfo");
        if (data.Length == 0)
        {
            float[] temp = new float[320];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = 9999f;
            }
            SaveFloatArrToPlayerPrefs("StageTimeInfo", temp);
            data = LoadFloatArrFromPlayerPrefs("StageTimeInfo");
        }
        else if (data.Length < 320)
        {
            float[] newData = new float[320];
            for (int i = 0; i < data.Length; i++)
            {
                newData[i] = data[i];
            }

            for (int i = data.Length; i < newData.Length; i++)
            {
                newData[i] = 9999f;
            }

            SaveFloatArrToPlayerPrefs("StageTimeInfo", newData);
            data = LoadFloatArrFromPlayerPrefs("StageTimeInfo");
        }

        return data;
    }

    public static float GetStageTimeInfo(int _stage)
    {
        float[] data = GetWholeStageTimeInfo();
        return data[_stage];
    }

    public static void SaveStageTimeInfo(int _stage, float _time)
    {
        float[] data = GetWholeStageTimeInfo();
        data[_stage] = _time;
        SaveFloatArrToPlayerPrefs("StageTimeInfo", data);
    }
    

    public static void SaveStageData(int _stage, bool _clear = false, int _heartCount = 0, float _spendTime = 0f, bool _overwrite = false)
    {
        bool clear = _overwrite ? _clear : GetStageClearInfo(_stage) || _clear;
        int heartCount = _overwrite ? _heartCount : Mathf.Max(GetStageHeartInfo(_stage), _heartCount);
        float spendTime = _overwrite ? _spendTime : Mathf.Min(GetStageTimeInfo(_stage), _spendTime);
        SaveStageClearInfo(_stage, clear);
        SaveStageHeartInfo(_stage, heartCount);
        SaveStageTimeInfo(_stage, spendTime);
    }
}
