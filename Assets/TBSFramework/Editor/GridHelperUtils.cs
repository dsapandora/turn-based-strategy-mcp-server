using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TbsFramework.EditorUtils
{
    public class GridHelperUtils
    {
        public static void ClearScene(bool keepMainCamera)
        {
            var objects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            var toDestroy = new List<GameObject>();

            foreach (var obj in objects)
            {
                bool isChild = obj.transform.parent != null;

                if (isChild || keepMainCamera && obj.CompareTag("MainCamera"))
                    continue;

                toDestroy.Add(obj);
            }
            toDestroy.ForEach(o => GameObject.DestroyImmediate(o));
        }
    }
}