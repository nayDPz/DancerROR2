using RoR2;
using UnityEngine;

namespace Ridley.Modules
{
    internal static class CameraParams // from PaladinMod
    {
        internal static CharacterCameraParams defaultCameraParams;

        internal static void InitializeParams()
        {
            defaultCameraParams = NewCameraParams("ccpRidley", 70f, 1.37f, new Vector3(0, 0.0f, -12.5f));
        }

        private static CharacterCameraParams NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 standardPosition)
        {
            return NewCameraParams(name, pitch, pivotVerticalOffset, standardPosition, 0.1f);
        }

        private static CharacterCameraParams NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 standardPosition, float wallCushion)
        {
            CharacterCameraParams newParams = ScriptableObject.CreateInstance<CharacterCameraParams>();

            newParams.name = name;
            newParams.maxPitch = pitch;
            newParams.minPitch = -pitch;
            newParams.pivotVerticalOffset = pivotVerticalOffset;
            newParams.standardLocalCameraPos = standardPosition;
            newParams.wallCushion = wallCushion;

            return newParams;
        }
    }
}