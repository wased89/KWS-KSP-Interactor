using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GeodesicGrid;


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
            
            return body.rotation * (FlightGlobals.Bodies[0].position - body.position).normalized;
        }

        public float SunlightAngle(Vector3 sunDir, int AltLayer, Cell cell)
        {
            return Vector3.Angle(cell.Position, sunDir);
        }

    }
}
