﻿using System.Collections;
using System.Collections.Generic;

public class IOManager {
    public static string FileDirRoot
    {
        get
        {
            return UnityEngine.Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar;
        }
    }

    public IOManager() {

    }

    public bool MkdirIfNotExist(params string[] path)
    {
        string dir = UnityEngine.Application.persistentDataPath;
        for (var i = 0; i < path.Length; ++i)
        {
            dir += System.IO.Path.DirectorySeparatorChar + path[i];
        }
        try
        {
            System.IO.Directory.CreateDirectory(dir);
        }
        catch (System.IO.IOException)
        {
            return false;
        }
        return true;
    }

    public string[] ListAllFiles(params string[] path)
    {
        string dir = UnityEngine.Application.persistentDataPath;
        for (var i = 0; i < path.Length; ++i)
        {
            dir += System.IO.Path.DirectorySeparatorChar + path[i];
        }
        return System.IO.Directory.GetFiles(dir);
    }

    public string[] ListAllFiles(string partten, params string[] path)
    {
        string dir = UnityEngine.Application.persistentDataPath;
        for (var i = 0; i < path.Length; ++i)
        {
            dir += System.IO.Path.DirectorySeparatorChar + path[i];
        }
        return System.IO.Directory.GetFiles(dir, partten);
    }

    public async System.Threading.Tasks.Task<string> SaveToFile(byte[] content, params string[] path) {
        string filename = UnityEngine.Application.persistentDataPath;
        for(var i = 0; i < path.Length; ++i)
        {
            filename += System.IO.Path.DirectorySeparatorChar + path[i];
        }
        return await SaveToFileWholePath(content, filename);
    }

    public async System.Threading.Tasks.Task<string> SaveToFileWholePath(byte[] content, string path)
    {
        System.IO.FileStream file = System.IO.File.Create(path, content.Length, System.IO.FileOptions.Asynchronous);
        await file.WriteAsync(content, 0, content.Length);
        file.Close();
        return path;
    }

    public async System.Threading.Tasks.Task<byte[]> LoadFromFile(params string[] path)
    {
        string filename = UnityEngine.Application.persistentDataPath;
        for (var i = 0; i < path.Length; ++i)
        {
            filename += System.IO.Path.DirectorySeparatorChar + path[i];
        }
        return await LoadFromFileWholePath(filename);
    }

    public async System.Threading.Tasks.Task<byte[]> LoadFromFileWholePath(string path)
    {
        System.IO.FileStream file;
        try
        {
            file = System.IO.File.OpenRead(path);
        }
        catch (System.SystemException)
        {
            return null;
        }
        int len = System.Convert.ToInt32(file.Length);
        var ret = new byte[len];
        var num = await file?.ReadAsync(ret, 0, len);
        return ret?.Length > 0 ? ret : null;
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
