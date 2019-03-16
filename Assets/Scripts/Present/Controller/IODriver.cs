using System.Collections;
using System.Collections.Generic;

public class IODriver {
    public static string FileDirRoot
    {
        get
        {
            return UnityEngine.Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar;
        }
    }

    public IODriver() {

    }

    public async System.Threading.Tasks.Task SaveToFile(string filename, byte[] content) {
        System.IO.FileStream file = System.IO.File.Create(UnityEngine.Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + filename, content.Length, System.IO.FileOptions.Asynchronous);
        await file.WriteAsync(content, 0, content.Length);
        file.Close();
    }

    public async System.Threading.Tasks.Task<byte[]> LoadFromFile(string filename) {
        System.IO.FileStream file;
        try {
            file = System.IO.File.OpenRead(UnityEngine.Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + filename);
        } catch (System.SystemException) {
            return null;
        }
        int len = System.Convert.ToInt32(file.Length);
        var ret = new byte[len];
        var num = await file?.ReadAsync(ret, 0, len);
        return ret;
    }

    public UnityEngine.Networking.UnityWebRequest HttpGet(string url) {
        var request = UnityEngine.Networking.UnityWebRequest.Get(url);
        return request;
    }

    public UnityEngine.Networking.UnityWebRequest HttpPost(string url, string fieldData)
    {
        var request = UnityEngine.Networking.UnityWebRequest.Post(url, fieldData);
        return request;
    }

    public UnityEngine.Networking.UnityWebRequest HttpPost(string url, Dictionary<string, string> fields)
    {
        var request = UnityEngine.Networking.UnityWebRequest.Post(url, fields);
        return request;
    }

    public UnityEngine.Networking.UnityWebRequest HttpPost(string url, Dictionary<string, string> fields, Dictionary<string, byte[]> binaries)
    {
        var form = new UnityEngine.WWWForm();
        if (fields != null)
        {
            foreach (var post_arg in fields)
            {
                form.AddField(post_arg.Key, post_arg.Value);
            }
        }
        if (binaries != null)
        {
            foreach (var post_arg in binaries)
            {
                form.AddBinaryData(post_arg.Key, post_arg.Value);
            }
        }
        var request = UnityEngine.Networking.UnityWebRequest.Post(url, fields);
        return request;
    }

    public UnityEngine.Networking.UnityWebRequest HttpPost(string url, Dictionary<string, string> fields, Dictionary<string, (byte[] contents, string filename, string mimetype)> binaries)
    {
        var form = new UnityEngine.WWWForm();
        if (fields != null)
        {
            foreach (var post_arg in fields)
            {
                form.AddField(post_arg.Key, post_arg.Value);
            }
        }
        if (binaries != null)
        {
            foreach (var post_arg in binaries)
            {
                form.AddBinaryData(post_arg.Key, post_arg.Value.contents, post_arg.Value.filename, post_arg.Value.mimetype);
            }
        }
        var request = UnityEngine.Networking.UnityWebRequest.Post(url, fields);
        return request;
    }

}
