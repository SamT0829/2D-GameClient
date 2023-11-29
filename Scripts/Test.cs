using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;

public class Test : MonoBehaviour
{
    public int testNumber;
    public string testString;
    private System.Diagnostics.Stopwatch _stopwatch;
    [SerializeField] GameObject Tests;

    private void Awake()
    {
        _stopwatch = new System.Diagnostics.Stopwatch();
        // _stopwatch.Start();

        Dictionary<string, string> test = new Dictionary<string, string>();
        test.Add("TestTable", "TestTable.txt");
        test.Add(typeof(MultiPlayerGameStartTable).Name, "MultiPlayerGameStartTable.txt");
        var path = Application.dataPath + "/Text";
        var testPath = "C:/Users/USER/Desktop/Unity/SamClient/Assets/Text/Table/TestTable.txt";
        if (File.Exists(testPath))
            Debug.Log(path);
        TableManager.Instance.CreateFirmTablesFromDictionary(test, path);
    }

    private void Start()
    {
        StartCoroutine(PlayerDie());
        // Task.Run(PlayerDieTask);
    }

    private async Task test()
    {
        Debug.Log("start");
        await Task.Delay(TimeSpan.FromSeconds(10));
        Debug.Log("finish");
    }


    private async Task PlayerDieTask()
    {
        Debug.Log("PlayerDie");
        Tests.SetActive(false);
        await Task.Delay(TimeSpan.FromSeconds(3));
        Tests.SetActive(true);
        Debug.Log("PlayerDie finsih");
    }

    private IEnumerator PlayerDie()
    {
        Debug.Log("PlayerDie");
        Tests.SetActive(false);
        yield return new WaitForSeconds(3);
        Tests.SetActive(true);
        Debug.Log("PlayerDie finsih");
    }

    byte[] CreateSerializedObject()
    {
        int dataSize = 0;
        byte[] firmBytes = Encoding.Unicode.GetBytes(testString);

        dataSize = sizeof(int) + sizeof(int) + firmBytes.Length;

        byte[] data = new byte[dataSize];
        int currentWritePosition = 0;


        byte[] intBytes = BitConverter.GetBytes(testNumber);
        Array.Copy(intBytes, 0, data, currentWritePosition, sizeof(int));
        currentWritePosition += sizeof(int);

        intBytes = BitConverter.GetBytes(firmBytes.Length);
        Array.Copy(intBytes, 0, data, currentWritePosition, sizeof(int));
        currentWritePosition += sizeof(int);

        Array.Copy(firmBytes, 0, data, currentWritePosition, firmBytes.Length);
        currentWritePosition += firmBytes.Length;

        return data;
    }

    public void SyncFromInfo(byte[] data)
    {
        int currentReadPosition = 0;

        testNumber = BitConverter.ToInt32(data, currentReadPosition);
        currentReadPosition += sizeof(int);

        int firmStringSize = BitConverter.ToInt32(data, currentReadPosition);
        currentReadPosition += sizeof(int);

        testString = Encoding.Unicode.GetString(data, currentReadPosition, firmStringSize);
        currentReadPosition += firmStringSize;
    }
}
