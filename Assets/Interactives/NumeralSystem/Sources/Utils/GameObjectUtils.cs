using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NumeralSystem.Utils
{
    public class GameObjectUtils
    {
        public static T GetComponent<T>(GameObject obj, bool addComponentIfNoExist = true) where T : Component
        {
            if (obj == null)
                return null;
            T script = obj.GetComponent<T>();
            if (script == null && addComponentIfNoExist)
                script = obj.AddComponent<T>();
            return script;
        }

        public static T GetComponent<T>(Transform transform, bool addComponentIfNoExist = true) where T : Component
        {
            if (transform == null)
                return null;
            return GetComponent<T>(transform.gameObject, addComponentIfNoExist);
        }

        public static T GetComponent<T>(Transform transform, string path) where T : Component
        {
            if (transform == null)
                return null;
            if (string.IsNullOrEmpty(path))
                return transform.GetComponent<T>();
            Transform childTransform = transform.Find(path);
            if (childTransform == null)
                return null;
            T script = childTransform.GetComponent<T>();
            return script;
        }

        public static GameObject FindGameObject(Transform transform, string path)
        {
            if (transform == null)
                return null;
            Transform findTransform = transform.Find(path);
            if (findTransform == null)
            {
                Debug.LogError(string.Format("GameObjectUtils.FindGameObject - Failed to find GameObject from path '{0}'", path));
                return null;
            }
            return findTransform.gameObject;
        }

        public static GameObject InstantiateGameObject(GameObject obj, Transform parent = null, string name = null)
        {
            if (obj == null)
                return null;
            GameObject newObj = Object.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
            if (newObj == null)
                return null;
            if (parent != null)
                newObj.transform.SetParent(parent);
            if (!string.IsNullOrEmpty(name))
                newObj.name = name;
            return newObj;
        }

        public static void TweenMoveTo(GameObject go, Vector3 position, float time, bool isLocal = false, iTween.EaseType easeType = iTween.EaseType.easeInCubic)
        {
            if (go == null)
                return;
            Hashtable args = new Hashtable();
            args.Add("easetype", easeType);
            args.Add("time", time);
            args.Add("position", position);
            args.Add("islocal", isLocal);
            iTween.MoveTo(go, args);
        }

        public static void SetActive(GameObject obj, bool active)
        {
            if (obj == null)
                return;
            obj.transform.localScale = active ? Vector3.one : Vector3.zero;
            //if (obj.activeSelf != active)
            //    obj.SetActive(active);
        }

        public static void SetImageColor(GameObject obj, Color color)
        {
            if (obj == null)
                return;
            Image image = GameObjectUtils.GetComponent<Image>(obj, false);
            if (image == null)
                return;
            image.color = color;
        }

        public static bool SetImageSprite(GameObject obj, string spritePath)
        {
            if (obj == null)
                return false;
            Sprite sprite = ResourceManager.Instance.LoadResource(spritePath, typeof(Sprite)) as Sprite;
            if(sprite != null)
            {
                Image image = GetComponent<Image>(obj, false);
                if(image != null)
                {
                    image.sprite = sprite;
                    return true;
                }
            }
            return false;
        }
    }
}