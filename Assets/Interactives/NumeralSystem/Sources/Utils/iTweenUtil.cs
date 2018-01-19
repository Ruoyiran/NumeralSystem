using System.Collections;
using UnityEngine;
namespace NumeralSystem.Utils
{
    public class TweenMove
    {
        public static void MoveTo(GameObject go, Vector3 position, float time)
        {
            Hashtable args = new Hashtable();
            args.Add("easetype", iTween.EaseType.easeInCubic);
            //args.Add("easetype", iTween.EaseType.easeInSine);
            args.Add("time", time);
            args.Add("position", position);
            args.Add("islocal", true);
            iTween.MoveTo(go, args);
        }
    }
}
