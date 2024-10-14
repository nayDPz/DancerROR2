using RoR2;
using UnityEngine;

namespace Dancer.Modules
{

    internal static class CameraParams
    {
        internal static CharacterCameraParams defaultCameraParams;

        internal static void InitializeParams()
        {
            defaultCameraParams = NewCameraParams("ccpDancer", 70f, 1.325f, new Vector3(0f, 0f, -12f));
        }

        private static CharacterCameraParams NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 standardPosition)
        {
            return NewCameraParams(name, pitch, pivotVerticalOffset, standardPosition, 0.1f);
        }

        private static CharacterCameraParams NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 standardPosition, float wallCushion)
        {
            CharacterCameraParams characterCameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            characterCameraParams.name = name;
            characterCameraParams.data.maxPitch = pitch;
            characterCameraParams.data.minPitch = 0f - pitch;
            characterCameraParams.data.pivotVerticalOffset = pivotVerticalOffset;
            characterCameraParams.data.idealLocalCameraPos = standardPosition;
            characterCameraParams.data.wallCushion = wallCushion;
            return characterCameraParams;
        }
    }
}
