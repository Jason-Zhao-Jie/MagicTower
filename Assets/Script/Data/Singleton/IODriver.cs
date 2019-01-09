using System.Collections;
using System.Collections.Generic;

public class IODriver {
    public static IODriver instance = null;

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

    public IEnumerator HttpGet(string url) {
        var www = new UnityEngine.WWW(url);
        yield return www;
    }

    public IEnumerator HttpPost(string url, Dictionary<string, string> strings, Dictionary<string, byte[]> binaries) {
        var form = new UnityEngine.WWWForm();
        if (strings != null) {
            foreach (KeyValuePair<string, string> post_arg in strings) {
                form.AddField(post_arg.Key, post_arg.Value);
            }
        }
        if (binaries != null) {
            foreach (KeyValuePair<string, byte[]> post_arg in binaries) {
                form.AddBinaryData(post_arg.Key, post_arg.Value);
            }
        }

        var www = new UnityEngine.WWW(url, form);
        yield return www;
    }

}
