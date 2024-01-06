using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace StarSystem
{
    class Star
    {
        public double mass;
        public double temperature;
        public double luminosity;
        public double radius;
        public string spectralType;

        public Star(double age)
        {
            Random random = new Random();
            double rand = random.NextDouble();

            mass = (0.140975 - 0.358227 * Math.Log(rand) + 1.73588 * Math.Pow(0.0150929, rand))/ 2;

            double mspan = (11.7026 * Math.Pow(mass, -2.32776) + 326.646 * Math.Pow(0.0326696, mass))/2;
            double sspan = 1.57285 * Math.Pow(mass, -3.00266);
            double gspan = 0.956059 * Math.Pow(mass, -2.99456);

            spectralType = "undefined";

            if (age < mspan)
            {
                temperature = 2631.58 + 2898.5 * mass;

                double lmin, lmax;

                if (mass < 0.45)
                {
                    luminosity = 0.682477 * Math.Pow(mass, 2.77149);
                }
                else if (mass < 1)
                {
                    lmin = 0.682477 * Math.Pow(mass, 2.77149);
                    lmax = 0.00954249 * Math.Pow(179.629, mass);

                    luminosity = lmin + ((age / mspan) * (lmax - lmin));
                }
                else
                {
                    lmin = 12.9808 * Math.Pow(mass, 2) - 23.7521 * mass + 11.4966;
                    lmax = 12.7839 * Math.Pow(mass, 3) - 40.2089 * Math.Pow(mass, 2) + 49.5648 * mass - 20.6165;

                    luminosity = lmin + ((age / mspan) * (lmax - lmin));
                }

                radius = (155000 * Math.Sqrt(luminosity)) / Math.Pow(temperature, 2);

                if (temperature < 2700)
                {
                    spectralType = "M8";
                }
                else if (temperature < 2900)
                {
                    spectralType = "M7";
                }
                else if (temperature < 3100)
                {
                    spectralType = "M6";
                }
                else if (temperature < 3200)
                {
                    spectralType = "M5";
                }
                else if (temperature < 3400)
                {
                    spectralType = "M4";
                }
                else if (temperature < 3500)
                {
                    spectralType = "M3";
                }
                else if (temperature < 3600)
                {
                    spectralType = "M2";
                }
                else if (temperature < 3700)
                {
                    spectralType = "M1";
                }
                else if (temperature < 3750)
                {
                    spectralType = "M0";
                }
                else if (temperature < 3900)
                {
                    spectralType = "K8";
                }
                else if (temperature < 4000)
                {
                    spectralType = "K7";
                }
                else if (temperature < 4200)
                {
                    spectralType = "K6";
                }
                else if (temperature < 4400)
                {
                    spectralType = "K5";
                }
                else if (temperature < 4600)
                {
                    spectralType = "K4";
                }
                else if (temperature < 4800)
                {
                    spectralType = "K3";
                }
                else if (temperature < 4960)
                {
                    spectralType = "K2";
                }
                else if (temperature < 5110)
                {
                    spectralType = "K1";
                }
                else if (temperature < 5240)
                {
                    spectralType = "K0";
                }
                else if (temperature < 5440)
                {
                    spectralType = "G8";
                }
                else if (temperature < 5510)
                {
                    spectralType = "G7";
                }
                else if (temperature < 5580)
                {
                    spectralType = "G6";
                }
                else if (temperature < 5660)
                {
                    spectralType = "G5";
                }
                else if (temperature < 5700)
                {
                    spectralType = "G4";
                }
                else if (temperature < 5750)
                {
                    spectralType = "G3";
                }
                else if (temperature < 5800)
                {
                    spectralType = "G2";
                }
                else if (temperature < 5930)
                {
                    spectralType = "G1";
                }
                else if (temperature < 6050)
                {
                    spectralType = "G0";
                }
                else if (temperature < 6300)
                {
                    spectralType = "F8";
                }
                else if (temperature < 6400)
                {
                    spectralType = "F7";
                }
                else if (temperature < 6550)
                {
                    spectralType = "F6";
                }
                else if (temperature < 6700)
                {
                    spectralType = "F5";
                }
                else if (temperature < 6770)
                {
                    spectralType = "F4";
                }
                else if (temperature < 6850)
                {
                    spectralType = "F3";
                }
                else if (temperature < 7050)
                {
                    spectralType = "F2";
                }
                else if (temperature < 7200)
                {
                    spectralType = "F1";
                }
                else if (temperature < 7350)
                {
                    spectralType = "F0";
                }
                else if (temperature < 7640)
                {
                    spectralType = "A8";
                }
                else if (temperature < 7920)
                {
                    spectralType = "A7";
                }
                else if (temperature < 8120)
                {
                    spectralType = "A6";
                }
                else if (temperature < 8310)
                {
                    spectralType = "A5";
                }
                else
                {
                    spectralType = "A4";
                }

                spectralType += "V";
            }
            else if (mass < 0.9)
            {
                mass = 0.9 + 0.05 * (5 * random.NextDouble() + 5 * random.NextDouble());
                temperature = 0;
                luminosity = 0;
                radius = 0;
                spectralType = "D";
            }
            else if (age < sspan + mspan)
            {
                double mtemp = 2631.58 + 2898.5 * mass;
                temperature = mtemp - (((age - mspan) / sspan) * (mtemp - 4800));

                if (mass < 1)
                {
                    luminosity = 0.00954249 * Math.Pow(179.629, mass);
                }
                else
                {
                    luminosity = 12.7839 * Math.Pow(mass, 3) - 40.2089 * Math.Pow(mass, 2) + 49.5648 * mass - 20.6165;
                }

                radius = (155000 * Math.Sqrt(luminosity)) / Math.Pow(temperature, 2);

                if (temperature < 2700)
                {
                    spectralType = "M8";
                }
                else if (temperature < 2900)
                {
                    spectralType = "M7";
                }
                else if (temperature < 3100)
                {
                    spectralType = "M6";
                }
                else if (temperature < 3200)
                {
                    spectralType = "M5";
                }
                else if (temperature < 3400)
                {
                    spectralType = "M4";
                }
                else if (temperature < 3500)
                {
                    spectralType = "M3";
                }
                else if (temperature < 3600)
                {
                    spectralType = "M2";
                }
                else if (temperature < 3700)
                {
                    spectralType = "M1";
                }
                else if (temperature < 3750)
                {
                    spectralType = "M0";
                }
                else if (temperature < 3900)
                {
                    spectralType = "K8";
                }
                else if (temperature < 4000)
                {
                    spectralType = "K7";
                }
                else if (temperature < 4200)
                {
                    spectralType = "K6";
                }
                else if (temperature < 4400)
                {
                    spectralType = "K5";
                }
                else if (temperature < 4600)
                {
                    spectralType = "K4";
                }
                else if (temperature < 4800)
                {
                    spectralType = "K3";
                }
                else if (temperature < 4960)
                {
                    spectralType = "K2";
                }
                else if (temperature < 5110)
                {
                    spectralType = "K1";
                }
                else if (temperature < 5240)
                {
                    spectralType = "K0";
                }
                else if (temperature < 5440)
                {
                    spectralType = "G8";
                }
                else if (temperature < 5510)
                {
                    spectralType = "G7";
                }
                else if (temperature < 5580)
                {
                    spectralType = "G6";
                }
                else if (temperature < 5660)
                {
                    spectralType = "G5";
                }
                else if (temperature < 5700)
                {
                    spectralType = "G4";
                }
                else if (temperature < 5750)
                {
                    spectralType = "G3";
                }
                else if (temperature < 5800)
                {
                    spectralType = "G2";
                }
                else if (temperature < 5930)
                {
                    spectralType = "G1";
                }
                else if (temperature < 6050)
                {
                    spectralType = "G0";
                }
                else if (temperature < 6300)
                {
                    spectralType = "F8";
                }
                else if (temperature < 6400)
                {
                    spectralType = "F7";
                }
                else if (temperature < 6550)
                {
                    spectralType = "F6";
                }
                else if (temperature < 6700)
                {
                    spectralType = "F5";
                }
                else if (temperature < 6770)
                {
                    spectralType = "F4";
                }
                else if (temperature < 6850)
                {
                    spectralType = "F3";
                }
                else if (temperature < 7050)
                {
                    spectralType = "F2";
                }
                else if (temperature < 7200)
                {
                    spectralType = "F1";
                }
                else if (temperature < 7350)
                {
                    spectralType = "F0";
                }
                else if (temperature < 7640)
                {
                    spectralType = "A8";
                }
                else if (temperature < 7920)
                {
                    spectralType = "A7";
                }
                else if (temperature < 8120)
                {
                    spectralType = "A6";
                }
                else if (temperature < 8310)
                {
                    spectralType = "A5";
                }
                else
                {
                    spectralType = "A4";
                }

                spectralType += "IV";
            }
            else if (age < mspan + sspan + mspan)
            {
                temperature = (5 * random.NextDouble() + 5 * random.NextDouble()) * 200 + 3000;

                if (mass < 1)
                {
                    luminosity = 25 * 0.00954249 * Math.Pow(179.629, mass);
                }
                else
                {
                    luminosity = 25 * (12.7839 * Math.Pow(mass, 3) - 40.2089 * Math.Pow(mass, 2) + 49.5648 * mass - 20.6165);
                }

                radius = (155000 * Math.Sqrt(luminosity)) / Math.Pow(temperature, 2);

                if (temperature < 2700)
                {
                    spectralType = "M8";
                }
                else if (temperature < 2900)
                {
                    spectralType = "M7";
                }
                else if (temperature < 3100)
                {
                    spectralType = "M6";
                }
                else if (temperature < 3200)
                {
                    spectralType = "M5";
                }
                else if (temperature < 3400)
                {
                    spectralType = "M4";
                }
                else if (temperature < 3500)
                {
                    spectralType = "M3";
                }
                else if (temperature < 3600)
                {
                    spectralType = "M2";
                }
                else if (temperature < 3700)
                {
                    spectralType = "M1";
                }
                else if (temperature < 3750)
                {
                    spectralType = "M0";
                }
                else if (temperature < 3900)
                {
                    spectralType = "K8";
                }
                else if (temperature < 4000)
                {
                    spectralType = "K7";
                }
                else if (temperature < 4200)
                {
                    spectralType = "K6";
                }
                else if (temperature < 4400)
                {
                    spectralType = "K5";
                }
                else if (temperature < 4600)
                {
                    spectralType = "K4";
                }
                else if (temperature < 4800)
                {
                    spectralType = "K3";
                }
                else if (temperature < 4960)
                {
                    spectralType = "K2";
                }
                else if (temperature < 5110)
                {
                    spectralType = "K1";
                }
                else if (temperature < 5240)
                {
                    spectralType = "K0";
                }
                else if (temperature < 5440)
                {
                    spectralType = "G8";
                }
                else if (temperature < 5510)
                {
                    spectralType = "G7";
                }
                else if (temperature < 5580)
                {
                    spectralType = "G6";
                }
                else if (temperature < 5660)
                {
                    spectralType = "G5";
                }
                else if (temperature < 5700)
                {
                    spectralType = "G4";
                }
                else if (temperature < 5750)
                {
                    spectralType = "G3";
                }
                else if (temperature < 5800)
                {
                    spectralType = "G2";
                }
                else if (temperature < 5930)
                {
                    spectralType = "G1";
                }
                else if (temperature < 6050)
                {
                    spectralType = "G0";
                }
                else if (temperature < 6300)
                {
                    spectralType = "F8";
                }
                else if (temperature < 6400)
                {
                    spectralType = "F7";
                }
                else if (temperature < 6550)
                {
                    spectralType = "F6";
                }
                else if (temperature < 6700)
                {
                    spectralType = "F5";
                }
                else if (temperature < 6770)
                {
                    spectralType = "F4";
                }
                else if (temperature < 6850)
                {
                    spectralType = "F3";
                }
                else if (temperature < 7050)
                {
                    spectralType = "F2";
                }
                else if (temperature < 7200)
                {
                    spectralType = "F1";
                }
                else if (temperature < 7350)
                {
                    spectralType = "F0";
                }
                else if (temperature < 7640)
                {
                    spectralType = "A8";
                }
                else if (temperature < 7920)
                {
                    spectralType = "A7";
                }
                else if (temperature < 8120)
                {
                    spectralType = "A6";
                }
                else if (temperature < 8310)
                {
                    spectralType = "A5";
                }
                else
                {
                    spectralType = "A4";
                }

                spectralType += "III";
            }
            else
            {
                mass = 0.9 + 0.05 * (5 * random.NextDouble() + 5 * random.NextDouble());
                temperature = 0;
                luminosity = 0;
                radius = 0;
                spectralType = "D";
            }

            if (spectralType.ToString().StartsWith("M"))
            {
                rand = random.NextDouble();

                if (rand < 0.375)
                {
                    spectralType += "e";
                }
            }
        }

        public static IEnumerable<XElement> createPlanetarySystem(double innerLimit, double outerLimit, double snowLine, int forbidInt)
        {
            XElement outputRoot = new XElement("root");

            Random random = new Random();
            double rand = random.NextDouble();

            bool giantPlaced = false;
            int giantMode;

            if (rand < 0.5)
            {
                //No gas giant
                giantMode = 0;
            }
            else if (rand < 0.7407)
            {
                //Conventional gas giant
                giantMode = 1;
                rand = 0.05 * ((1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()) - 2) + 1;

                if (rand * snowLine > innerLimit && rand * snowLine < outerLimit)
                {
                    XElement giant = new XElement("giant");
                    giant.Add(new XAttribute("orbitalRadius", rand * snowLine));

                    outputRoot.Add(giant);
                    giantPlaced = true;
                }

                outputRoot.Add(new XElement("conventional"));
            }
            else if (rand < 0.9074)
            {
                //Eccentric gas giant
                giantMode = 2;
                rand = 0.125 * (1 + 5 * random.NextDouble());

                if (rand * snowLine > innerLimit && rand * snowLine < outerLimit)
                {
                    XElement giant = new XElement("giant");
                    giant.Add(new XAttribute("orbitalRadius", rand * snowLine));

                    if (rand < snowLine)
                    {
                        giant.Add(new XAttribute("eccentric", "true"));
                    }

                    outputRoot.Add(giant);
                    giantPlaced = true;
                }

                outputRoot.Add(new XElement("eccentric"));
            }
            else
            {
                //Epistellar gas giant
                giantMode = 3;
                rand = 0.1 * ((1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()));

                XElement giant = new XElement("giant");
                giant.Add(new XAttribute("orbitalRadius", rand * innerLimit));
                giant.Add(new XAttribute("epistellar", "true"));

                outputRoot.Add(giant);
                outputRoot.Add(new XElement("epistellar"));
                giantPlaced = true;
            }

            double initialOrbit = outerLimit;
            double currentOrbit = initialOrbit;

            if (giantPlaced)
            {
                IEnumerable<XElement> giants = from el in outputRoot.Elements("giant") select el;
                foreach (XElement giant in giants)
                {
                    if (Convert.ToDouble(giant.Attribute("orbitalRadius").Value) > innerLimit)
                    {
                        initialOrbit = Convert.ToDouble(giant.Attribute("orbitalRadius").Value);
                        currentOrbit = initialOrbit;
                    }
                    else
                    {
                        giantPlaced = false;
                    }
                }
            }

            //loop to go inwards from inital orbit
            while (true)
            {
                rand = random.NextDouble();
                double ratio = 2.26018 * Math.Pow(rand, 3) - 3.27229 * Math.Pow(rand, 2) + 1.60233 * rand + 1.37265;

                currentOrbit /= ratio;

                if (currentOrbit < innerLimit)
                {
                    break;
                }

                XElement orbit = new XElement("orbit");
                orbit.Add(new XAttribute("orbitalRadius", currentOrbit));

                outputRoot.Add(orbit);
            }

            currentOrbit = initialOrbit;

            //loop to go out
            if (giantPlaced)
            {
                while (true)
                {
                    rand = random.NextDouble();
                    double ratio = 2.26018 * Math.Pow(rand, 3) - 3.27229 * Math.Pow(rand, 2) + 1.60233 * rand + 1.37265;

                    currentOrbit *= ratio;

                    if (currentOrbit > outerLimit)
                    {
                        break;
                    }

                    XElement orbit = new XElement("orbit");
                    orbit.Add(new XAttribute("orbitalRadius", currentOrbit));

                    outputRoot.Add(orbit);
                }
            }

            IEnumerable<XElement> sortedOrbits = from el in outputRoot.Elements() where el.HasAttributes orderby Convert.ToDouble(el.Attribute("orbitalRadius").Value) select el;
            outputRoot = new XElement("root");
            foreach (XElement sortedOrbit in sortedOrbits)
            {
                outputRoot.Add(sortedOrbit);
            }

            //place other gas giants
            if (giantMode != 0)
            {
                IEnumerable<XElement> orbits = from el in outputRoot.Elements("orbit") select el;
                foreach (XElement orbit in orbits)
                {
                    rand = random.NextDouble();
                    switch (giantMode)
                    {
                        case 0:
                            break;
                        case 1:
                            if (Convert.ToDouble(orbit.Attribute("orbitalRadius").Value) < snowLine)
                            {
                                break;
                            }
                            else
                            {
                                if (rand < 0.9537)
                                {
                                    orbit.Name = "giant";
                                }
                            }
                            break;
                        case 2:
                            if (Convert.ToDouble(orbit.Attribute("orbitalRadius").Value) < snowLine)
                            {
                                if (rand < 0.25926)
                                {
                                    orbit.Name = "giant";
                                    orbit.Add(new XAttribute("eccentric", "true"));
                                }
                            }
                            else
                            {
                                if (rand < 0.9074)
                                {
                                    orbit.Name = "giant";
                                }
                            }
                            break;
                        case 3:
                            if (Convert.ToDouble(orbit.Attribute("orbitalRadius").Value) < snowLine)
                            {
                                if (rand < 0.0926)
                                {
                                    orbit.Name = "giant";
                                }
                            }
                            else
                            {
                                if (rand < 0.9074)
                                {
                                    orbit.Name = "giant";
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            //size gas giants
            if (giantMode != 0)
            {
                IEnumerable<XElement> giants = from el in outputRoot.Elements("giant") select el;
                foreach (XElement giant in giants)
                {
                    rand = random.NextDouble();

                    try
                    {
                        if (Convert.ToDouble(giant.ElementsBeforeSelf().Last<XElement>().Attribute("orbitalRadius").Value) < snowLine)
                        {
                            if (rand < 0.0926)
                            {
                                giant.Add(new XAttribute("class", "Small"));
                            }
                            else if (rand < 0.7407)
                            {
                                giant.Add(new XAttribute("class", "Medium"));
                            }
                            else
                            {
                                giant.Add(new XAttribute("class", "Large"));
                            }
                        }
                        else
                        {
                            if (rand < 0.5)
                            {
                                giant.Add(new XAttribute("class", "Small"));
                            }
                            else if (rand < 0.9815)
                            {
                                giant.Add(new XAttribute("class", "Medium"));
                            }
                            else
                            {
                                giant.Add(new XAttribute("class", "Large"));
                            }
                        }
                    }
                    catch
                    {
                        if (rand < 0.5)
                        {
                            giant.Add(new XAttribute("class", "Small"));
                        }
                        else if (rand < 0.9815)
                        {
                            giant.Add(new XAttribute("class", "Medium"));
                        }
                        else
                        {
                            giant.Add(new XAttribute("class", "Large"));
                        }
                    }
                }
            }

            //fill other orbits
            IEnumerable<XElement> otherOrbits = from el in outputRoot.Elements("orbit") select el;
            foreach (XElement otherOrbit in otherOrbits)
            {
                rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());
                if ((otherOrbit.ElementsAfterSelf() == null && forbidInt == 2) || (otherOrbit.ElementsBeforeSelf() == null && forbidInt == 1))
                {
                    //Adjacent to verboden zone
                    rand -= 6;
                }
                try
                {
                    if (otherOrbit.ElementsAfterSelf().First<XElement>().Name == "giant")
                    {
                        //Inside of giant
                        rand -= 6;
                    }
                }
                catch
                {

                }
                try
                {
                    if (otherOrbit.ElementsBeforeSelf().Last<XElement>().Name == "giant")
                    {
                        //Outside of giant
                        rand -= 3;
                    }
                }
                catch
                {

                }
                if ((otherOrbit.ElementsBeforeSelf() == null && forbidInt != 1) || (otherOrbit.ElementsAfterSelf() == null && forbidInt != 2))
                {
                    //Adjacent to gravitational limits
                    rand -= 3;
                }

                if (rand < 3)
                {
                    continue;
                }
                else if (rand < 6)
                {
                    otherOrbit.Name = "belt";
                    
                    if (Convert.ToDouble(otherOrbit.Attribute("orbitalRadius").Value) < snowLine)
                    {
                        otherOrbit.Add(new XAttribute("type", "rocky"));
                    }
                    else
                    {
                        otherOrbit.Add(new XAttribute("type", "ice"));
                    }
                }
                else if (rand < 8)
                {
                    otherOrbit.Name = "planet";
                    otherOrbit.Add(new XAttribute("class", "tiny"));
                }
                else if (rand < 11)
                {
                    otherOrbit.Name = "planet";
                    otherOrbit.Add(new XAttribute("class", "small"));
                }
                else if (rand < 15)
                {
                    otherOrbit.Name = "planet";
                    otherOrbit.Add(new XAttribute("class", "standard"));
                }
                else
                {
                    otherOrbit.Name = "planet";
                    otherOrbit.Add(new XAttribute("class", "large"));
                }
            }

            //clear unused orbits
            IEnumerable<XElement> emptyOrbits = from el in outputRoot.Elements("orbit") select el;
            int numEmptyOrbits = emptyOrbits.Count<XElement>();
            for (int i = 0; i < numEmptyOrbits; i++)
            {
                foreach (XElement emptyOrbit in emptyOrbits)
                {
                    emptyOrbit.Remove();
                    break;
                }
            }

            //give giants moons
            IEnumerable<XElement> giantsNeedMooned = from el in outputRoot.Elements("giant") select el;
            foreach (XElement giantNeedsMooned in giantsNeedMooned)
            {
                //Do rings
                XElement rings = new XElement("rings");

                rand = random.Next(1, 7) + random.Next(1, 7);
                if (Convert.ToDouble(giantNeedsMooned.Attribute("orbitalRadius").Value) < 0.1)
                {
                    rand -= 10;
                }
                else if (Convert.ToDouble(giantNeedsMooned.Attribute("orbitalRadius").Value) < 0.5)
                {
                    rand -= 8;
                }
                else if (Convert.ToDouble(giantNeedsMooned.Attribute("orbitalRadius").Value) < 0.75)
                {
                    rand -= 6;
                }
                else if (Convert.ToDouble(giantNeedsMooned.Attribute("orbitalRadius").Value) < 1.5)
                {
                    rand -= 3;
                }

                if (rand < 6)
                {
                    rings.Add(new XAttribute("class", "faint"));
                }
                else if (rand < 10)
                {
                    rings.Add(new XAttribute("class", "standard"));
                }
                else
                {
                    rings.Add(new XAttribute("class", "brilliant"));
                }

                if (Convert.ToDouble(giantNeedsMooned.Attribute("orbitalRadius").Value) < snowLine)
                {
                    rings.Add(new XAttribute("type", "rocky"));
                }
                else
                {
                    rings.Add(new XAttribute("type", "icy"));
                }
                
                for (int i = 0; i < rand; i++)
                {
                    rings.Add(new XElement("shepard"));
                }

                if (rand > 0)
                {
                    giantNeedsMooned.Add(rings);
                }

                //Do major moons
                rand = random.Next(1, 7);
                if (Convert.ToDouble(giantNeedsMooned.Attribute("orbitalRadius").Value) < 0.1)
                {
                    rand -= 6;
                }
                else if (Convert.ToDouble(giantNeedsMooned.Attribute("orbitalRadius").Value) < 0.5)
                {
                    rand -= 5;
                }
                else if (Convert.ToDouble(giantNeedsMooned.Attribute("orbitalRadius").Value) < 0.75)
                {
                    rand -= 4;
                }
                else if (Convert.ToDouble(giantNeedsMooned.Attribute("orbitalRadius").Value) < 1.5)
                {
                    rand -= 1;
                }

                for (int i = 0; i < rand; i++)
                {
                    XElement moon = new XElement("moon");

                    int rand1 = random.Next(1, 7) + random.Next(1, 7) + random.Next(1, 7);

                    if (rand1 < 12)
                    {
                        moon.Add(new XAttribute("class", "tiny"));
                    }
                    else if (rand1 < 15)
                    {
                        moon.Add(new XAttribute("class", "small"));
                    }
                    else
                    {
                        moon.Add(new XAttribute("class", "standard"));
                    }

                    giantNeedsMooned.Add(moon);
                }

                //Do captured moonlets
                rand = random.Next(1, 7);
                if (Convert.ToDouble(giantNeedsMooned.Attribute("orbitalRadius").Value) < 0.5)
                {
                    rand -= 6;
                }
                else if (Convert.ToDouble(giantNeedsMooned.Attribute("orbitalRadius").Value) < 0.75)
                {
                    rand -= 5;
                }
                else if (Convert.ToDouble(giantNeedsMooned.Attribute("orbitalRadius").Value) < 1.5)
                {
                    rand -= 4;
                }
                else if (Convert.ToDouble(giantNeedsMooned.Attribute("orbitalRadius").Value) < 3)
                {
                    rand -= 1;
                }

                if (rand > 0)
                {
                    XElement moonlets = new XElement("moonlets");
                    moonlets.Add(new XElement("moonlet"));

                    giantNeedsMooned.Add(moonlets);
                }
            }

            //give terrestrials moons
            IEnumerable<XElement> planetsNeedMooned = from el in outputRoot.Elements("planet") select el;
            foreach (XElement planetNeedsMooned in planetsNeedMooned)
            {
                //do major moons
                rand = random.Next(1, 7) - 4;

                if (Convert.ToDouble(planetNeedsMooned.Attribute("orbitalRadius").Value) < 0.5)
                {
                    rand -= 4;
                }
                else if (Convert.ToDouble(planetNeedsMooned.Attribute("orbitalRadius").Value) < 0.75)
                {
                    rand -= 3;
                }
                else if (Convert.ToDouble(planetNeedsMooned.Attribute("orbitalRadius").Value) < 1.5)
                {
                    rand -= 1;
                }

                if (planetNeedsMooned.Attribute("class").Value == "tiny")
                {
                    rand -= 2;
                }
                else if (planetNeedsMooned.Attribute("class").Value == "small")
                {
                    rand -= 1;

                    for (int i = 0; i < rand; i++)
                    {
                        XElement moon = new XElement("moon");
                        moon.Add(new XAttribute("class", "tiny"));

                        planetNeedsMooned.Add(moon);
                    }
                }
                else if (planetNeedsMooned.Attribute("class").Value == "standard")
                {
                    for (int i = 0; i < rand; i++)
                    {
                        XElement moon = new XElement("moon");

                        int rand1 = random.Next(1, 7) + random.Next(1, 7) + random.Next(1, 7);

                        if (rand1 < 15)
                        {
                            moon.Add(new XAttribute("class", "tiny"));
                        }
                        else
                        {
                            moon.Add(new XAttribute("class", "small"));
                        }

                        planetNeedsMooned.Add(moon);
                    }
                }
                else if (planetNeedsMooned.Attribute("class").Value == "large")
                {
                    rand += 1;

                    for (int i = 0; i < rand; i++)
                    {
                        XElement moon = new XElement("moon");

                        int rand1 = random.Next(1, 7) + random.Next(1, 7) + random.Next(1, 7);

                        if (rand1 < 12)
                        {
                            moon.Add(new XAttribute("class", "tiny"));
                        }
                        else if (rand1 < 15)
                        {
                            moon.Add(new XAttribute("class", "small"));
                        }
                        else
                        {
                            moon.Add(new XAttribute("class", "standard"));
                        }

                        planetNeedsMooned.Add(moon);
                    }
                }

                //No moonlets with major moons
                IEnumerable<XElement> moonsSoFar = from el in planetNeedsMooned.Elements("moon") select el;
                if (moonsSoFar.Count() > 0)
                {
                    continue;
                }

                //do moonlets
                XElement moonlets = new XElement("moonlets");

                rand = random.Next(1, 7) - 2;

                if (Convert.ToDouble(planetNeedsMooned.Attribute("orbitalRadius").Value) < 0.5)
                {
                    rand -= 4;
                }
                else if (Convert.ToDouble(planetNeedsMooned.Attribute("orbitalRadius").Value) < 0.75)
                {
                    rand -= 3;
                }
                else if (Convert.ToDouble(planetNeedsMooned.Attribute("orbitalRadius").Value) < 1.5)
                {
                    rand -= 1;
                }

                if (planetNeedsMooned.Attribute("class").Value == "tiny")
                {
                    rand -= 2;
                }
                else if (planetNeedsMooned.Attribute("class").Value == "small")
                {
                    rand -= 1;
                }
                else if (planetNeedsMooned.Attribute("class").Value == "large")
                {
                    rand += 1;
                }

                for (int i = 0; i < rand; i++)
                {
                    moonlets.Add(new XElement("moonlet"));
                }

                if (rand > 0)
                {
                    planetNeedsMooned.Add(moonlets);
                }
            }

            IEnumerable < XElement > output = from el in outputRoot.Elements() select el;
            return output;
        }
    }
}
