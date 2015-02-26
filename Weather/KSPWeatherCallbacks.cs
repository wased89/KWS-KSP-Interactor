using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GeodesicGrid;
using KerbalWeatherSystems;

namespace Weather
{
    class KSPWeatherCallbacks
    {
        private CelestialBody body;

        public KSPWeatherCallbacks(CelestialBody body)
        {
            this.body = body;
        }

        public Vector3 SunDirection()
        {
            return Vector3.zero;
        }

        public float SunlightAngle(Vector3 sunDir, int AltLayer, Cell cell)
        {
            return 0f;
        }

    }
}
