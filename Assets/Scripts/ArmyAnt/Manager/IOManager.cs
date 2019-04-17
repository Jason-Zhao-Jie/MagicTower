using System.Collections;
using System.Collections.Generic;

namespace ArmyAnt.Manager {

    public static class IOManager {
        public static string FileDirRoot {
            get {
                return UnityEngine.Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar;
            }
        }

        public static bool ExistFile(params string[] path) {
            return System.IO.File.Exists(ParsePath(path));
        }

        public static bool ExistDirectory(params string[] path) {
            return System.IO.Directory.Exists(ParsePath(path));
        }

        public static bool MkdirIfNotExist(params string[] path) {
            var dir = ParsePath(path);
            try {
                if (System.IO.Directory.Exists(dir)) {
                    return false;
                }
                System.IO.Directory.CreateDirectory(dir);
            } catch (System.IO.IOException) {
                return false;
            }
            return true;
        }

        public static bool RemoveFolder(params string[] path) {
            try {
                System.IO.Directory.Delete(ParsePath(path), true);
            } catch (System.IO.IOException) {
                return false;
            }
            return true;
        }

        public static string[] ListAllFiles(params string[] path) {
            return System.IO.Directory.GetFiles(ParsePath(path));
        }

        public static string[] ListAllFiles(string partten, params string[] path) {
            return System.IO.Directory.GetFiles(ParsePath(path), partten);
        }

        public static string[] ListAllDirectories(params string[] path) {
            return System.IO.Directory.GetDirectories(ParsePath(path));
        }

        public static async System.Threading.Tasks.Task<string> SaveToFile(byte[] content, params string[] path) {
            return await SaveToFileWholePath(content, ParsePath(path));
        }

        public static async System.Threading.Tasks.Task<string> SaveToFileWholePath(byte[] content, string path) {
            System.IO.FileStream file = System.IO.File.Create(path, content.Length, System.IO.FileOptions.Asynchronous);
            await file.WriteAsync(content, 0, content.Length);
            file.Close();
            return path;
        }

        public static async System.Threading.Tasks.Task<byte[]> LoadFromFile(params string[] path) {
            return await LoadFromFileWholePath(ParsePath(path));
        }

        public static async System.Threading.Tasks.Task<byte[]> LoadFromFileWholePath(string path) {
            System.IO.FileStream file;
            try {
                file = System.IO.File.OpenRead(path);
            } catch (System.SystemException) {
                return null;
            }
            int len = System.Convert.ToInt32(file.Length);
            var ret = new byte[len];
            var num = await file?.ReadAsync(ret, 0, len);
            return ret?.Length > 0 ? ret : null;
        }

        private static string ParsePath(string[] path) {
            string filename = UnityEngine.Application.persistentDataPath;
            for (var i = 0; i < path.Length; ++i) {
                filename += System.IO.Path.DirectorySeparatorChar + path[i];
            }
            return filename;
        }

        public static UnityEngine.Networking.UnityWebRequest HttpGet(string url) {
            var request = UnityEngine.Networking.UnityWebRequest.Get(url);
            return request;
        }

        public static UnityEngine.Networking.UnityWebRequest HttpPost(string url, string fieldData) {
            var request = UnityEngine.Networking.UnityWebRequest.Post(url, fieldData);
            return request;
        }

        public static UnityEngine.Networking.UnityWebRequest HttpPost(string url, Dictionary<string, string> fields) {
            var request = UnityEngine.Networking.UnityWebRequest.Post(url, fields);
            return request;
        }

        public static UnityEngine.Networking.UnityWebRequest HttpPost(string url, Dictionary<string, string> fields, Dictionary<string, byte[]> binaries) {
            var form = new UnityEngine.WWWForm();
            if (fields != null) {
                foreach (var post_arg in fields) {
                    form.AddField(post_arg.Key, post_arg.Value);
                }
            }
            if (binaries != null) {
                foreach (var post_arg in binaries) {
                    form.AddBinaryData(post_arg.Key, post_arg.Value);
                }
            }
            var request = UnityEngine.Networking.UnityWebRequest.Post(url, fields);
            return request;
        }

        public static UnityEngine.Networking.UnityWebRequest HttpPost(string url, Dictionary<string, string> fields, Dictionary<string, (byte[] contents, string filename, string mimetype)> binaries) {
            var form = new UnityEngine.WWWForm();
            if (fields != null) {
                foreach (var post_arg in fields) {
                    form.AddField(post_arg.Key, post_arg.Value);
                }
            }
            if (binaries != null) {
                foreach (var post_arg in binaries) {
                    form.AddBinaryData(post_arg.Key, post_arg.Value.contents, post_arg.Value.filename, post_arg.Value.mimetype);
                }
            }
            var request = UnityEngine.Networking.UnityWebRequest.Post(url, fields);
            return request;
        }

    }

}
