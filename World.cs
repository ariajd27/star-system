using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace StarSystem
{
    class World
    {
        string type;
        string sizeClass;
        double orbitalRadius;

        double blackbodyTemp;
        string worldType;
        double atmoMass;
        string atmoType;
        string atmoMarginals;
        double atmoPressure;
        double hydroCoverage;
        double avgTemp;
        string climate;
        double density;
        double surfaceGravity;
        double mass;
        double diameter;

        double parentLuminosity;
        bool sulfurPossible;
        bool ammoniaPossible;
        double age;

        public World(string type, string sizeClass, double orbitalRadius, double parentLuminosity, bool sulfurPossible, bool ammoniaPossible, double age)
        {
            this.type = type;
            this.sizeClass = sizeClass;
            this.orbitalRadius = orbitalRadius;
            this.parentLuminosity = parentLuminosity;
            this.sulfurPossible = sulfurPossible;
            this.ammoniaPossible = ammoniaPossible;
            this.age = age;
        }

        public XElement worldElement()
        {
            Random random = new Random();
            double rand;

            XElement outputElement = new XElement(type);
            if (type == "planet")
            {
                outputElement.Add(new XAttribute("orbitalRadius", orbitalRadius));
            }

            blackbodyTemp = 278 * Math.Sqrt(Math.Sqrt(parentLuminosity)) / Math.Sqrt(orbitalRadius);

            //Assign planet type
            switch(sizeClass)
            {
                case "tiny":
                    if (blackbodyTemp < 140)
                    {
                        if (sulfurPossible)
                        {
                            worldType = "Tiny (Sulfur)";
                        }
                        else
                        {
                            worldType = "Tiny (Ice)";
                        }
                    }
                    else
                    {
                        worldType = "Tiny (Rock)";
                    }
                    break;
                case "small":
                    if (blackbodyTemp < 80)
                    {
                        worldType = "Small (Hadean)";
                    }
                    else if (blackbodyTemp < 140)
                    {
                        worldType = "Small (Ice)";
                    }
                    else
                    {
                        worldType = "Small (Rock)";
                    }
                    break;
                case "standard":
                    if (blackbodyTemp < 80)
                    {
                        worldType = "Standard (Hadean)";
                    }
                    else if (blackbodyTemp < 240)
                    {
                        worldType = "Standard (Ice)";

                        if (blackbodyTemp > 150 && blackbodyTemp < 230 && ammoniaPossible)
                        {
                            worldType = "Standard (Ammonia)";
                        }
                    }
                    else if (blackbodyTemp < 320)
                    {
                        worldType = "Standard (Ocean)";

                        int rand1 = random.Next(1, 7) + random.Next(1, 7) + random.Next(1, 7);

                        int ageBonus = (int)Math.Round(age * 2);
                        if (ageBonus > 10)
                        {
                            ageBonus = 10;
                        }

                        rand1 += ageBonus;
                        if (rand1 >= 18)
                        {
                            worldType = "Standard (Garden)";
                        }
                    }
                    else if (blackbodyTemp < 500)
                    {
                        worldType = "Standard (Greenhouse)";
                    }
                    else
                    {
                        worldType = "Standard (Cthonian)";
                    }
                    break;
                case "large":
                    if (blackbodyTemp < 240)
                    {
                        worldType = "Large (Ice)";

                        if (blackbodyTemp > 150 && blackbodyTemp < 230 && ammoniaPossible)
                        {
                            worldType = "Standard (Ammonia)";
                        }
                    }
                    else if (blackbodyTemp < 320)
                    {
                        worldType = "Large (Ocean)";

                        int rand1 = random.Next(1, 7) + random.Next(1, 7) + random.Next(1, 7);

                        int ageBonus = (int)Math.Round(age * 2);
                        if (ageBonus > 10)
                        {
                            ageBonus = 10;
                        }

                        rand1 += ageBonus;
                        if (rand1 >= 18)
                        {
                            worldType = "Large (Garden)";
                        }
                    }
                    else if (blackbodyTemp < 500)
                    {
                        worldType = "Large (Greenhouse)";
                    }
                    else
                    {
                        worldType = "Large (Cthonian)";
                    }
                    break;
                default:
                    throw new Exception("World could not be assigned a world type!");
            }

            outputElement.Add(new XAttribute("type", worldType));

            //Initialize atmosphere
            if (sizeClass == "tiny" || worldType == "Small (Hadean)" || worldType == "Small (Rock)" || worldType == "Standard (Cthonian)" || worldType == "Standard (Hadean)" || worldType == "Large (Cthonian)")
            {
                atmoType = "Trace";
            }
            else
            {
                rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());
                atmoMass = rand / 10;

                //Do atmoMarginals
                switch (worldType)
                {
                    case "Small (Ice)":
                        rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());
                        if (rand < 15)
                        {
                            atmoMarginals = "Suffocating, Mildly Toxic";
                        }
                        else
                        {
                            atmoMarginals = "Suffocating, Highly Toxic";
                        }
                        break;
                    case "Standard (Greenhouse)":
                    case "Standard (Ammonia)":
                    case "Large (Ammonia)":
                    case "Large (Greenhouse)":
                        atmoMarginals = "Suffocating, Lethally Toxic, Corrosive";
                        break;
                    case "Standard (Ice)":
                    case "Standard (Ocean)":
                        rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());
                        if (rand < 12)
                        {
                            atmoMarginals = "Suffocating";
                        }
                        else
                        {
                            atmoMarginals = "Suffocating, Mildly Toxic";
                        }
                        break;
                    case "Standard (Garden)":
                    case "Large (Garden)":
                        rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());
                        if (rand < 11)
                        {
                            atmoMarginals = "None";
                        }
                        else
                        {
                            rand = (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble());
                            if (rand < 4)
                            {
                                atmoMarginals = "High Chlorine/Flourine";
                            }
                            else if (rand < 6)
                            {
                                atmoMarginals = "Sulfur Compounds";
                            }
                            else if (rand < 7)
                            {
                                atmoMarginals = "Nitrogen Compounds";
                            }
                            else if (rand < 9)
                            {
                                atmoMarginals = "Organic Toxins";
                            }
                            else if (rand < 11)
                            {
                                atmoMarginals = "Low Oxygen";
                            }
                            else if (rand < 13)
                            {
                                atmoMarginals = "Pollutants";
                            }
                            else if (rand < 14)
                            {
                                atmoMarginals = "High Carbon Dioxide";
                            }
                            else if (rand < 16)
                            {
                                atmoMarginals = "High Oxygen";
                            }
                            else
                            {
                                atmoMarginals = "Inert Gases";
                            }
                        }
                        break;
                    case "Large (Ice)":
                    case "Large (Ocean)":
                        atmoMarginals = "Suffocating, Highly Toxic";
                        break;
                    default:
                        throw new Exception("worldType has no corresponding atmospheric marginal description");
                }
            }

            //Do hydrographics
            switch (worldType)
            {
                case "Small (Ice)":
                    hydroCoverage = 30 + 5 * random.NextDouble();
                    break;
                case "Standard (Ammonia)":
                case "Large (Ammonia)":
                    hydroCoverage = 10 * ((1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble()));
                    if (hydroCoverage > 100)
                    {
                        hydroCoverage = 100;
                    }
                    break;
                case "Standard (Ice)":
                case "Large (Ice)":
                    hydroCoverage = 10 * ((1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble())) - 100;
                    if (hydroCoverage < 0)
                    {
                        hydroCoverage = 0;
                    }
                    break;
                case "Standard (Ocean)":
                case "Standard (Garden)":
                    hydroCoverage = 10 * (1 + 5 * random.NextDouble()) + 40;
                    if (hydroCoverage > 100)
                    {
                        hydroCoverage = 100;
                    }
                    break;
                case "Large (Ocean)":
                case "Large (Garden)":
                    hydroCoverage = 10 * (1 + 5 * random.NextDouble()) + 60;
                    if (hydroCoverage > 100)
                    {
                        hydroCoverage = 100;
                    }
                    break;
                case "Standard (Greenhouse)":
                case "Large (Greenhouse)":
                    hydroCoverage = 10 * ((1 + 5 * random.NextDouble()) + (1 + 5 * random.NextDouble())) - 70;
                    if (hydroCoverage < 0)
                    {
                        hydroCoverage = 0;
                    }
                    break;
                default:
                    hydroCoverage = 0;
                    break;
            }
            outputElement.Add(new XAttribute("hydroCoverage", hydroCoverage));

            //Determine blackbody correction and climate type
            {
                double absorptionFactor;
                double greenhouseFactor;

                if (worldType == "Tiny (Ice)")
                {
                    absorptionFactor = 0.86;
                    greenhouseFactor = 0;
                }
                else if (worldType == "Tiny (Rock)")
                {
                    absorptionFactor = 0.97;
                    greenhouseFactor = 0;
                }
                else if (worldType == "Tiny (Sulfur)")
                {
                    absorptionFactor = 0.77;
                    greenhouseFactor = 0;
                }
                else if (worldType == "Small (Hadean)")
                {
                    absorptionFactor = 0.67;
                    greenhouseFactor = 0;
                }
                else if (worldType == "Small (Ice)")
                {
                    absorptionFactor = 0.93;
                    greenhouseFactor = 0.1;
                }
                else if (worldType == "Small (Rock)")
                {
                    absorptionFactor = 0.96;
                    greenhouseFactor = 0;
                }
                else if (worldType == "Standard (Hadean)")
                {
                    absorptionFactor = 0.67;
                    greenhouseFactor = 0;
                }
                else if (worldType == "Standard (Ammonia)" || worldType == "Large (Ammonia)")
                {
                    absorptionFactor = 0.84;
                    greenhouseFactor = 0.2;
                }
                else if (worldType == "Standard (Ice)" || worldType == "Large (Ice)")
                {
                    absorptionFactor = 0.86;
                    greenhouseFactor = 0.2;
                }
                else if (worldType == "Standard (Greenhouse)" || worldType == "Large (Greenhouse)")
                {
                    absorptionFactor = 0.77;
                    greenhouseFactor = 2;
                }
                else if (worldType == "Standard (Cthonian)" || worldType == "Large (Cthonian)")
                {
                    absorptionFactor = 0.97;
                    greenhouseFactor = 0;
                }
                else
                {
                    absorptionFactor = -0.00000333333 * Math.Pow(hydroCoverage, 2) - 0.000922189 * hydroCoverage + 0.958623;
                    greenhouseFactor = 0.16;
                }

                double blackbodyCorrection = absorptionFactor * (1 + (atmoMass * greenhouseFactor));
                avgTemp = blackbodyCorrection * blackbodyTemp;

                if (avgTemp < 244)
                {
                    climate = "Frozen";
                }
                else if (avgTemp < 255)
                {
                    climate = "Very Cold";
                }
                else if (avgTemp < 266)
                {
                    climate = "Cold";
                }
                else if (avgTemp < 278)
                {
                    climate = "Chilly";
                }
                else if (avgTemp < 289)
                {
                    climate = "Cool";
                }
                else if (avgTemp < 300)
                {
                    climate = "Normal";
                }
                else if (avgTemp < 311)
                {
                    climate = "Warm";
                }
                else if (avgTemp < 322)
                {
                    climate = "Tropical";
                }
                else if (avgTemp < 333)
                {
                    climate = "Hot";
                }
                else if (avgTemp < 344)
                {
                    climate = "Very Hot";
                }
                else
                {
                    climate = "Infernal";
                }
            }
            outputElement.Add(new XAttribute("avgTemp", avgTemp));
            outputElement.Add(new XAttribute("climate", climate));

            //Calculate physical attributes
            rand = random.NextDouble();
            switch (worldType)
            {
                case "Tiny (Ice)":
                case "Tiny (Sulfur)":
                case "Small (Hadean)":
                case "Small (Ice)":
                case "Standard (Hadean)":
                case "Standard (Ammonia)":
                case "Large (Ammonia)":
                    density = 0.372965 * Math.Pow(rand, 2) - 0.0543322 * rand + 0.308775;
                    break;
                case "Tiny (Rock)":
                case "Small (Rock)":
                    density = 0.329014 * Math.Pow(rand, 2) + 0.0118921 * rand + 0.59949;
                    break;
                default:
                    density = 0.329014 * Math.Pow(rand, 2) + 0.0118921 * rand + 0.79949;
                    break;
            }
            outputElement.Add(new XAttribute("density", density));
            double minSizeConstraint, maxSizeConstraint;
            if (worldType.StartsWith("Large"))
            {
                minSizeConstraint = 0.065;
                maxSizeConstraint = 0.091;
            }
            else if (worldType.StartsWith("Standard"))
            {
                minSizeConstraint = 0.03;
                maxSizeConstraint = 0.065;
            }
            else if (worldType.StartsWith("Small"))
            {
                minSizeConstraint = 0.024;
                maxSizeConstraint = 0.03;
            }
            else
            {
                minSizeConstraint = 0.004;
                maxSizeConstraint = 0.024;
            }
            minSizeConstraint *= Math.Sqrt(blackbodyTemp / density);
            maxSizeConstraint *= Math.Sqrt(blackbodyTemp / density);
            rand = (5 * random.NextDouble()) + (5 * random.NextDouble());
            rand *= (maxSizeConstraint - minSizeConstraint) / 10;
            diameter = minSizeConstraint + rand;
            outputElement.Add(new XAttribute("diameter", diameter));
            surfaceGravity = density * diameter;
            outputElement.Add(new XAttribute("surfaceGravity", surfaceGravity));
            mass = density * Math.Pow(diameter, 3);
            outputElement.Add(new XAttribute("mass", mass));

            //Finalize atmosphere
            if (atmoType != "Trace")
            {
                double pressureFactor;

                switch (worldType)
                {
                    case "Small (Ice)":
                        pressureFactor = 10;
                        break;
                    case "Standard (Greenhouse)":
                        pressureFactor = 100;
                        break;
                    case "Large(Greenhouse)":
                        pressureFactor = 500;
                        break;
                    default:
                        if (sizeClass == "standard")
                        {
                            pressureFactor = 1;
                        }
                        else if (sizeClass == "large")
                        {
                            pressureFactor = 5;
                        }
                        else
                        {
                            throw new Exception("No pressureFactor could be assigned!");
                        }
                        break;
                }

                atmoPressure = atmoMass * pressureFactor * surfaceGravity;
            }
            else
            {
                atmoPressure = 0;
            }
            if (atmoPressure < 0.01)
            {
                atmoType = "Trace";
            }
            else if (atmoPressure < 0.5)
            {
                atmoType = "Very Thin";
            }
            else if (atmoPressure < 0.8)
            {
                atmoType = "Thin";
            }
            else if (atmoPressure < 1.2)
            {
                atmoType = "Standard";
            }
            else if (atmoPressure < 1.5)
            {
                atmoType = "Dense";
            }
            else if (atmoPressure < 10)
            {
                atmoType = "Very Dense";
            }
            else
            {
                atmoType = "Superdense";
            }
            outputElement.Add(new XAttribute("atmoPressure", atmoPressure));
            outputElement.Add(new XAttribute("atmoType", atmoType));

            if (atmoMarginals != null)
            {
                outputElement.Add(new XAttribute("atmoMarginals", atmoMarginals));
            }

            return outputElement;
        }
    }
}
