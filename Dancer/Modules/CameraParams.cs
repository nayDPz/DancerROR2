﻿using RoR2;
using UnityEngine;

namespace Dancer.Modules
{
    internal static class CameraParams // from PaladinMod
    {
        internal static CharacterCameraParams defaultCameraParams;

        internal static void InitializeParams()
        {
            defaultCameraParams = NewCameraParams("ccpDancer", 70f, 1.325f, new Vector3(0, 0.0f, -12f));
        }

        private static CharacterCameraParams NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 standardPosition)
        {
            return NewCameraParams(name, pitch, pivotVerticalOffset, standardPosition, 0.1f);
        }

        private static CharacterCameraParams NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 standardPosition, float wallCushion)
        {
            CharacterCameraParams newParams = ScriptableObject.CreateInstance<CharacterCameraParams>();

            newParams.name = name;
            newParams.data.maxPitch = pitch;
            newParams.data.minPitch = -pitch;
            newParams.data.pivotVerticalOffset = pivotVerticalOffset;
            newParams.data.idealLocalCameraPos = standardPosition;
            newParams.data.wallCushion = wallCushion;

            return newParams;
        }
    }
}