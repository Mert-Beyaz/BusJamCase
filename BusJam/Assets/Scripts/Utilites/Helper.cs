using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public static class Helper
    {
        /*/////////////////////////////////////////////////////////////////////////////////////////
		//////////// Main Camera Finder
		//////////////////////////////////////////////////////////////////////////////////////////*/
        private static Camera _camera;
        public static Camera MainCamera
        {
            get
            {
                if (_camera == null) _camera = Camera.main;
                return _camera;
            }
        }

        /*/////////////////////////////////////////////////////////////////////////////////////////
		//////////// None-Allocating WaitForSeconds 
		//////////////////////////////////////////////////////////////////////////////////////////*/
        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();

        public static WaitForSeconds GetWait(float time)
        {
            if (WaitDictionary.TryGetValue(time, out var wait)) return wait;

            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }

        /*/////////////////////////////////////////////////////////////////////////////////////////
		//////////// Return Canvas element position to World position
		//////////////////////////////////////////////////////////////////////////////////////////*/
        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, MainCamera, out var result);
            return result;
        }

        /*/////////////////////////////////////////////////////////////////////////////////////////
		//////////// Quick destroy all child objects
		//////////////////////////////////////////////////////////////////////////////////////////*/
        public static void DeleteChildren(this Transform t)
        {
            foreach (Transform child in t) Object.Destroy(child.gameObject);
        }
    }

}
