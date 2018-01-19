using UnityEngine;
using NumeralSystem.Utils;
using System.Collections.Generic;

namespace NumeralSystem
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        private Dictionary<string, Object> _resPool = new Dictionary<string, Object>();

        public Object LoadResource(string path, System.Type type)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            Object res = null;
            if (_resPool.TryGetValue(path, out res))
                return res;
            res = Resources.Load(path, type);
            _resPool.Add(path, res);
            return res;
        }
    }
}