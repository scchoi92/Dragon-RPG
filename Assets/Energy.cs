using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] RawImage energyBar;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float pointsPerHit = 10f;

        float currentEnergyPoints;

        CameraRaycaster cameraRaycaster;

        // Start is called before the first frame update
        void Start()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            SetCurrentMaxEnergy();
            cameraRaycaster.notifyRightClickObservers += ProcessRightClick;
        }

        private void SetCurrentMaxEnergy() { currentEnergyPoints = maxEnergyPoints; }

        void ProcessRightClick(RaycastHit raycastHit, int layerHit)
        {
            float newEnergyPoints = currentEnergyPoints - pointsPerHit;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
            UpdateEnergyBarUI();
        }

        private void UpdateEnergyBarUI()
        {
            float xValue = -(EnergyAsPercent() / 2f) - 0.5f;
            energyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        }

        float EnergyAsPercent()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }
    }
}