using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GeodesicGrid;
using KerbalWeatherSimulator;
using KWSKSPButtToucher;

namespace Weather
{
    class KSPWeatherFunctions
    {

        public static Cell getCellAtLocation(CelestialBody body, Vector3 worldPos)
        {
            Cell cell = Cell.Containing(body.transform.InverseTransformPoint(worldPos), KWSSettings.gridLevel);
            return cell;
        }

        public static Vector3 getSunPosition()
        {
            return FlightGlobals.Bodies[0].position;
        }

        public static Vector3 getCoriolisAcc(CelestialBody body, Cell cell)
        {
            //ac = -2(angularVelocity) x velocity
            Vector3 acc = -2 * Vector3.Cross(Vector3.Cross(body.angularVelocity,
                new Vector3(0, Mathf.Cos(WeatherFunctions.getLatitude(cell)), Mathf.Sin(WeatherFunctions.getLatitude(cell)))),
                KSPHeadMaster.PlanetMap[body].LiveMap[0][cell].WindDirection);
            return acc;
        }

        public static bool isOcean(CelestialBody body, Cell cell)
        {

            //Debug.Log(body);
            if (body.ocean)
            {
                if (body.pqsController != null)
                {

                    Vector3 thing = thing = new Vector3(cell.Position.x, cell.Position.y, cell.Position.z);

                    if (Math.Round(body.pqsController.GetSurfaceHeight(thing) - body.pqsController.radius, 1) < 0)
                    {
                        return true;
                    }
                    else
                    {

                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }

            else
            {
                return false;
            }
        }


    }
}
