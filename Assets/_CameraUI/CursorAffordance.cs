using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
    [RequireComponent(typeof(CameraRaycaster))]
    public class CursorAffordance : MonoBehaviour
    {

        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D targetCursor = null;
        [SerializeField] Texture2D errorCursor = null;
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);
        [SerializeField] const int walkableLayerNumber = 9;
        [SerializeField] const int enemyLayerNumber = 10;

        CameraRaycaster cameraRaycaster;

        // Use this for initialization
        void Start()
        {
            cameraRaycaster = GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyLayerChangeObservers += OnLayerChanged; // registering
        }

        void OnLayerChanged(int newLayer)
        {
            //print("Cursor over new layer" + newLayer.ToString());
            switch (newLayer)
            {
                case walkableLayerNumber:
                    Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                    break;

                case enemyLayerNumber:
                    Cursor.SetCursor(targetCursor, cursorHotspot, CursorMode.Auto);
                    break;


                default:
                    Cursor.SetCursor(errorCursor, cursorHotspot, CursorMode.Auto);
                    return;

            }
        }
    }
}