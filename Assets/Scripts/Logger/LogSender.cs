using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LogSender
{
    public static async void SendLog<T>(string gasURL,
                                        string sheetURL,
                                        T data,
                                        string sheetName = null)
    {
        if (data == null)
        {
            Debug.LogError($"Can't send log. Because dataClass is null");
            return;
        }

        if (gasURL == null || sheetURL == null)
        {
            Debug.LogError("Can't send log. Because not set gasURL or sheetURL");
            return;
        }

        if (sheetName == null) sheetName = "null";

        var dataType = typeof(T);
        var dataFields = dataType.GetFields();

        WWWForm form = new WWWForm();
        form.AddField("sheetURL", sheetURL);
        form.AddField("sheetName", sheetName);

        if (dataFields.Length == 0)
        {
            // フィールドがない → int や string など
            // form.AddField("value", dataClass.ToString());
            // Debug.Log($"[Primitive] value: {dataClass.ToString()}");
            Debug.Log($"{dataType.Name} is invalid data. Please pass class or struct as an parameter ");
            return;
        }
        else
        {
            var keys = dataFields.Select(f => f.Name).ToArray();
            form.AddField("keys", string.Join(',', keys));

            foreach (var field in dataFields)
            {
                var fieldValue = field.GetValue(data);
                form.AddField(field.Name, fieldValue?.ToString() ?? "null");
                Debug.Log($"[{field.Name}] {fieldValue}");
            }
        }

        using UnityWebRequest www = UnityWebRequest.Post(gasURL, form);
        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.LogError($"Failed to send log of {data}: " + www.error);
        else
            Debug.Log($"Success to send log of {data}: " + www.downloadHandler.text);
    }
}
