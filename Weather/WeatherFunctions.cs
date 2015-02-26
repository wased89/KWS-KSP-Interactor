using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using GeodesicGrid;
using KWSKSPButtToucher;


namespace Weather
{
    public class WeatherFunctions
    {

        private static float offSetMultiplier;

        public static float getCellLatitude(Cell cell)
        {
            return (float)(Math.Acos(cell.Position.y / Math.Sqrt(cell.Position.x * cell.Position.x + cell.Position.z * cell.Position.z)) * 180 / Math.PI);
        }

        public static float getCellLongitude(Cell cell)
        {
            return (float)(Math.Atan2(cell.Position.z, cell.Position.x) * 180 / Math.PI);
        }

        public static Vector3 getCellVector(Cell cellA, Cell cellB)
        {
            return (cellA.Position - cellB.Position);
        }

        public static Vector3 getSunPosition()
        {
            return FlightGlobals.Bodies[0].position;
        }

        public static float getSunlightAngle(int AltLayer, Cell cell)
        {
            float degrees;
            Vector3 sunPos = FlightGlobals.Bodies[0].transform.TransformDirection(cell.Position);
            
            degrees = Vector3.Angle(cell.Position, sunPos);

            return degrees;
        }
        internal static float ToTheFourth(float numb)
        {
            return numb * numb * numb * numb;
        }

        internal static float calculateEmissivity(CelestialBody body, int AltLayer, Cell cell)
        {

            float emis;
            if (HeadMaster.PlanetMap[body].LiveMap[AltLayer][cell].isOcean) //is it ocean?
            {
                emis = 0.96f;
            }
            else if (WeatherFunctions.getCellLatitude(cell) >= 60 && HeadMaster.PlanetMap[body].LiveMap[AltLayer][cell].isOcean != true) //Snowy colder regions
            {
                emis = 0.97f;
            }
            else
            {
                emis = 0.92f;
            }

            return emis;
        }



        internal static float calculateTransmissivity(CelestialBody body, int AltLayer, Cell cell)
        {
            //Beers-lambert law covers transmissivity
            //basically: transmitting rad = starting rad * e^(-m*optical depth)
            //rearranged: transmissivity = e^(-m * optical depth)
            //-m is the optical airmass
            //which is a scaling parameter based on the amount of air that the ray will travel through
            //optical depth is the "opacity" of the air and is dependant on composition
            //optical airmass is dependant on the thickness of the layers, and pressure

            float opticalDepth = calculateOpticalDepth(body, cell);
            float opticalAirMass = calculateAtmosphericPathLength(body, AltLayer, cell);
            float T = Mathf.Pow((float)Math.E, (-opticalAirMass * opticalDepth));
            return T * (1 - HeadMaster.PlanetMap[body].LiveMap[AltLayer][cell].Albedo);
        }

        internal static float calculateAtmosphericPathLength(CelestialBody body, int AltLayer, Cell cell)
        {

            float zenithAngle = getSunlightAngle(AltLayer, cell) * Mathf.Deg2Rad;
            float ymax = HeadMaster.PlanetMap[body].LiveMap[HeadMaster.PlanetMap[body].LiveMap.Count - 1][cell].Altitude + KWSKSPButtToucher.Settings.cellDefinitionAlt;
            float stuff = (float)Math.Sqrt(
                (((body.Radius + HeadMaster.PlanetMap[body].LiveMap[AltLayer][cell].Altitude) / ymax) * ((body.Radius + HeadMaster.PlanetMap[body].LiveMap[AltLayer][cell].Altitude) / ymax)) *
                (Mathf.Cos(zenithAngle) * Mathf.Cos(zenithAngle)) +
                ((2 * body.Radius) / (ymax * ymax)) *
                (ymax - HeadMaster.PlanetMap[body].LiveMap[AltLayer][cell].Altitude) -
                ((ymax / ymax) * (ymax / ymax)) + 1 -
                ((body.Radius + HeadMaster.PlanetMap[body].LiveMap[AltLayer][cell].Altitude) / ymax) *
                (Mathf.Cos(zenithAngle))
                );
            return stuff;
        }



        internal static float calculateOpticalDepth(CelestialBody body, Cell cell)
        {
            float opticalDepth = 0.002f; //Original: 0.2f
            return opticalDepth;
        }
        

        public static bool cellContainsOcean(CelestialBody body, int AltLayer, Cell cell)
        {

            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;

            if (Layers[0][cell].isOcean) { return true; }
            else { return false; }
        }

        public static Vector3 getCellThermalForce(CelestialBody body, int AltLayer, Cell cell)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;

