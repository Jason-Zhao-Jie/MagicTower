using System.Collections;
using System.Collections.Generic;

namespace ArmyAnt.Manager {

    public static class IOManager {
        public static string FileDirRoot {
            get {
                return UnityEngine.Application.persistentDataPath + System.IO.Path.AltDirectorySeparatorChar;
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

        public static string SaveToFile(byte[] content, params string[] path) {
            return SaveToFileWholePath(content, ParsePath(path));
        }

        public static string SaveToFileWholePath(byte[] content, string path) {
            System.IO.FileStream file = System.IO.File.Create(path, content.Length, System.IO.FileOptions.Asynchronous);
            file.Close();
            System.IO.File.WriteAllBytes(path, content);
            return path;
        }

        public static byte[] LoadFromFile(params string[] path) {
            return LoadFromFileWholePath(ParsePath(path));
        }

        public static byte[] LoadFromFileWholePath(string path) {
            try {
                var ret = System.IO.File.ReadAllBytes(path);
                if(ret == null || ret.Length <= 0) {
                    return null;
                }
                return ret;
            } catch (System.SystemException) {
                return null;
            }
        }

        private static string ParsePath(string[] path) {
            string filename = UnityEngine.Application.persistentDataPath;
            for (var i = 0; i < path.Length; ++i) {
                filename += System.IO.Path.AltDirectorySeparatorChar + path[i];
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
