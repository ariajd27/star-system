using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace StarSystem
{
    class System
    {
        bool inhabited;

        double age;
        int numStars;
        string name;

        XElement system;

        public System(bool inhabited)
        {
            this.inhabited = inhabited;
            generateSystem(); 
        }

        public void generateSystem()
        {
            system = new XElement("system");

            Random random = new Random();

            age = CalculateSystemAge();
            system.Add(new XAttribute("age", age));

            double rand = random.NextDouble();

            if (rand < 0.5)
            {
                numStars = 1;
            }
            else if (rand < 0.95)
            {
                numStars = 2;
            }
            else
            {
                numStars = 3;
            }

            for (int i = 0; i < numStars; i++)
            {
                Star genStar = new Star(age);

                XElement star = new XElement("star",
                    new XAttribute("mass", genStar.mass),
                    new XAttribute("spectralType", genStar.spectralType),
                    new XAttribute("temperature", genStar.temperature),
                    new XAttribute("luminosity", genStar.luminosity),
                    new XAttribute("radius", genStar.radius)
                    );
                system.Add(star);
            }

            if (numStars > 1)
            {
                doBarycenters();
            }

            doPlanetZones();

            //Create planetary systems
            IEnumerable<XElement> planetaries = from el in system.Descendants() where el.Attribute("snowLine") != null select el;
            foreach (XElement planetary in planetaries)
            {
                double innerLimit = Convert.ToDouble(planetary.Attribute("innerLimit").Value);
                double outerLimit = Convert.ToDouble(planetary.Attribute("outerLimit").Value);
                double snowLine = Convert.ToDouble(planetary.Attribute("snowLine").Value);

                int forbidInt = 0;
                if (planetary.Attribute("forbidInt") != null)
                {
                    forbidInt = Convert.ToInt32(planetary.Attribute("forbidInt").Value);
                }

                IEnumerable<XElement> planets = Star.createPlanetarySystem(innerLimit, outerLimit, snowLine, forbidInt);
                foreach (XElement planet in planets)
                {
                    planetary.Add(planet);
                }
            }

            //Detail the worlds
            IEnumerable<XElement> worldsNeedGenerated = from el in system.Descendants() where el.Name.ToString() == "planet" || el.Name.ToString() == "moon" select el;
            foreach (XElement worldNeedsGenerated in worldsNeedGenerated)
            {
                double parentLuminosity;
                double orbitalRadius;
                bool sulfurPossible = false;
                bool ammoniaPossible = false;

                if (worldNeedsGenerated.Name.ToString() == "planet")
                {
                    parentLuminosity = Convert.ToDouble(worldNeedsGenerated.Parent.Attribute("luminosity").Value);
                    orbitalRadius = Convert.ToDouble(worldNeedsGenerated.Attribute("orbitalRadius").Value);

                    if (Convert.ToDouble(worldNeedsGenerated.Parent.Attribute("mass").Value) < 0.65)
                    {
                        ammoniaPossible = true;
                    }
                }
                else
                {
                    parentLuminosity = Convert.ToDouble(worldNeedsGenerated.Parent.Parent.Attribute("luminosity").Value);
                    orbitalRadius = Convert.ToDouble(worldNeedsGenerated.Parent.Attribute("orbitalRadius").Value);

                    if (Convert.ToDouble(worldNeedsGenerated.Parent.Parent.Attribute("mass").Value) < 0.65)
                    {
                        ammoniaPossible = true;
                    }

                    rand = random.NextDouble();
                    if (worldNeedsGenerated.Parent.Name.ToString() == "giant")
                    {
                        if (worldNeedsGenerated.ElementsBeforeSelf("moon").Count<XElement>() == 0)
                        {
                            if (rand < 0.5)
                            {
                                sulfurPossible = true;
                            }
                        }
                    }
                }

                World world = new World(worldNeedsGenerated.Name.ToString(),
                    worldNeedsGenerated.Attribute("class").Value,
                    orbitalRadius,
                    parentLuminosity,
                    sulfurPossible,
                    ammoniaPossible,
                    Convert.ToDouble(system.Attribute("age").Value));

                worldNeedsGenerated.RemoveAttributes();

                IEnumerable<XAttribute> worldAttributes = world.worldElement().Attributes();
                foreach (XAttribute worldAttribute in worldAttributes)
                {
                    worldNeedsGenerated.Add(worldAttribute);
                }
            }

            //Calculate dynamic parameters and tidy up nodes
            IEnumerable<XElement> planetsNeedDynamics = from el in system.Descendants("planet") select el;
            foreach (XElement planetNeedsDynamics in planetsNeedDynamics)
            {
                double period = Math.Sqrt(Math.Pow(Convert.ToDouble(planetNeedsDynamics.Attribute("orbitalRadius").Value), 3) / (Convert.ToDouble(planetNeedsDynamics.Parent.Attribute("mass").Value) + Convert.ToDouble(planetNeedsDynamics.Attribute("mass").Value)));
                double eccentricity;

                rand = random.NextDouble();
                if (planetNeedsDynamics.Parent.Elements().Contains(new XElement("conventional")))
                {
                    eccentricity = 0.000191044 * Math.Pow(893.236, rand);
                }
                else
                {
                    eccentricity = 0.00411578 * Math.Pow(172.079, rand);
                }

                planetNeedsDynamics.Add(new XAttribute("orbitalPeriod", period));
                planetNeedsDynamics.Add(new XAttribute("eccentricity", eccentricity));

                double tidalEffect = (0.46 * Convert.ToDouble(planetNeedsDynamics.Parent.Attribute("mass").Value) * Convert.ToDouble(planetNeedsDynamics.Attribute("diameter").Value)) / Math.Pow(Convert.ToDouble(planetNeedsDynamics.Attribute("orbitalRadius").Value), 3);

                if (planetNeedsDynamics.Elements("moon") != null)
                {
                    IEnumerable<XElement> moonsNeedDynamics = from el in planetNeedsDynamics.Elements("moon") select el;
                    foreach (XElement moonNeedsDynamics in moonsNeedDynamics) {
                        double orbitalRadius;
                        int sizeDifference = 0;

                        if (moonNeedsDynamics.Attribute("type").Value.StartsWith("Tiny"))
                        {
                            if (moonNeedsDynamics.Parent.Attribute("type").Value.StartsWith("Small"))
                            {
                                sizeDifference = 1;
                            }
                            else if (moonNeedsDynamics.Parent.Attribute("type").Value.StartsWith("Standard"))
                            {
                                sizeDifference = 2;
                            }
                        }
                        else if (moonNeedsDynamics.Attribute("type").Value.StartsWith("Small"))
                        {
                            if (moonNeedsDynamics.Parent.Attribute("type").Value.StartsWith("Standard"))
                            {
                                sizeDifference = 1;
                            }
                            else
                            {
                                sizeDifference = 2;
                            }
                        }
                        else
                        {
                            sizeDifference = 1;
                        }

                        rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());
                        rand += 2 * sizeDifference;
                        orbitalRadius = rand * 2.5 * Convert.ToDouble(moonNeedsDynamics.Parent.Attribute("diameter").Value);

                        try
                        {
                            rand = random.NextDouble();

                            moonNeedsDynamics.ElementsBeforeSelf().Last<XElement>().Attribute("orbitalRadius").SetValue((orbitalRadius - 5) / 2.5 + 5);
                            orbitalRadius = (orbitalRadius - 5) / 2.5 + 5;
                            orbitalRadius *= 2 + 0.6 * rand;
                        }
                        catch
                        {
                            //Do nothing, as there is no other moon!
                        }

                        double orbitalPeriod = 0.0588 * Math.Sqrt(Math.Pow(orbitalRadius, 3) / (Convert.ToDouble(moonNeedsDynamics.Attribute("mass").Value) + Convert.ToDouble(moonNeedsDynamics.Parent.Attribute("mass").Value)));

                        moonNeedsDynamics.Add(new XAttribute("orbitalRadius", orbitalRadius));
                        moonNeedsDynamics.Add(new XAttribute("orbitalPeriod", orbitalPeriod));

                        tidalEffect += (17800000 * Convert.ToDouble(moonNeedsDynamics.Attribute("mass").Value) * Convert.ToDouble(planetNeedsDynamics.Attribute("diameter").Value)) / Math.Pow(orbitalRadius, 3);

                        double moonTidalEffect = (0.46 * Convert.ToDouble(moonNeedsDynamics.Parent.Attribute("mass").Value) * Convert.ToDouble(moonNeedsDynamics.Attribute("diameter").Value)) / Math.Pow(Convert.ToDouble(moonNeedsDynamics.Attribute("orbitalRadius").Value), 3);
                        int moonTidalScore = (int)Math.Round(moonTidalEffect);

                        double moonRotationalPeriod = 0;
                        bool moonTideLockNeeded = false;

                        if (moonTidalScore < 50)
                        {
                            rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());
                            moonRotationalPeriod = rand;

                            moonRotationalPeriod += moonTidalScore;

                            if (moonNeedsDynamics.Attribute("type").Value.StartsWith("Standard"))
                            {
                                moonRotationalPeriod += 10;
                            }
                            else if (moonNeedsDynamics.Attribute("type").Value.StartsWith("Small"))
                            {
                                moonRotationalPeriod += 14;
                            }
                            else
                            {
                                moonRotationalPeriod += 18;
                            }

                            if (rand > 36 || moonRotationalPeriod > 36)
                            {
                                rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());

                                if (rand < 6)
                                {
                                    //It's fine, folks.
                                }
                                else if (rand < 7)
                                {
                                    moonRotationalPeriod = (1 + 5 * random.NextDouble()) * 2 * 24;
                                }
                                else if (rand < 8)
                                {
                                    moonRotationalPeriod = (1 + 5 * random.NextDouble()) * 5 * 24;
                                }
                                else if (rand < 9)
                                {
                                    moonRotationalPeriod = (1 + 5 * random.NextDouble()) * 10 * 24;
                                }
                                else if (rand < 10)
                                {
                                    moonRotationalPeriod = (1 + 5 * random.NextDouble()) * 20 * 24;
                                }
                                else if (rand < 11)
                                {
                                    moonRotationalPeriod = (1 + 5 * random.NextDouble()) * 50 * 24;
                                }
                                else
                                {
                                    moonRotationalPeriod = (1 + 5 * random.NextDouble()) * 100 * 24;
                                }

                                if (orbitalPeriod * 24 < moonRotationalPeriod)
                                {
                                    moonTideLockNeeded = true;
                                }
                            }
                        }
                        else
                        {
                            moonTideLockNeeded = true;
                        }

                        if (moonTideLockNeeded)
                        {
                            moonRotationalPeriod = period * 24;
                            moonNeedsDynamics.Add(new XAttribute("tidalLock", "planet"));
                        }

                        rand = random.NextDouble();
                        if (!moonTideLockNeeded && rand < 0.01852)
                        {
                            moonRotationalPeriod *= -1;
                        }

                        moonNeedsDynamics.Add(new XAttribute("rotationalPeriod", moonRotationalPeriod));
                    }
                }
                if (planetNeedsDynamics.Element("moonlets") != null)
                {
                    IEnumerable<XElement> moonletsNeedDynamics = planetNeedsDynamics.Descendants("moonlet");
                    foreach (XElement moonletNeedsDynamics in moonletsNeedDynamics)
                    {
                        rand = 5 + 5 * random.NextDouble();
                        moonletNeedsDynamics.Add(new XAttribute("orbitalRadius", rand * Convert.ToDouble(planetNeedsDynamics.Attribute("diameter").Value) / 4));
                    }

                    moonletsNeedDynamics = from el in planetNeedsDynamics.Descendants("moonlet") orderby Convert.ToDouble(el.Attribute("orbitalRadius").Value) select el;
                    XElement moonletVault = new XElement("root");
                    foreach (XElement moonletNeedsDynamics in moonletsNeedDynamics)
                    {
                        moonletVault.Add(moonletNeedsDynamics);
                    }
                    planetNeedsDynamics.Element("moonlets").Remove();
                    foreach (XElement vaultMoonlet in moonletVault.Elements())
                    {
                        planetNeedsDynamics.Add(vaultMoonlet);
                    }
                }

                tidalEffect *= age / Convert.ToDouble(planetNeedsDynamics.Attribute("mass").Value);
                int tidalScore = (int)Math.Round(tidalEffect);

                double rotationalPeriod = 0;
                bool tideLockNeeded = false;

                if (tidalScore < 50)
                {
                    rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());
                    rotationalPeriod = rand;

                    rotationalPeriod += tidalScore;

                    if (planetNeedsDynamics.Attribute("type").Value.StartsWith("Large"))
                    {
                        rotationalPeriod += 6;
                    }
                    else if (planetNeedsDynamics.Attribute("type").Value.StartsWith("Standard"))
                    {
                        rotationalPeriod += 10;
                    }
                    else if (planetNeedsDynamics.Attribute("type").Value.StartsWith("Small"))
                    {
                        rotationalPeriod += 14;
                    }
                    else
                    {
                        rotationalPeriod += 18;
                    }

                    if (rand > 36 || rotationalPeriod > 36)
                    {
                        rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());

                        if (rand < 6)
                        {
                            //It's fine, folks.
                        }
                        else if (rand < 7)
                        {
                            rotationalPeriod = (1 + 5 * random.NextDouble()) * 2 * 24;
                        }
                        else if (rand < 8)
                        {
                            rotationalPeriod = (1 + 5 * random.NextDouble()) * 5 * 24;
                        }
                        else if (rand < 9)
                        {
                            rotationalPeriod = (1 + 5 * random.NextDouble()) * 10 * 24;
                        }
                        else if (rand < 10)
                        {
                            rotationalPeriod = (1 + 5 * random.NextDouble()) * 20 * 24;
                        }
                        else if (rand < 11)
                        {
                            rotationalPeriod = (1 + 5 * random.NextDouble()) * 50 * 24;
                        }
                        else
                        {
                            rotationalPeriod = (1 + 5 * random.NextDouble()) * 100 * 24;
                        }

                        double criticalRotationalPeriod;
                        if (planetNeedsDynamics.Element("moon") != null)
                        {
                            criticalRotationalPeriod = Convert.ToDouble(planetNeedsDynamics.Element("moon").Attribute("orbitalPeriod").Value) * 24;
                        }
                        else
                        {
                            criticalRotationalPeriod = period * 8760;
                        }

                        if (criticalRotationalPeriod < rotationalPeriod)
                        {
                            tideLockNeeded = true;
                        }
                    }
                }
                else
                {
                    tideLockNeeded = true;
                }

                if (tideLockNeeded)
                {
                    if (planetNeedsDynamics.Element("moon") != null)
                    {
                        rotationalPeriod = Convert.ToDouble(planetNeedsDynamics.Element("moon").Attribute("orbitalPeriod").Value) * 24;
                        planetNeedsDynamics.Add(new XAttribute("tidalLock", "moon"));
                    }
                    else
                    {
                        rotationalPeriod = period * 8760;
                        planetNeedsDynamics.Add(new XAttribute("tidalLock", "star"));

                        if (Convert.ToDouble(planetNeedsDynamics.Attribute("eccentricity").Value) >= 0.1)
                        {
                            rand = random.NextDouble();
                            if (rand < 0.375)
                            {
                                planetNeedsDynamics.Attribute("tidalLock").SetValue("resonant");
                                rotationalPeriod *= 2;
                            }
                        }

                        if (planetNeedsDynamics.Attribute("tidalLock").Value == "star")
                        {
                            double dayTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value);
                            double nightTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value);

                            switch (planetNeedsDynamics.Attribute("atmoType").Value)
                            {
                                case "Trace":
                                    planetNeedsDynamics.Attribute("atmoType").Remove();
                                    planetNeedsDynamics.Attribute("atmoPressure").Remove();
                                    planetNeedsDynamics.Attribute("hydroCoverage").SetValue(0);
                                    dayTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value) * 1.2;
                                    nightTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value) * 0.1;
                                    break;
                                case "Very Thin":
                                    planetNeedsDynamics.Attribute("atmoType").SetValue("Trace");
                                    planetNeedsDynamics.Attribute("atmoPressure").SetValue(0);
                                    planetNeedsDynamics.Attribute("hydroCoverage").SetValue(0);
                                    dayTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value) * 1.2;
                                    nightTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value) * 0.1;
                                    break;
                                case "Thin":
                                    planetNeedsDynamics.Attribute("atmoType").SetValue("Very Thin");
                                    planetNeedsDynamics.Attribute("atmoPressure").SetValue(Convert.ToDouble(planetNeedsDynamics.Attribute("atmoPressure").Value) - 0.3);
                                    planetNeedsDynamics.Attribute("hydroCoverage").SetValue(Math.Clamp(Convert.ToDouble(planetNeedsDynamics.Attribute("hydroCoverage").Value) - 50, 0, 100));
                                    dayTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value) * 1.16;
                                    nightTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value) * 0.67;
                                    break;
                                case "Standard":
                                    planetNeedsDynamics.Attribute("atmoType").SetValue("Thin");
                                    planetNeedsDynamics.Attribute("atmoPressure").SetValue(0.75 * Convert.ToDouble(planetNeedsDynamics.Attribute("atmoPressure").Value) - 0.1);
                                    planetNeedsDynamics.Attribute("hydroCoverage").SetValue(Math.Clamp(Convert.ToDouble(planetNeedsDynamics.Attribute("hydroCoverage").Value) - 25, 0, 100));
                                    dayTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value) * 1.12;
                                    nightTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value) * 0.8;
                                    break;
                                case "Dense":
                                    planetNeedsDynamics.Attribute("hydroCoverage").SetValue(Math.Clamp(Convert.ToDouble(planetNeedsDynamics.Attribute("hydroCoverage").Value) - 10, 0, 100));
                                    dayTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value) * 1.09;
                                    nightTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value) * 0.88;
                                    break;
                                case "Very Dense":
                                    dayTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value) * 1.05;
                                    nightTemp = Convert.ToDouble(planetNeedsDynamics.Attribute("avgTemp").Value) * 0.95;
                                    break;
                                case "Superdense":
                                    //Atmosphere protects planet
                                    break;
                                default:
                                    throw new Exception("atmoType did not fit any category for tidal-lock penalties!");
                            }

                            string dayClimate;
                            string nightClimate;

                            if (dayTemp < 244)
                            {
                                dayClimate = "Frozen";
                            }
                            else if (dayTemp < 255)
                            {
                                dayClimate = "Very Cold";
                            }
                            else if (dayTemp < 266)
                            {
                                dayClimate = "Cold";
                            }
                            else if (dayTemp < 278)
                            {
                                dayClimate = "Chilly";
                            }
                            else if (dayTemp < 289)
                            {
                                dayClimate = "Cool";
                            }
                            else if (dayTemp < 300)
                            {
                                dayClimate = "Normal";
                            }
                            else if (dayTemp < 311)
                            {
                                dayClimate = "Warm";
                            }
                            else if (dayTemp < 322)
                            {
                                dayClimate = "Tropical";
                            }
                            else if (dayTemp < 333)
                            {
                                dayClimate = "Hot";
                            }
                            else if (dayTemp < 344)
                            {
                                dayClimate = "Very Hot";
                            }
                            else
                            {
                                dayClimate = "Infernal";
                            }

                            if (nightTemp < 244)
                            {
                                nightClimate = "Frozen";
                            }
                            else if (nightTemp < 255)
                            {
                                nightClimate = "Very Cold";
                            }
                            else if (nightTemp < 266)
                            {
                                nightClimate = "Cold";
                            }
                            else if (nightTemp < 278)
                            {
                                nightClimate = "Chilly";
                            }
                            else if (nightTemp < 289)
                            {
                                nightClimate = "Cool";
                            }
                            else if (nightTemp < 300)
                            {
                                nightClimate = "Normal";
                            }
                            else if (nightTemp < 311)
                            {
                                nightClimate = "Warm";
                            }
                            else if (nightTemp < 322)
                            {
                                nightClimate = "Tropical";
                            }
                            else if (nightTemp < 333)
                            {
                                nightClimate = "Hot";
                            }
                            else if (nightTemp < 344)
                            {
                                nightClimate = "Very Hot";
                            }
                            else
                            {
                                nightClimate = "Infernal";
                            }

                            planetNeedsDynamics.Attribute("avgTemp").Remove();
                            planetNeedsDynamics.Attribute("climate").Remove();

                            planetNeedsDynamics.Add(
                                new XAttribute("dayTemp", dayTemp),
                                new XAttribute("dayClimate", dayClimate),
                                new XAttribute("nightTemp", nightTemp),
                                new XAttribute("nightClimate", nightClimate)
                                );
                        }
                    }
                }

                rand = random.NextDouble();
                if (!tideLockNeeded && rand < 0.25926)
                {
                    rotationalPeriod *= -1;
                }

                planetNeedsDynamics.Add(new XAttribute("rotationalPeriod", rotationalPeriod));
            }

            //Axial tilts for gas giants are also done here
            IEnumerable<XElement> giantsNeedDynamics = from el in system.Descendants("giant") select el;
            foreach (XElement giantNeedsDynamics in giantsNeedDynamics)
            {
                double mass;
                double density;
                double diameter;
                double temperature;

                rand = random.NextDouble();
                switch (giantNeedsDynamics.Attribute("class").Value)
                {
                    case "Small":
                        mass = 2.10087 * Math.Pow(35.5415, rand);
                        if (mass < 10)
                        {
                            mass = 10;
                        }
                        density = 0.159638 * Math.Pow(rand, -0.712283);
                        if (density > 0.45)
                        {
                            density = 0.45;
                        }
                        break;
                    case "Medium":
                        mass = 44.0526 * Math.Pow(10.5597, rand);
                        if (mass < 80)
                        {
                            mass = 80;
                        }
                        density = 0.190977 * Math.Pow(rand, 3) - 0.154345 * Math.Pow(rand, 2) + 0.778811 * rand + 0.166565;
                        break;
                    case "Large":
                        mass = 17223 * Math.Pow(rand, 3) - 23881.7 * Math.Pow(rand, 2) + 11588.2 * rand - 1113.47;
                        if (mass < 500)
                        {
                            mass = 500;
                        }
                        density = 6.90143 * Math.Pow(rand, 3) - 9.36889 * Math.Pow(rand, 2) + 4.2944 * rand - 0.298701;
                        if (density < 0.3)
                        {
                            density = 0.3;
                        }
                        break;
                    default:
                        throw new Exception("Gas giant size class has no size data!");
                }

                diameter = Math.Cbrt(mass / density);

                temperature = 278 * Math.Sqrt(Math.Sqrt(Convert.ToDouble(giantNeedsDynamics.Parent.Attribute("luminosity").Value))) / Math.Sqrt(Convert.ToDouble(giantNeedsDynamics.Attribute("orbitalRadius").Value));

                if (mass > 4100)
                {
                    temperature += 0.00000000012554 * Math.Pow(mass, 3) - 0.00000346464 * Math.Pow(mass, 2) + 0.0722229 * mass + 121.425;
                }

                giantNeedsDynamics.Add(new XAttribute("mass", mass),
                    new XAttribute("density", density),
                    new XAttribute("diameter", diameter),
                    new XAttribute("temp", temperature));

                double period = Math.Sqrt(Math.Pow(Convert.ToDouble(giantNeedsDynamics.Attribute("orbitalRadius").Value), 3) / (Convert.ToDouble(giantNeedsDynamics.Parent.Attribute("mass").Value) + Convert.ToDouble(giantNeedsDynamics.Attribute("mass").Value)));
                double eccentricity;

                rand = random.NextDouble();
                if (giantNeedsDynamics.Parent.Elements().Contains(new XElement("eccentric")) && giantNeedsDynamics.Attributes().Contains(new XAttribute("eccentric", "true")))
                {
                    eccentricity = 0.387695 * Math.Pow(rand, 2) + 0.43609 * rand + 0.073739;
                    giantNeedsDynamics.Attribute("eccentric").Remove();
                }
                else if (giantNeedsDynamics.Attributes().Contains(new XAttribute("epistellar", "true")))
                {
                    eccentricity = 0.000191044 * Math.Pow(893.236, rand);
                    giantNeedsDynamics.Attribute("epistellar").Remove();
                }
                else if (giantNeedsDynamics.Parent.Elements().Contains(new XElement("conventional")))
                {
                    eccentricity = 0.000191044 * Math.Pow(893.236, rand);
                }
                else
                {
                    eccentricity = 0.00411578 * Math.Pow(172.079, rand);
                }

                giantNeedsDynamics.Add(new XAttribute("orbitalPeriod", period));
                giantNeedsDynamics.Add(new XAttribute("eccentricity", eccentricity));

                double tidalEffect = (0.46 * Convert.ToDouble(giantNeedsDynamics.Parent.Attribute("mass").Value) * Convert.ToDouble(giantNeedsDynamics.Attribute("diameter").Value)) / Math.Pow(Convert.ToDouble(giantNeedsDynamics.Attribute("orbitalRadius").Value), 3);

                if (giantNeedsDynamics.Element("rings") != null)
                {
                    IEnumerable<XElement> shepardsNeedDynamics = giantNeedsDynamics.Descendants("shepard");
                    foreach (XElement shepardNeedsDynamics in shepardsNeedDynamics)
                    {
                        rand = 5 + 5 * random.NextDouble();
                        shepardNeedsDynamics.Add(new XAttribute("orbitalRadius", rand * Convert.ToDouble(giantNeedsDynamics.Attribute("diameter").Value) / 4));
                    }

                    shepardsNeedDynamics = from el in giantNeedsDynamics.Descendants("shepard") orderby Convert.ToDouble(el.Attribute("orbitalRadius").Value) select el;
                    XElement shepardVault = new XElement("root");
                    foreach (XElement shepardNeedsDynamics in shepardsNeedDynamics)
                    {
                        shepardVault.Add(shepardNeedsDynamics);
                    }
                    giantNeedsDynamics.Element("rings").RemoveNodes();
                    foreach (XElement vaultShepard in shepardVault.Elements())
                    {
                        giantNeedsDynamics.Element("rings").Add(vaultShepard);
                    }
                }
                if (giantNeedsDynamics.Element("moon") != null)
                {
                    IEnumerable<XElement> moonsNeedDynamics = giantNeedsDynamics.Descendants("moon");
                    foreach (XElement moonNeedsDynamics in moonsNeedDynamics)
                    {
                        rand = 3.5 + (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());
                        if (rand > 15)
                        {
                            rand += (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());
                        }

                        if (moonNeedsDynamics.Attribute("type").Value == "Tiny (Sulfur)")
                        {
                            rand = 5 + 0.5 * random.NextDouble();
                        }

                        double orbitalRadius = rand * Convert.ToDouble(giantNeedsDynamics.Attribute("diameter").Value) / 2;
                        double orbitalPeriod = 0.0588 * Math.Sqrt(Math.Pow(orbitalRadius, 3) / (Convert.ToDouble(moonNeedsDynamics.Attribute("mass").Value) + Convert.ToDouble(moonNeedsDynamics.Parent.Attribute("mass").Value)));
                        tidalEffect += (17800000 * Convert.ToDouble(moonNeedsDynamics.Attribute("mass").Value) * Convert.ToDouble(giantNeedsDynamics.Attribute("diameter").Value)) / Math.Pow(orbitalRadius, 3);

                        moonNeedsDynamics.Add(new XAttribute("orbitalRadius", orbitalRadius));
                        moonNeedsDynamics.Add(new XAttribute("orbitalPeriod", orbitalPeriod));

                        double moonTidalEffect = (0.46 * Convert.ToDouble(moonNeedsDynamics.Parent.Attribute("mass").Value) * Convert.ToDouble(moonNeedsDynamics.Attribute("diameter").Value)) / Math.Pow(Convert.ToDouble(moonNeedsDynamics.Attribute("orbitalRadius").Value), 3);
                        int moonTidalScore = (int)Math.Round(moonTidalEffect);

                        double moonRotationalPeriod = 0;
                        bool moonTideLockNeeded = false;

                        if (moonTidalScore < 50)
                        {
                            rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());
                            moonRotationalPeriod = rand;

                            moonRotationalPeriod += moonTidalScore;

                            if (moonNeedsDynamics.Attribute("type").Value.StartsWith("Standard"))
                            {
                                moonRotationalPeriod += 10;
                            }
                            else if (moonNeedsDynamics.Attribute("type").Value.StartsWith("Small"))
                            {
                                moonRotationalPeriod += 14;
                            }
                            else
                            {
                                moonRotationalPeriod += 18;
                            }

                            if (rand > 36 || moonRotationalPeriod > 36)
                            {
                                rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());

                                if (rand < 6)
                                {
                                    //It's fine, folks.
                                }
                                else if (rand < 7)
                                {
                                    moonRotationalPeriod = (1 + 5 * random.NextDouble()) * 2 * 24;
                                }
                                else if (rand < 8)
                                {
                                    moonRotationalPeriod = (1 + 5 * random.NextDouble()) * 5 * 24;
                                }
                                else if (rand < 9)
                                {
                                    moonRotationalPeriod = (1 + 5 * random.NextDouble()) * 10 * 24;
                                }
                                else if (rand < 10)
                                {
                                    moonRotationalPeriod = (1 + 5 * random.NextDouble()) * 20 * 24;
                                }
                                else if (rand < 11)
                                {
                                    moonRotationalPeriod = (1 + 5 * random.NextDouble()) * 50 * 24;
                                }
                                else
                                {
                                    moonRotationalPeriod = (1 + 5 * random.NextDouble()) * 100 * 24;
                                }

                                if (orbitalPeriod * 24 < moonRotationalPeriod)
                                {
                                    moonTideLockNeeded = true;
                                }
                            }
                        }
                        else
                        {
                            moonTideLockNeeded = true;
                        }

                        if (moonTideLockNeeded)
                        {
                            moonRotationalPeriod = period * 24;
                            moonNeedsDynamics.Add(new XAttribute("tidalLock", "planet"));
                        }

                        rand = random.NextDouble();
                        if (!moonTideLockNeeded && rand < 0.01852)
                        {
                            moonRotationalPeriod *= -1;
                        }

                        moonNeedsDynamics.Add(new XAttribute("rotationalPeriod", moonRotationalPeriod));
                    }

                    moonsNeedDynamics = from el in giantNeedsDynamics.Descendants("moon") orderby Convert.ToDouble(el.Attribute("orbitalRadius").Value) select el;
                    XElement moonVault = new XElement("root");
                    foreach (XElement moonNeedsDynamics in moonsNeedDynamics)
                    {
                        moonVault.Add(moonNeedsDynamics);
                    }
                    int numMoons = moonsNeedDynamics.Count<XElement>();
                    for (int i = 0; i < numMoons; i++)
                    {
                        foreach (XElement moonToDelete in moonsNeedDynamics)
                        {
                            moonToDelete.Remove();
                        }
                    }
                    foreach (XElement vaultMoon in moonVault.Elements())
                    {
                        if (giantNeedsDynamics.Element("moonlets") == null)
                        {
                            giantNeedsDynamics.Add(vaultMoon);
                        }
                        else
                        {
                            giantNeedsDynamics.Element("moonlets").AddBeforeSelf(vaultMoon);
                        }
                    }
                }
                if (giantNeedsDynamics.Element("moonlets") != null)
                {
                    IEnumerable<XElement> moonletsNeedDynamics = giantNeedsDynamics.Descendants("moonlet");
                    foreach (XElement moonletNeedsDynamics in moonletsNeedDynamics)
                    {
                        rand = 20 * Math.Pow(65, random.NextDouble());
                        moonletNeedsDynamics.Add(new XAttribute("orbitalRadius", rand * Convert.ToDouble(giantNeedsDynamics.Attribute("diameter").Value)));
                    }

                    moonletsNeedDynamics = from el in giantNeedsDynamics.Descendants("moonlet") orderby Convert.ToDouble(el.Attribute("orbitalRadius").Value) select el;
                    XElement moonletVault = new XElement("root");
                    foreach (XElement moonletNeedsDynamics in moonletsNeedDynamics)
                    {
                        moonletVault.Add(moonletNeedsDynamics);
                    }
                    giantNeedsDynamics.Element("moonlets").Remove();
                    foreach (XElement vaultMoonlet in moonletVault.Elements())
                    {
                        giantNeedsDynamics.Add(vaultMoonlet);
                    }
                }

                tidalEffect *= age / Convert.ToDouble(giantNeedsDynamics.Attribute("mass").Value);
                int tidalScore = (int)Math.Round(tidalEffect);

                double rotationalPeriod = 0;
                bool tideLockNeeded = false;

                if (tidalScore < 50)
                {
                    rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());
                    rotationalPeriod = rand;

                    rotationalPeriod += tidalScore;

                    if (giantNeedsDynamics.Attribute("class").Value == "Small")
                    {
                        rotationalPeriod += 6;
                    }

                    if (rand > 36 || rotationalPeriod > 36)
                    {
                        rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());

                        if (rand < 6)
                        {
                            //It's fine, folks.
                        }
                        else if (rand < 7)
                        {
                            rotationalPeriod = (1 + 5 * random.NextDouble()) * 2 * 24;
                        }
                        else if (rand < 8)
                        {
                            rotationalPeriod = (1 + 5 * random.NextDouble()) * 5 * 24;
                        }
                        else if (rand < 9)
                        {
                            rotationalPeriod = (1 + 5 * random.NextDouble()) * 10 * 24;
                        }
                        else if (rand < 10)
                        {
                            rotationalPeriod = (1 + 5 * random.NextDouble()) * 20 * 24;
                        }
                        else if (rand < 11)
                        {
                            rotationalPeriod = (1 + 5 * random.NextDouble()) * 50 * 24;
                        }
                        else
                        {
                            rotationalPeriod = (1 + 5 * random.NextDouble()) * 100 * 24;
                        }

                        double criticalRotationalPeriod;
                        if (giantNeedsDynamics.Element("moon") != null)
                        {
                            criticalRotationalPeriod = Convert.ToDouble(giantNeedsDynamics.Element("moon").Attribute("orbitalPeriod").Value) * 24;
                        }
                        else
                        {
                            criticalRotationalPeriod = period * 8760;
                        }

                        if (criticalRotationalPeriod < rotationalPeriod)
                        {
                            tideLockNeeded = true;
                        }
                    }
                }
                else
                {
                    tideLockNeeded = true;
                }

                if (tideLockNeeded)
                {
                    if (giantNeedsDynamics.Element("moon") != null)
                    {
                        rotationalPeriod = Convert.ToDouble(giantNeedsDynamics.Element("moon").Attribute("orbitalPeriod").Value) * 24;
                        giantNeedsDynamics.Add(new XAttribute("tidalLock", "moon"));
                    }
                    else
                    {
                        rotationalPeriod = period * 8760;
                        giantNeedsDynamics.Add(new XAttribute("tidalLock", "star"));

                        if (Convert.ToDouble(giantNeedsDynamics.Attribute("eccentricity").Value) >= 0.1)
                        {
                            rand = random.NextDouble();
                            if (rand < 0.375)
                            {
                                giantNeedsDynamics.Attribute("tidalLock").SetValue("resonant");
                                rotationalPeriod *= 2;
                            }
                        }
                    }
                }

                rand = random.NextDouble();
                if (!tideLockNeeded && rand < 0.25926)
                {
                    rotationalPeriod *= -1;
                }

                giantNeedsDynamics.Add(new XAttribute("rotationalPeriod", rotationalPeriod));

                double axialTilt;

                rand = random.NextDouble();
                if (rand < 0.9877)
                {
                    axialTilt = 38.4381 * Math.Pow(rand, 1.79794);
                }
                else
                {
                    axialTilt = 79.1665 * Math.Pow(rand, 39.2799);
                }

                giantNeedsDynamics.Add(new XAttribute("axialTilt", axialTilt));
            }

            IEnumerable<XElement> worlds = from el in system.Descendants() where el.Name == "planet" || el.Name == "moon" select el;
            foreach (XElement world in worlds)
            {
                double axialTilt;
                VolcanismLevel volcanismLevel;
                TectonicLevel tectonicLevel;

                rand = random.NextDouble();
                if (rand < 0.9877)
                {
                    axialTilt = 38.4381 * Math.Pow(rand, 1.79794);
                }
                else
                {
                    axialTilt = 79.1665 * Math.Pow(rand, 39.2799);
                }

                int volcanismScore = random.Next(1, 7) + random.Next(1, 7) + random.Next(1, 7);
                volcanismScore += (int)Math.Round(40 * Convert.ToDouble(world.Attribute("surfaceGravity").Value) / age);

                if (world.Name.ToString() == "planet" && world.Element("moon") != null)
                {
                    volcanismScore += 5;

                    if (world.Elements("moon").Count<XElement>() > 1)
                    {
                        volcanismScore += 5;
                    }
                }

                if (world.Attribute("type").Value == "Tiny (Sulfur)")
                {
                    volcanismScore += 60;
                }
                else if (world.Parent.Name.ToString() == "giant")
                {
                    volcanismScore += 5;
                }

                if (volcanismScore < 17)
                {
                    volcanismLevel = VolcanismLevel.None;
                }
                else if (volcanismScore < 21)
                {
                    volcanismLevel = VolcanismLevel.Light;
                }
                else if (volcanismScore < 27)
                {
                    volcanismLevel = VolcanismLevel.Moderate;
                }
                else if (volcanismScore < 71)
                {
                    volcanismLevel = VolcanismLevel.Heavy;

                    if (world.Attribute("type").Value == "Standard (Garden)" || world.Attribute("type").Value == "Large (Garden)")
                    {
                        rand = random.NextDouble();

                        if (rand < 0.25926)
                        {
                            if (world.Attribute("atmoMarginals").Value == "None")
                            {
                                world.Attribute("atmoMarginals").SetValue("Sulfur Compounds");
                            }
                            else if (world.Attribute("atmoMarginals").Value == "Sulfur Compounds")
                            {
                                world.Attribute("atmoMarginals").SetValue(world.Attribute("atmoMarginals").Value + ", Pollutants");
                            }
                            else
                            {
                                world.Attribute("atmoMarginals").SetValue(world.Attribute("atmoMarginals").Value + ", Sulfur Compounds");
                            }
                        }
                    }
                }
                else
                {
                    volcanismLevel = VolcanismLevel.Extreme;

                    if (world.Attribute("type").Value == "Standard (Garden)" || world.Attribute("type").Value == "Large (Garden)")
                    {
                        rand = random.NextDouble();

                        if (rand < 0.9074)
                        {
                            if (world.Attribute("atmoMarginals").Value == "None")
                            {
                                world.Attribute("atmoMarginals").SetValue("Sulfur Compounds");
                            }
                            else if (world.Attribute("atmoMarginals").Value == "Sulfur Compounds")
                            {
                                world.Attribute("atmoMarginals").SetValue(world.Attribute("atmoMarginals").Value + ", Pollutants");
                            }
                            else
                            {
                                world.Attribute("atmoMarginals").SetValue(world.Attribute("atmoMarginals").Value + ", Sulfur Compounds");
                            }
                        }
                    }
                }

                if (world.Attribute("type").Value.StartsWith("Tiny") || world.Attribute("type").Value.StartsWith("Small"))
                {
                    tectonicLevel = TectonicLevel.None;
                }
                else
                {
                    int tectonicPoints;

                    tectonicPoints = random.Next(1, 7) + random.Next(1, 7) + random.Next(1, 7);
                    tectonicPoints += 4 * ((int)volcanismLevel - 3);

                    if (Convert.ToDouble(world.Attribute("hydroCoverage").Value) < 50)
                    {
                        tectonicPoints -= 5;
                    }

                    if (world.Name.ToString() == "planet" && world.Element("moon") != null)
                    {
                        tectonicPoints += 2;

                        if (world.Elements("moon").Count<XElement>() > 1)
                        {
                            tectonicPoints += 2;
                        }
                    }

                    if (tectonicPoints < 7)
                    {
                        tectonicLevel = TectonicLevel.None;
                    }
                    else if (tectonicPoints < 11)
                    {
                        tectonicLevel = TectonicLevel.Light;
                    }
                    else if (tectonicPoints < 15)
                    {
                        tectonicLevel = TectonicLevel.Moderate;
                    }
                    else if (tectonicPoints < 19)
                    {
                        tectonicLevel = TectonicLevel.Heavy;
                    }
                    else
                    {
                        tectonicLevel = TectonicLevel.Extreme;
                    }
                }

                world.Add(new XAttribute("volcanism", volcanismLevel.ToString()));
                world.Add(new XAttribute("tectonics", tectonicLevel.ToString()));
            }

            IEnumerable<XElement> barycenters = from el in system.Descendants("barycenter") select el;
            foreach (XElement barycenter in barycenters)
            {
                double period = Math.Sqrt(Math.Pow(Convert.ToDouble(barycenter.Attribute("separation").Value), 3) / Convert.ToDouble(barycenter.Attribute("mass").Value));
                barycenter.Add(new XAttribute("period", period));

                barycenter.Attribute("luminosity").Remove();
                barycenter.Attribute("mass").Remove();

                if (barycenter.Attribute("snowLine") != null)
                {
                    barycenter.Attribute("innerLimit").Remove();
                    barycenter.Attribute("outerLimit").Remove();
                    barycenter.Attribute("snowLine").Remove();
                }
            }

            IEnumerable<XElement> stars = from el in system.Descendants("star") select el;
            foreach (XElement star in stars)
            {
                star.Attribute("innerLimit").Remove();
                star.Attribute("outerLimit").Remove();
                star.Attribute("snowLine").Remove();

                if (star.Attribute("forbidInt") != null)
                {
                    star.Attribute("forbidInt").Remove();
                }
            }

            //NAME EVERYTHING!!!
            name = randomName();
            system.RemoveAttributes();
            system.Add(new XAttribute("name", name));
            system.Add(new XAttribute("age", age));

            int j = 0;
            foreach (XElement star in stars)
            {
                switch (j)
                {
                    case 0:
                        star.Add(new XAttribute("name", name + " A"));
                        break;
                    case 1:
                        star.Add(new XAttribute("name", name + " B"));
                        break;
                    case 2:
                        star.Add(new XAttribute("name", name + " C"));
                        break;
					case 3:
						star.Add(new XAttribute("name", name + " D"));
						break;
                    default:
                        throw new Exception("Too many stars and not enough letters!");
                }

                IEnumerable<XAttribute> attributesToMove = from at in star.Attributes() where at.Name.ToString() != "name" select at;
                XElement attributeVault = new XElement("root");
                attributeVault.Add(attributesToMove);
                attributesToMove.Remove();
                star.Add(from at in attributeVault.Attributes() select at);

                j++;

				int beltIndex = 1;
				var thisStarBelts = from el in star.Descendants() where el.Name.ToString() == "belt" select el;
				foreach (var thisStarBelt in thisStarBelts)
				{
					string beltName = thisStarBelt.Parent.Attribute("name").Value + " Belt";
					if (thisStarBelts.Count() > 1) beltName += " " + beltIndex;
					thisStarBelt.Add(new XAttribute("name", beltName));
					IEnumerable<XAttribute> beltAttributesToMove = from at in thisStarBelt.Attributes() where at.Name.ToString() != "name" select at;
                    XElement beltAttributeVault = new XElement("root");
                    beltAttributeVault.Add(beltAttributesToMove);
                    beltAttributesToMove.Remove();
                	thisStarBelt.Add(from at in beltAttributeVault.Attributes() select at);
					beltIndex++;
				}

                IEnumerable<XElement> planets = from el in star.Descendants() where el.Name.ToString() == "planet" || el.Name.ToString() == "giant" select el;
                int k = 1;
                foreach (XElement planet in planets)
                {
                    string planetIndex = ToRoman(k);
                    string planetName = planet.Parent.Attribute("name").Value + " " + planetIndex;
                    planet.Add(new XAttribute("name", planetName));

                    IEnumerable<XAttribute> planetAttributesToMove = from at in planet.Attributes() where at.Name.ToString() != "name" select at;
                    XElement planetAttributeVault = new XElement("root");
                    planetAttributeVault.Add(planetAttributesToMove);
                    planetAttributesToMove.Remove();
                    planet.Add(from at in planetAttributeVault.Attributes() select at);

                    k++;

                    IEnumerable<XElement> shepards = from el in planet.Descendants("shepard") select el;
                    int l = 97;
                    foreach (XElement moonlet in shepards)
                    {
                        char moonIndex = Convert.ToChar(l);
                        string moonName = planetName + moonIndex;
                        moonlet.Add(new XAttribute("name", moonName));

                        IEnumerable<XAttribute> moonAttributesToMove = from at in moonlet.Attributes() where at.Name.ToString() != "name" select at;
                        XElement moonAttributeVault = new XElement("root");
                        moonAttributeVault.Add(moonAttributesToMove);
                        moonAttributesToMove.Remove();
                        moonlet.Add(from at in moonAttributeVault.Attributes() select at);

                        l++;
                    }

                    IEnumerable<XElement> moons = from el in planet.Elements("moon") select el;
                    foreach (XElement moon in moons)
                    {
                        char moonIndex = Convert.ToChar(l);
                        string moonName = planetName + moonIndex;
                        moon.Add(new XAttribute("name", moonName));

                        IEnumerable<XAttribute> moonAttributesToMove = from at in moon.Attributes() where at.Name.ToString() != "name" select at;
                        XElement moonAttributeVault = new XElement("root");
                        moonAttributeVault.Add(moonAttributesToMove);
                        moonAttributesToMove.Remove();
                        moon.Add(from at in moonAttributeVault.Attributes() select at);

                        l++;
                    }

                    IEnumerable<XElement> moonlets = from el in planet.Descendants("moonlet") select el;
                    foreach (XElement moonlet in moonlets)
                    {
                        char moonIndex = Convert.ToChar(l);
                        string moonName = planetName + moonIndex;
                        moonlet.Add(new XAttribute("name", moonName));

                        IEnumerable<XAttribute> moonAttributesToMove = from at in moonlet.Attributes() where at.Name.ToString() != "name" select at;
                        XElement moonAttributeVault = new XElement("root");
                        moonAttributeVault.Add(moonAttributesToMove);
                        moonAttributesToMove.Remove();
                        moonlet.Add(from at in moonAttributeVault.Attributes() select at);

                        l++;
					}
                    int ringIndex = 1;
					var thisPlanetRings = from el in planet.Descendants() where el.Name.ToString() == "rings" select el;
					foreach (var thisPlanetRing in thisPlanetRings)
					{
						string ringName = thisPlanetRing.Parent.Attribute("name").Value + " Ring";
						if (thisPlanetRings.Count() > 1) ringName += " " + ringIndex;
						thisPlanetRing.Add(new XAttribute("name", ringName));
						IEnumerable<XAttribute> ringAttributesToMove = from at in thisPlanetRing.Attributes() where at.Name.ToString() != "name" select at;
               		    XElement ringAttributeVault = new XElement("root");
            	        ringAttributeVault.Add(ringAttributesToMove);
             	       	ringAttributesToMove.Remove();
           		     	thisPlanetRing.Add(from at in ringAttributeVault.Attributes() select at);
						ringIndex++;
					}
                }
            }
            foreach (XElement barycenter in barycenters)
            {
                string barycenterID = "";

                IEnumerable<XElement> barycenterStars = from el in barycenter.Descendants("star") select el;
                foreach (XElement star in barycenterStars)
                {
                    barycenterID += star.Attribute("name").Value.Substring(star.Attribute("name").Value.Length - 1);
                }

                IEnumerable<XElement> planets = from el in barycenter.Elements() where el.Name.ToString() == "planet" || el.Name.ToString() == "giant" select el;
                int k = 1;
                foreach (XElement planet in planets)
                {
                    string planetIndex = ToRoman(k);
                    string planetName = name + " " + barycenterID + " " + planetIndex;
                    planet.Add(new XAttribute("name", planetName));

                    IEnumerable<XAttribute> planetAttributesToMove = from at in planet.Attributes() where at.Name.ToString() != "name" select at;
                    XElement planetAttributeVault = new XElement("root");
                    planetAttributeVault.Add(planetAttributesToMove);
                    planetAttributesToMove.Remove();
                    planet.Add(from at in planetAttributeVault.Attributes() select at);

                    k++;

                    IEnumerable<XElement> shepards = from el in planet.Descendants("shepard") select el;
                    int l = 97;
                    foreach (XElement moonlet in shepards)
                    {
                        char moonIndex = Convert.ToChar(l);
                        string moonName = planetName + moonIndex;
                        moonlet.Add(new XAttribute("name", moonName));

                        IEnumerable<XAttribute> moonAttributesToMove = from at in moonlet.Attributes() where at.Name.ToString() != "name" select at;
                        XElement moonAttributeVault = new XElement("root");
                        moonAttributeVault.Add(moonAttributesToMove);
                        moonAttributesToMove.Remove();
                        moonlet.Add(from at in moonAttributeVault.Attributes() select at);

                        l++;
                    }

                    IEnumerable<XElement> moons = from el in planet.Elements("moon") select el;
                    foreach (XElement moon in moons)
                    {
                        char moonIndex = Convert.ToChar(l);
                        string moonName = planetName + moonIndex;
                        moon.Add(new XAttribute("name", moonName));

                        IEnumerable<XAttribute> moonAttributesToMove = from at in moon.Attributes() where at.Name.ToString() != "name" select at;
                        XElement moonAttributeVault = new XElement("root");
                        moonAttributeVault.Add(moonAttributesToMove);
                        moonAttributesToMove.Remove();
                        moon.Add(from at in moonAttributeVault.Attributes() select at);

                        l++;
                    }

                    IEnumerable<XElement> moonlets = from el in planet.Descendants("moonlet") select el;
                    foreach (XElement moonlet in moonlets)
                    {
                        char moonIndex = Convert.ToChar(l);
                        string moonName = planetName + moonIndex;
                        moonlet.Add(new XAttribute("name", moonName));

                        IEnumerable<XAttribute> moonAttributesToMove = from at in moonlet.Attributes() where at.Name.ToString() != "name" select at;
                        XElement moonAttributeVault = new XElement("root");
                        moonAttributeVault.Add(moonAttributesToMove);
                        moonAttributesToMove.Remove();
                        moonlet.Add(from at in moonAttributeVault.Attributes() select at);

                        l++;
                    }
                }
            }

            //Calculate resource scores
            foreach (XElement world in worlds)
            {
                ResourceLevel resourceLevel;

                VolcanismLevel volcanismLevel = (VolcanismLevel)Enum.Parse(typeof(VolcanismLevel), world.Attribute("volcanism").Value);

                int randInt = random.Next(1, 7) + random.Next(1, 7) + random.Next(1, 7);
                randInt += (int)volcanismLevel - 3;

                if (randInt <= 2)
                {
                    resourceLevel = ResourceLevel.Scant;
                }
                else if (randInt <= 4)
                {
                    resourceLevel = ResourceLevel.VeryPoor;
                }
                else if (randInt <= 7)
                {
                    resourceLevel = ResourceLevel.Poor;
                }
                else if (randInt <= 13)
                {
                    resourceLevel = ResourceLevel.Average;
                }
                else if (randInt <= 16)
                {
                    resourceLevel = ResourceLevel.Abundant;
                }
                else if (randInt <= 18)
                {
                    resourceLevel = ResourceLevel.VeryAbundant;
                }
                else
                {
                    resourceLevel = ResourceLevel.Rich;
                }

                world.Add(new XAttribute("resources", resourceLevel.ToString()));
            }

            IEnumerable<XElement> belts = from el in system.Descendants() where el.Name.ToString() == "belt" || el.Name.ToString() == "rings" select el;
            foreach (XElement belt in belts)
            {
                ResourceLevel resourceLevel;

                int randInt = random.Next(1, 7) + random.Next(1, 7) + random.Next(1, 7);

                if (randInt <= 3)
                {
                    resourceLevel = ResourceLevel.Worthless;
                }
                else if (randInt <= 4)
                {
                    resourceLevel = ResourceLevel.VeryScant;
                }
                else if (randInt <= 5)
                {
                    resourceLevel = ResourceLevel.Scant;
                }
                else if (randInt <= 7)
                {
                    resourceLevel = ResourceLevel.VeryPoor;
                }
                else if (randInt <= 9)
                {
                    resourceLevel = ResourceLevel.Poor;
                }
                else if (randInt <= 11)
                {
                    resourceLevel = ResourceLevel.Average;
                }
                else if (randInt <= 13)
                {
                    resourceLevel = ResourceLevel.Abundant;
                }
                else if (randInt <= 15)
                {
                    resourceLevel = ResourceLevel.VeryAbundant;
                }
                else if (randInt <= 16)
                {
                    resourceLevel = ResourceLevel.Rich;
                }
                else if (randInt <= 17)
                {
                    resourceLevel = ResourceLevel.VeryRich;
                }
                else
                {
                    resourceLevel = ResourceLevel.Motherlode;
                }

                belt.Add(new XAttribute("resources", resourceLevel.ToString()));
            }

            //Calculate affinity scores
            IEnumerable<XElement> bodies = belts.Concat<XElement>(worlds);
            foreach (XElement body in bodies)
            {
                int affinityScore = (int)(ResourceLevel)Enum.Parse(typeof(ResourceLevel), body.Attribute("resources").Value);

                //The habitability score of a belt will always be 0
                if (worlds.Contains(body))
                {
                    //Atmosphere and temperature requirements
                    if (body.Attribute("atmoType") != null)
                    {
                        if (body.Attribute("atmoType").Value != "Trace")
                        {
                            if (body.Attribute("atmoMarginals").Value.StartsWith("Suffocating"))
                            {
                                if (body.Attribute("atmoMarginals").Value.EndsWith("Corrosive"))
                                {
                                    affinityScore -= 2;
                                }
                                else if (body.Attribute("atmoMarginals").Value != "Suffocating")
                                {
                                    affinityScore -= 1;
                                }
                            }
                            else
                            {
                                if (body.Attribute("atmoType").Value == "Very Thin")
                                {
                                    affinityScore += 1;
                                }
                                else if (body.Attribute("atmoType").Value == "Thin")
                                {
                                    affinityScore += 2;
                                }
                                else if (body.Attribute("atmoType").Value == "Standard" || body.Attribute("atmoType").Value == "Dense")
                                {
                                    affinityScore += 3;
                                }
                                else
                                {
                                    affinityScore += 1;
                                }

                                if (body.Attribute("atmoMarginals").Value == "None")
                                {
                                    affinityScore += 1;
                                }

                                if (body.Attribute("tidalLock") != null)
                                {
                                    if (body.Attribute("tidalLock").Value == "star")
                                    {
                                        int dayAffinity = 0;
                                        int nightAffinity = 0;

                                        if (body.Attribute("dayClimate").Value == "Cold" || body.Attribute("dayClimate").Value == "Hot")
                                        {
                                            dayAffinity += 1;
                                        }
                                        else if (body.Attribute("dayClimate").Value == "Chilly" || body.Attribute("dayClimate").Value == "Cool" || body.Attribute("dayClimate").Value == "Normal" || body.Attribute("dayClimate").Value == "Warm" || body.Attribute("dayClimate").Value == "Tropical")
                                        {
                                            dayAffinity += 2;
                                        }
                                        if (body.Attribute("nightClimate").Value == "Cold" || body.Attribute("nightClimate").Value == "Hot")
                                        {
                                            nightAffinity += 1;
                                        }
                                        if (body.Attribute("nightClimate").Value == "Chilly" || body.Attribute("nightClimate").Value == "Cool" || body.Attribute("nightClimate").Value == "Normal" || body.Attribute("nightClimate").Value == "Warm" || body.Attribute("nightClimate").Value == "Tropical")
                                        {
                                            nightAffinity += 2;
                                        }

                                        if (dayAffinity > nightAffinity)
                                        {
                                            affinityScore += dayAffinity;
                                        }
                                        else
                                        {
                                            affinityScore += nightAffinity;
                                        }
                                    }
                                    else
                                    {
                                        if (body.Attribute("climate").Value == "Cold" || body.Attribute("climate").Value == "Hot")
                                        {
                                            affinityScore += 1;
                                        }
                                        else if (body.Attribute("climate").Value == "Chilly" || body.Attribute("climate").Value == "Cool" || body.Attribute("climate").Value == "Normal" || body.Attribute("climate").Value == "Warm" || body.Attribute("climate").Value == "Tropical")
                                        {
                                            affinityScore += 2;
                                        }
                                    }
                                }
                                else
                                {
                                    if (body.Attribute("climate").Value == "Cold" || body.Attribute("climate").Value == "Hot")
                                    {
                                        affinityScore += 1;
                                    }
                                    else if (body.Attribute("climate").Value == "Chilly" || body.Attribute("climate").Value == "Cool" || body.Attribute("climate").Value == "Normal" || body.Attribute("climate").Value == "Warm" || body.Attribute("climate").Value == "Tropical")
                                    {
                                        affinityScore += 2;
                                    }
                                }
                            }
                        }
                    }

                    //Water requirements
                    if (body.Attribute("type").Value == "Standard (Ice)" || body.Attribute("type").Value == "Large (Ice)" || body.Attribute("type").Value.EndsWith("(Ocean)") || body.Attribute("type").Value.EndsWith("(Garden)"))
                    {
                        if (Convert.ToDouble(body.Attribute("hydroCoverage").Value) > 0)
                        {
                            if (Convert.ToDouble(body.Attribute("hydroCoverage").Value) < 60)
                            {
                                affinityScore += 1;
                            }
                            else if (Convert.ToDouble(body.Attribute("hydroCoverage").Value) < 90)
                            {
                                affinityScore += 2;
                            }
                            else if (Convert.ToDouble(body.Attribute("hydroCoverage").Value) < 99)
                            {
                                affinityScore += 1;
                            }
                        }
                    }

                    affinityScore -= Math.Clamp(Math.Clamp((int)(VolcanismLevel)Enum.Parse(typeof(VolcanismLevel), body.Attribute("volcanism").Value) - 2, 0, 2) + Math.Clamp((int)(TectonicLevel)Enum.Parse(typeof(TectonicLevel), body.Attribute("tectonics").Value) - 2, 0, 2), 0, 2);
                }

                body.Add(new XAttribute("affinity", affinityScore));
            }

			bodies = from el in system.Descendants() where el.Name == "moon" || el.Name == "planet" || el.Name == "belt" || el.Name == "rings" select el;

            //Identify and most interesting body
            XElement bestBody = (from el in bodies orderby Convert.ToDouble(el.Attribute("affinity").Value) select el).Last();

            
            //Add habitations
            if (inhabited && Convert.ToDouble(bestBody.Attribute("affinity").Value) > 0)
            {
                // Do primary world
                bestBody.Add(new XAttribute("habType", "colony"));
                rand = random.NextDouble();
                int tL;
                if (rand < 0.0046) {
                    int roll = random.Next(1, 7) + random.Next(1, 7) + random.Next(1, 7) - 12;
                    tL = Math.Max(0, roll);
                }
                else if (rand < 0.0185) {
                    tL = 7;
                }
                else if (rand < 0.0463) {
                    tL = 8;
                }
                else if (rand < 0.0162) {
                    tL = 9;
                }
                else tL = 10;
                bestBody.Add(new XAttribute("tL", tL));
			}
            
			Console.WriteLine(system);
			Console.WriteLine("\n");

			/*foreach (var body in bodies)
			{
				if (body.Attribute("name") == null) continue;
				Console.WriteLine(body.Attribute("name").Value);
			}

            Console.WriteLine("\n");*/

            //Mini-tree the most interesting body
            while (true)
            {
                IEnumerable<XElement> worseBodies = from el in bestBody.Parent.Elements() where el != bestBody select el;
                worseBodies.Remove<XElement>();

                bestBody = bestBody.Parent;
                if (bestBody.Name.ToString() == "system")
                {
                    break;
                }
            }
            Console.WriteLine(bestBody);
        }

        private double CalculateSystemAge()
        {
            Random random = new Random();
            int randInt = random.Next(0, 15);

            double output = 0;

            switch (randInt)
            {
                case 0:
                case 1:
                case 2:
                    output = 0.1 + 0.5 * random.Next(0, 5) + 0.1 * random.Next(0, 5);
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                    output = 2 + 0.6 * random.Next(0, 5) + 0.1 * random.Next(0, 5);
                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                    output = 5.6 + 0.6 * random.Next(0, 5) + 0.1 * random.Next(0, 5);
                    break;
                case 11:
                case 12:
                case 13:
                    output = 8 + 0.6 * random.Next(0, 5) + 0.1 * random.Next(0, 5);
                    break;
                default:
                    break;
            }

            output = Math.Round(output, 2);

            return output;
        }

        private void doBarycenters()
        {
            Random random = new Random();
            double rand = random.NextDouble();

            for (int i = 0; i < numStars - 1; i++)
            {
                XElement barycenter = new XElement("barycenter");

                IEnumerable<XElement> stars = from el in system.Elements() select el;
                int j = 0;
                foreach (XElement star in stars)
                {
                    barycenter.Add(star);

                    if(!barycenter.HasAttributes)
                    {
                        barycenter.Add(new XAttribute("mass", star.Attribute("mass").Value),
                        new XAttribute("luminosity", star.Attribute("luminosity").Value)
                        );
                    }
                    else
                    {
                        barycenter.Attribute("mass").Value = (Convert.ToDouble(barycenter.Attribute("mass").Value) + Convert.ToDouble(star.Attribute("mass").Value)).ToString();
                        barycenter.Attribute("luminosity").Value = (Convert.ToDouble(barycenter.Attribute("luminosity").Value) + Convert.ToDouble(star.Attribute("luminosity").Value)).ToString();
                    }

                    star.RemoveAll();

                    j++;
                    if (j > 1)
                    {
                        break;
                    }
                }

                double neededSeparation = 0;
                stars = from el in barycenter.Elements() select el;
                foreach (XElement star in stars)
                {
                    if (star.Name == "star")
                    {
                        neededSeparation += Convert.ToDouble(star.Attribute("radius").Value);
                    }
                    else
                    {
                        neededSeparation += Convert.ToDouble(star.Attribute("separation").Value);
                    }
                }

                rand = random.NextDouble();
                double separation;
                double eccentricity;
                while (true)
                {
                    if (rand < 0.0926)
                    {
                        rand = 5 * (random.NextDouble() + 1) + 5 * (random.NextDouble() + 1);
                        separation = rand * 0.05;

                        rand = random.NextDouble();
                        eccentricity = 2.50415 * Math.Pow(rand, 3) - 4.84641 * Math.Pow(rand, 2) + 3.75636 * rand - 0.865119;

                        if (eccentricity < 0)
                        {
                            eccentricity = 0;
                        }
                    }
                    else if (rand < 0.375)
                    {
                        rand = 5 * (random.NextDouble() + 1) + 5 * (random.NextDouble() + 1);
                        separation = rand * 0.5;

                        rand = random.NextDouble();
                        eccentricity = 2.19416 * Math.Pow(rand, 3) - 3.85711 * Math.Pow(rand, 2) + 2.64079 * rand - 0.348435;

                        if (eccentricity < 0)
                        {
                            eccentricity = 0;
                        }
                    }
                    else if (rand < 0.625)
                    {
                        rand = 5 * (random.NextDouble() + 1) + 5 * (random.NextDouble() + 1);
                        separation = rand * 2;

                        rand = random.NextDouble();
                        eccentricity = 3.25444 * Math.Pow(rand, 3) - 5.19289 * Math.Pow(rand, 2) + 2.79928 * rand - 0.122703;

                        if (eccentricity < 0)
                        {
                            eccentricity = 0;
                        }
                    }
                    else if (rand < 0.9074)
                    {
                        rand = 5 * (random.NextDouble() + 1) + 5 * (random.NextDouble() + 1);
                        separation = rand * 10;

                        rand = random.NextDouble();
                        eccentricity = 3.98629 * Math.Pow(rand, 3) - 6.01226 * Math.Pow(rand, 2) + 2.88642 * rand + 0.0399809;
                    }
                    else
                    {
                        rand = 5 * (random.NextDouble() + 1) + 5 * (random.NextDouble() + 1);
                        separation = rand * 50;

                        rand = random.NextDouble();
                        eccentricity = 3.98629 * Math.Pow(rand, 3) - 6.01226 * Math.Pow(rand, 2) + 2.88642 * rand + 0.0399809;
                    }

                    if ((1 - eccentricity) * separation > neededSeparation)
                    {
                        barycenter.Add(new XAttribute("separation", separation));
                        barycenter.Add(new XAttribute("eccentricity", eccentricity));
                        break;
                    }
                }

                while (true)
                {
                    IEnumerable<XElement> deadStars = from el in system.Elements() where !el.HasAttributes select el;
                    foreach (XElement deadStar in deadStars)
                    {
                        deadStar.Remove();
                    }

                    if (deadStars.Count() == 0)
                    {
                        break;
                    }
                }

                system.Add(barycenter);
            }
        }

        private void doPlanetZones()
        {
            IEnumerable<XElement> stars = from el in system.Descendants("star") select el;
            foreach (XElement star in stars)
            {
                double starMass = Convert.ToDouble(star.Attribute("mass").Value);
                double starLuminosity = Convert.ToDouble(star.Attribute("luminosity").Value);

                double innerLimit = 0.1 * starMass;
                if (0.01 * Math.Sqrt(starLuminosity) > innerLimit)
                {
                    innerLimit = 0.01 * Math.Sqrt(starLuminosity);
                }

                double outerLimit = 40 * starMass;

                double snowLine = 4.85 * Math.Sqrt(starLuminosity);

                star.Add(new XAttribute("innerLimit", innerLimit),
                    new XAttribute("outerLimit", outerLimit),
                    new XAttribute("snowLine", snowLine));

                if (numStars > 1)
                {
                    double forbiddenInnerLimit = 0;
                    double forbiddenOuterLimit = 0;

                    IEnumerable<XElement> starBarycenter = from el in system.Descendants("star") where el.Attribute("mass").Value == starMass.ToString() select el.Parent;
                    foreach (XElement barycenter in starBarycenter)
                    {
                        double separation = Convert.ToDouble(barycenter.Attribute("separation").Value);
                        double eccentricity = Convert.ToDouble(barycenter.Attribute("eccentricity").Value);

                        forbiddenInnerLimit = ((1 - eccentricity) * separation) / 3;
                        forbiddenOuterLimit = ((1 + eccentricity) * separation) * 3;
                    }

                    if (forbiddenInnerLimit < innerLimit)
                    {
                        if (forbiddenOuterLimit < innerLimit)
                        {
                            //forbidden zone inside legal zone - planets will be of the binary
                            star.Attribute("innerLimit").Remove();
                            star.Attribute("outerLimit").Remove();
                            star.Attribute("snowLine").Remove();
                        }
                        else if (forbiddenOuterLimit < outerLimit)
                        {
                            //forbidden zone pushes legal zone outwards - planets will be of the binary.
                            star.Attribute("innerLimit").Remove();
                            star.Attribute("outerLimit").Remove();
                            star.Attribute("snowLine").Remove();
                            star.Add(new XAttribute("forbidInt", 1));
                            if (star.Parent.Attribute("snowLine") == null)
                            {
                                double sharedLuminosity = Convert.ToDouble(star.Parent.Attribute("luminosity").Value);
                                double sharedMass = Convert.ToDouble(star.Parent.Attribute("mass").Value);

                                star.Parent.Add(new XAttribute("innerLimit", forbiddenOuterLimit),
                                    new XAttribute("outerLimit", 40 * sharedMass),
                                    new XAttribute("snowLine", 4.85 * Math.Sqrt(sharedLuminosity)));
                            }
                        }
                        else
                        {
                            //all of legal zone is forbidden
                            star.Attribute("innerLimit").Remove();
                            star.Attribute("outerLimit").Remove();
                            star.Attribute("snowLine").Remove();
                        }
                    }
                    else if (forbiddenInnerLimit < outerLimit)
                    {
                        if (forbiddenOuterLimit < outerLimit)
                        {
                            //forbidden zone entirely contained, splits planetary system in two
                            star.Attribute("outerLimit").Value = forbiddenInnerLimit.ToString();
                            star.Add(new XAttribute("forbidInt", 2));
                            if (star.Parent.Attribute("snowLine") == null)
                            {
                                double sharedLuminosity = Convert.ToDouble(star.Parent.Attribute("luminosity").Value);
                                double sharedMass = Convert.ToDouble(star.Parent.Attribute("mass").Value);


                                star.Parent.Add(new XAttribute("innerLimit", forbiddenOuterLimit),
                                    new XAttribute("outerLimit", 40 * sharedMass),
                                    new XAttribute("snowLine", 4.85 * Math.Sqrt(sharedLuminosity)));
                            }
                        }
                        else
                        {
                            //forbidden zone pushes legal zone inwards
                            star.Attribute("outerLimit").Value = forbiddenInnerLimit.ToString();
                            star.Add(new XAttribute("forbidInt", 2));
                        }
                    }
                    else
                    {
                        //forbidden zone outside legal zone
                    }
                }
            }
        }

        private string randomName()
        {
            string word;
            int cons;
            int vow;

            //counter
            int i;
            Random rnd = new Random();

            //set a new string array of consonants
            string[] consonant = new string[] { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };

            //set a new string array of vowels
            string[] vowel = new string[] { "a", "e", "i", "o", "u" };
            word = null;

            //randomizes word length
            Random random = new Random();
            int num = random.Next(6, 11);

            //set the counter "i" to 1
            i = 1;
            if (num % 2 == 0)
            {
                while (i <= num)
                {
                    if (num != 1)
                    {
                        cons = rnd.Next(0, 20);
                        word = word + consonant[cons];
                        i++;
                        if (cons == 12)
                        {
                            word = word + vowel[4];
                            i++;
                        }
                    }
                    vow = rnd.Next(0, 4);
                    word = word + vowel[vow];
                    i++;
                }
            }
            if (num % 2 != 0)
            {
                while (i <= num - 1)
                {
                    if (num != 1)
                    {
                        cons = rnd.Next(0, 20);
                        word = word + consonant[cons];
                        i++;
                        if (cons == 12)
                        {
                            word = word + vowel[4];
                            i++;
                        }
                    }
                    vow = rnd.Next(0, 4);
                    word = word + vowel[vow];
                    i++;
                }
                if (num != 1)
                {
                    cons = rnd.Next(0, 20);
                    word = word + consonant[cons];
                }
                else
                {
                    vow = rnd.Next(0, 4);
                    word = word + vowel[vow];
                }
                i = 1;
            }

            return char.ToUpper(word[0]) + word.Substring(1);
        }

        private string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        enum VolcanismLevel
        {
            None,
            Light,
            Moderate,
            Heavy,
            Extreme
        }

        enum TectonicLevel
        {
            None,
            Light,
            Moderate,
            Heavy,
            Extreme
        }

        enum ResourceLevel
        {
            Worthless,
            VeryScant,
            Scant,
            VeryPoor,
            Poor,
            Average,
            Abundant,
            VeryAbundant,
            Rich,
            VeryRich,
            Motherlode
        }
    }
}