            return Vector3.zero;
        }

        public static bool doesCellHaveThermal(CelestialBody body, int AltLayer, Cell cell)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;

            return false;
        }

        public static bool doesCellHaveTempScanner(CelestialBody body, int AltLayer, Cell cell)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;

            return false;
        }

        public static bool doesCellHavePressureScanner(CelestialBody body, int AltLayer, Cell cell)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;

            return false;
        }

        public static bool doesCellHaveWindScanner(CelestialBody body, int AltLayer, Cell cell)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;

            return false;
        }

        public static bool doesCellHaveRainScanner(CelestialBody body, int AltLayer, Cell cell)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;

            return false;
        }

        public static CellMap<WeatherCell> getCellMap(CelestialBody body, int AltLayer)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;

            return Layers[AltLayer];
        }

        public static float getCellAlbedo(CelestialBody body, int AltLayer, Cell cell)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;

            //double Albedo = cell.Albedo;
            float Albedo = Layers[AltLayer][cell].Albedo;
            return Albedo;
        }

        public static WeatherCell getWeatherCellFromCell(CelestialBody body, int AltLayer, Cell cell)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;
            WeatherCell weatherCell = Layers[AltLayer][cell];
            return weatherCell;
        }

        public static double getPressureDifference(CelestialBody body, int AltLayer, Cell cellA, Cell cellB)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;
            double PressureDifference = (Layers[AltLayer][cellA].Pressure * 101325) - (Layers[AltLayer][cellB].Pressure * 101325);
            return PressureDifference;
        }

        public static double getAbsolutePressureDifference(CelestialBody body, int AltLayer, Cell cellA, Cell cellB)
        {
            //Debug.Log("1: " + HeadMaster.PlanetMap[body][cellA].Pressure);
            //Debug.Log("2: " + HeadMaster.PlanetMap[body][cellB].Pressure);
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;
            double AbsolutePressureDifference = Math.Abs(((Layers[AltLayer][cellA].Pressure) - (Layers[AltLayer][cellB].Pressure)));
            return AbsolutePressureDifference;
        }

        public static Cell getNeighbourWithHighestPressureDifference(CelestialBody body, int AltLayer, Cell cell)
        {
            //Step 1: Collect neighbours
            //Step 2: Compare absolute pressure differences
            //Step 3: grab highest pressure gradient
            //Step 4: return CellID of neighbour with highest Pressure gradient.

            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;
            Cell HighestNeighbour = cell;
            double PressDiff;
            double HighestPressureDiff = -1;
            IEnumerable<Cell> Neighbours = cell.GetNeighbors(KWSKSPButtToucher.Settings.gridLevel);
            foreach (Cell CellNeighbour in Neighbours)
            {
                PressDiff = getAbsolutePressureDifference(body, AltLayer, cell, CellNeighbour);
                if (PressDiff > HighestPressureDiff)
                {
                    HighestPressureDiff = PressDiff;
                    HighestNeighbour = CellNeighbour;
                }

            }

            return HighestNeighbour;

        }

        public static Vector3 getCellPressureGradientVector(CelestialBody body, int AltLayer, Cell cell)
        {
            //CellAcceleration = -1/Cell.Density * (VectorFromCellAtoCEllB * PressureGradient)

            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;
            Cell Neighbour = getNeighbourWithHighestPressureDifference(body, AltLayer, cell);

            Vector3 CellVector = getCellVector(cell, Neighbour);
            double Acc = -1 / Layers[AltLayer][cell].Density * ((getPressureDifference(body, AltLayer, cell, Neighbour) / CellVector.magnitude));

            Vector3 CellAcc = CellVector.normalized * -((float)Acc);

            return CellAcc;
        }

        

        public static int getLayerFromAltitude(CelestialBody body, double Altitude)
        {
            if (Math.Round((Altitude / KWSKSPButtToucher.Settings.cellDefinitionAlt), 1) <= HeadMaster.PlanetMap[body].LiveMap.Count)
            {
                return (int)Math.Round((Altitude / KWSKSPButtToucher.Settings.cellDefinitionAlt), 1);
            }
            else { throw null; }
            //return null;
        }

        public static IEnumerable<Cell> getCellNeighbours(int AltLayer, Cell cell)
        {
            return cell.GetNeighbors(KWSKSPButtToucher.Settings.gridLevel);
        }

        public static float GetCellWindSpeed(CelestialBody body, int AltLayer, Cell cell)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;
            float cellWindSpeed = Layers[AltLayer][cell].WindDirection.magnitude;
            return cellWindSpeed;
        }

        public static Vector3 getCellWindDirection(CelestialBody body, int AltLayer, Cell cell)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;
            Vector3 cellWindDirection = Layers[AltLayer][cell].WindDirection;
            return cellWindDirection;
        }

        public static bool isCellHigherPressure(CelestialBody body, int AltLayer, Cell cellA, Cell cellB)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;
            if (Layers[AltLayer][cellA].Pressure > Layers[AltLayer][cellB].Pressure) { return true; }
            else { return false; }

        }
        public static bool isCellHigherTemperature(CelestialBody body, int AltLayer, Cell cellA, Cell cellB)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;

            if (Layers[AltLayer][cellA].Temperature > Layers[AltLayer][cellB].Temperature) { return true; }
            else { return false; }

        }
        public static bool isCellMoreHumid(CelestialBody body, int AltLayer, Cell cellA, Cell cellB)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;

            if (Layers[AltLayer][cellA].Humidity > Layers[AltLayer][cellB].Humidity) { return true; }
            else { return false; }

        }

        public static uint getNumberOfCells(int gridLevel)
        {
            uint numberOfCells = Cell.CountAtLevel(gridLevel);
            return numberOfCells;
        }

        public static double getCellAltitude(CelestialBody body, int AltLayer, Cell cell)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;
            return Layers[AltLayer][cell].Altitude;
        }

        public static double getCellHumidity(CelestialBody body, int AltLayer, Cell cell)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;
            return Layers[AltLayer][cell].Humidity;
        }

        public static double getCellTemperature(CelestialBody body, int AltLayer, Cell cell)
        {
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].LiveMap;
            //Debug.Log(Layers.Count);
            //Debug.Log(Layers[AltLayer]);


            return Layers[AltLayer][cell].Temperature;
        }

        public static double CalculateCellPressure(CelestialBody body, int AltLayer, Cell cell)
        {
            //Pressure equation when lapse rate != 0, which = -6.5C/km
            //Pressure = Pb * (Tb/ (Tb + Lb) * (h - hb)) ^ (g0 * M / R* * Lb)
            //P = P(static) * (Temperature(K) / (Temperature(K) + TempLapseRate) * (h of layer - h of bottom of layer))) ^ ((9.8m/s * 0.0289644km/mol) / 8.31432 * TempLapseRate)
            //Debug.Log("Pressure: " + HeadMaster.PlanetMap[body][cell].Pressure);
            //double Pressure = (FlightGlobals.getStaticPressure(HeadMaster.PlanetMap[body][cell].Altitude, body)) * Math.Pow(((HeadMaster.PlanetMap[body][cell].Temperature) / ((HeadMaster.PlanetMap[body][cell].Temperature + -0.0065) * (((HeadMaster.PlanetMap[body][cell].Altitude + KWSKSPButtToucher.Settings.cellDefinitionAlt) - HeadMaster.PlanetMap[body][cell].Altitude)))), ((9.80665 * 0.028964) / (8.31432 * -0.0065)));
            //double Pressure = (FlightGlobals.getStaticPressure(HeadMaster.PlanetMap[body][cell].Altitude, body)) * Math.Pow(((273.15 + HeadMaster.PlanetMap[body][cell].Temperature) / ((273.15 + HeadMaster.PlanetMap[body][cell].Temperature + -0.0065) * (((HeadMaster.PlanetMap[body][cell].Altitude + KWSKSPButtToucher.Settings.cellDefinitionAlt) - HeadMaster.PlanetMap[body][cell].Altitude)))), ((9.80665 * 0.028964) / (8.31432 * -0.0065)));
            //Debug.Log(HeadMaster.PlanetMap[body][cell].Temperature);
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].BufferMap;
            double TLR;
            if (AltLayer + 1 < Layers.Count)
            {
                TLR = 1000 * ((Layers[(AltLayer + 1)][cell].Temperature - Layers[AltLayer][cell].Temperature) / (Layers[(AltLayer + 1)][cell].Altitude - Layers[AltLayer][cell].Altitude));
            }
            else
            {
                TLR = 1000 * ((2.7 - Layers[AltLayer][cell].Temperature) / (Layers[AltLayer][cell].Altitude + KWSKSPButtToucher.Settings.cellDefinitionAlt - Layers[AltLayer][cell].Altitude));
            }

            //Debug.Log(TLR);
            double Pressure = FlightGlobals.getStaticPressure(Layers[AltLayer][cell].Altitude, body) * Math.Pow(((Layers[AltLayer][cell].Temperature) / ((Layers[AltLayer][cell].Temperature) + TLR) * ((Layers[AltLayer][cell].Altitude + KWSKSPButtToucher.Settings.cellDefinitionAlt))), ((body.GeeASL * 0.028964) / (8.31432 * TLR)));


            //double Pressure = cell.Density / (287.058f * (273.15f +  cell.Temperature));
            //double Pressure = cell.Density / (287.058f * (cell.Temperature));


            return Pressure;

        }

        public static double CalculateCellDensity(CelestialBody body, int AltLayer, Cell cell)
        {
            //density = densityb * (Tempb + Lapseb * (height - heightb) / Tempb)^(gravacc * Molarmas / gas constant * Lapserate) - 1
            //Debug.Log("Density: " + HeadMaster.PlanetMap[body][cell].Density);
            //double density = HeadMaster.PlanetMap[body][cell].Density * Math.Pow(((((HeadMaster.PlanetMap[body][cell].Temperature + -0.0065) * (HeadMaster.PlanetMap[body][cell].Altitude + KWSKSPButtToucher.Settings.cellDefinitionAlt) - HeadMaster.PlanetMap[body][cell].Altitude)) / (HeadMaster.PlanetMap[body][cell].Temperature)), (-(9.80665 - 0.028964) / (8.31432 - -0.0065) - 1));
            List<CellMap<WeatherCell>> Layers = HeadMaster.PlanetMap[body].BufferMap;

            double test = FlightGlobals.getAtmDensity(FlightGlobals.getStaticPressure(Layers[AltLayer][cell].Altitude, body));
            double TLR;
            if (AltLayer + 1 < Layers.Count)
            {
                TLR = -((Layers[(AltLayer + 1)][cell].Temperature - Layers[AltLayer][cell].Temperature) / (Layers[(AltLayer + 1)][cell].Altitude - Layers[AltLayer][cell].Altitude));
            }
            else
            {
                TLR = -((2.7 - Layers[AltLayer][cell].Temperature) / (Layers[AltLayer][cell].Altitude + KWSKSPButtToucher.Settings.cellDefinitionAlt - Layers[AltLayer][cell].Altitude));
            }

            double density = test * Math.Pow(((((Layers[AltLayer][cell].Temperature) + TLR * (Layers[AltLayer][cell].Altitude + KWSKSPButtToucher.Settings.cellDefinitionAlt) - Layers[AltLayer][cell].Altitude)) / (Layers[AltLayer][cell].Temperature)), ((-(body.GeeASL * 0.0289644) / (8.31432 * TLR)) - 1));

            //double density = HeadMaster.PlanetMap[body][cell].Density * Math.Pow(((((273.15 + HeadMaster.PlanetMap[body][cell].Temperature + -0.0065) * (HeadMaster.PlanetMap[body][cell].Altitude + KWSKSPButtToucher.Settings.cellDefinitionAlt) - HeadMaster.PlanetMap[body][cell].Altitude)) / (273.15 + HeadMaster.PlanetMap[body][cell].Temperature)), (-(9.80665 * 0.028964) / (8.31432 * -0.0065) - 1));

            return density;
        }

        public static Vector3 getCellPosition(CelestialBody body, int AltLayer, Cell cell)
        {

            return cell.Position;
        }

        public static Cell getCellAtLocation(CelestialBody body, Vector3 worldPos)
        {
            Cell cell = Cell.Containing(body.transform.InverseTransformPoint(worldPos), KWSKSPButtToucher.Settings.gridLevel);
            return cell;
        }

        internal static float GetOffSetMultiplier() //Magical numbers that idk the meaning of
        {
            try
            {
                CelestialBody body = FlightGlobals.currentMainBody;
                //offset multipliers for 1m/s of windspeed.
                if (body.bodyName == "Kerbin") { offSetMultiplier = 0.0000025f; }
                else if (body.bodyName == "Laythe") { offSetMultiplier = 0.00000011f; }
                else if (body.bodyName == "Duna") { offSetMultiplier = 0.000003f; }
                else if (body.bodyName == "Eve") { offSetMultiplier = -0.000001f; }
                else if (body.bodyName == "Jool") { offSetMultiplier = 0.000005f; }
                else { offSetMultiplier = 0.0000025f; }
            }
            catch
            {
                //Debug.Log("Null body");
                offSetMultiplier = 0.0000025f; //default to kerbin's offset rate
            }
            return offSetMultiplier;
        }

        public static bool IsSunlight(CelestialBody body, int AltLayer, Cell cell)
        {

            if (getSunlightAngle(AltLayer, cell) <= 91)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
