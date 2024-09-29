using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.IO;

using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Sensors;



namespace LegoRobot
{
    public class Robot
    {

        private Motor motorLinks;
        private Motor motorRechts;

        private int[] correctieMotorLR;

        private Motor motorArm;

        private EV3GyroSensor gyroSensor;


        private bool isAanHetRijden = false;
        private bool isAanHetDraaien = false;

        private int hoekTeller;

        public Robot(
            Motor imotorLinks, Motor imotorRechts, Motor imotorArm, 
            EV3GyroSensor igyroSensor,

            int[] icorrectieArray)
        {
            // motors 
            motorLinks = imotorLinks;
            motorRechts = imotorRechts;
            motorArm = imotorArm;
            // sensor
            gyroSensor = igyroSensor;
            hoekTeller = GyroLezen();
            //correctie moter
            correctieMotorLR = icorrectieArray;
        }


        public void VoortBewegen(int tijd_ms, int snelheid, bool vooruit = true)
        // tijd_ms: Hoelang bewegen
        // snelheid: welk percentage snelheid bewegen
        // vooruit: Welke richting, True is vooruit, False is achteruit
        {

            
            // maakt variable aan dat gebruikt kan worden om te kijken of de robot recht rijd
            int beginGraad = hoekTeller; // lees gyro uit


            int motorSnelheidLinks = snelheid + correctieMotorLR[0];
            int motorSnelheidRechts = snelheid + correctieMotorLR[1];



            // flipt de snelheid als hij achteruit moet rijden
            if (vooruit == false)
            {
                motorSnelheidLinks *=-1;
                motorSnelheidRechts *= -1;
            }

            // Snelheid een waarde maken waar de motors wat mee kunnen
            sbyte snelheidSB = Convert.ToSByte(snelheid);

            //-----------------

            //debug
            // Get the current directory of the application
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Define the file name
            string fileName = "output.txt";

            // Define the full path to the file
            string filePath = Path.Combine(currentDirectory, fileName);

            File.AppendAllText(filePath, $"de start angele is {beginGraad}  \n");


            //-----------------





            //functie voor gryo om de robot recht te houden
            void Rijden(int RH_motorSnelheidLinks, int RH_motorSnelheidRechts)
            {
                //start waardes motor
                int motorSnelheidLinksBegin = RH_motorSnelheidLinks;
                int motorSnelheidRechtsBegin = RH_motorSnelheidRechts;

                //variable om te kijken of richting flipt
                bool gewisseld = true;

                while (isAanHetRijden)
                {
                   

                    int huidigeHoek = GyroLezen();

                    //-----------------
                    // Debug
                    LcdConsole.WriteLine($"{huidigeHoek}");
                    File.AppendAllText(filePath, $"the huidige difference is (huid-begin) {huidigeHoek - beginGraad } , de rechtermoter doet: {RH_motorSnelheidRechts} , en de LINKER moter doet: {RH_motorSnelheidLinks} , naar links : {gewisseld} \n");
                    //-----------------

                    // Als niet recht door rijd
                    if (beginGraad != huidigeHoek)
                    {
                        // Naar rechts gedraaid
                        if (beginGraad < huidigeHoek)
                        {
                            // Als de robot van een correctie voor de andere kant af komt wordt dat hier op gevangen
                            if (gewisseld == true)
                            {
                                gewisseld = false;
                                RH_motorSnelheidLinks = motorSnelheidLinksBegin;
                                RH_motorSnelheidRechts = motorSnelheidRechtsBegin;
                                motorLinks.SetPower(Convert.ToSByte(motorSnelheidLinksBegin));
                                motorRechts.SetPower(Convert.ToSByte(motorSnelheidRechtsBegin));
                            }

                                
                            //Te hard aan het corrigeren
                            if ((motorSnelheidRechtsBegin + 20) < RH_motorSnelheidRechts) 
                            {
                                RH_motorSnelheidLinks = motorSnelheidLinksBegin - 20;
                                RH_motorSnelheidRechts = motorSnelheidRechtsBegin + 20;
                                motorLinks.SetPower(Convert.ToSByte(RH_motorSnelheidLinks));
                                motorRechts.SetPower(Convert.ToSByte(motorSnelheidRechtsBegin));
                            }

                            // zit nog binnen de marges
                            else
                            {
                                RH_motorSnelheidLinks -= 1;
                                RH_motorSnelheidRechts += 1;
                                motorLinks.SetPower(Convert.ToSByte(RH_motorSnelheidLinks));
                                motorRechts.SetPower(Convert.ToSByte(RH_motorSnelheidRechts));
                            }

                        }
                        // Naar links gedraaid
                        else if (beginGraad > huidigeHoek)
                        {
                            // Als de robot van een correctie voor de andere kant af komt wordt dat hier op gevangen
                            if (gewisseld == false)
                            {
                                gewisseld = true;
                                RH_motorSnelheidLinks = motorSnelheidLinksBegin;
                                RH_motorSnelheidRechts = motorSnelheidRechtsBegin;
                                motorLinks.SetPower(Convert.ToSByte(RH_motorSnelheidLinks));
                                motorRechts.SetPower(Convert.ToSByte(RH_motorSnelheidRechts));
                            }

                                

                            // corigeerd te hard
                            if ((motorSnelheidLinksBegin + 20) < RH_motorSnelheidLinks)
                            {
                                RH_motorSnelheidLinks = motorSnelheidLinksBegin + 20;
                                RH_motorSnelheidRechts = motorSnelheidRechtsBegin - 20;
                                motorLinks.SetPower(Convert.ToSByte(RH_motorSnelheidLinks));
                                motorRechts.SetPower(Convert.ToSByte(RH_motorSnelheidRechts));
                            }
                            // zit nog binnen de marges
                            else
                            {
                                RH_motorSnelheidLinks += 1;
                                RH_motorSnelheidRechts -= 1;
                                motorLinks.SetPower(Convert.ToSByte(RH_motorSnelheidLinks));
                                motorRechts.SetPower(Convert.ToSByte(RH_motorSnelheidRechts));
                            }
                        } 

                        Thread.Sleep(50);



                    }

                    // rijdt recht
                    else
                    {
                        RH_motorSnelheidLinks = motorSnelheidLinksBegin;
                        RH_motorSnelheidRechts = motorSnelheidRechtsBegin;
                        motorLinks.SetPower(Convert.ToSByte(motorSnelheidLinksBegin));
                        motorRechts.SetPower(Convert.ToSByte(motorSnelheidRechtsBegin));
                    }

                }

                // remt wanneer wacht process klaar is
                motorLinks.Brake();
                motorRechts.Brake();
                LcdConsole.WriteLine($"geremd");
                
            }

            //Gaat rijden
            isAanHetRijden = true;

            //maakt thread aan om tijdens het rijden recht te houden
            Thread rijdenT = new Thread(() => Rijden(motorSnelheidLinks, motorSnelheidRechts));

            // start thread om recht te houden
            rijdenT.Start();

            // werkt als timer
            Thread.Sleep(tijd_ms);

            // Zorgt dat rij process stopt
            isAanHetRijden = false;

            // brengt de threads samen
            rijdenT.Join();


        }

        public void Reizen(int tijd_ms, bool vooruit)
        // tijd_ms: Hoelang bewegen
        // vooruit: Welke richting, True is vooruit, False is achteruit
        {
            VoortBewegen(tijd_ms, 50, vooruit);
        }


        


        public void DraaienNaar(int richting, sbyte draaiSnelhed = 40)
        // Draait naar de richting met de gyro
        // int richting: kan negatief zijn & positief. Alles boven 359 wordt afgevangen
        // draaisnelheid: 20 tot 100 
        {

            //-----------------

            //debug
            // Get the current directory of the application
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Define the file name
            string fileName = "output1.txt";

            // Define the full path to the file
            string filePath = Path.Combine(currentDirectory, fileName);


            //-----------------

            //-----------------

            //debug
            // Get the current directory of the application
            string currentDirectory2 = AppDomain.CurrentDomain.BaseDirectory;

            // Define the file name
            string fileName2 = "output2.txt";

            // Define the full path to the file
            string filePath2 = Path.Combine(currentDirectory2, fileName2);


            //-----------------

            //-----------------
            // Debug
            File.AppendAllText(filePath, $"Hoi ik ben hier wel. Mijn hoek is {richting} \n");
            //-----------------






            richting = richting % 360; //Om hoeken boven 360 op te vangen

            bool klapOver360 = false; // Draait over de 360 heen want die angel is kleiner
            bool gyroGaatWisselen = false; // Gyro gaat over de nul heen tijdens het draaien en gaat van - naar + of + naar -
            bool eindRichtingIsNegatief;

            int negatieveRichting;
            int positieveRichting;

            int huidigeHoek = GyroLezen();

            //-----------------
            // Debug
            File.AppendAllText(filePath, $"De hoek waar we op beginnen is {huidigeHoek} \n");
            //-----------------

            TegenovergesteldeHoek(richting, out negatieveRichting, out positieveRichting);
           

            int verschilTussenHoeken;
            




            // negative waardes
            if (huidigeHoek < 0)
            {
                eindRichtingIsNegatief = true;


                verschilTussenHoeken = negatieveRichting - huidigeHoek;
                if (verschilTussenHoeken < 0)
                {
                    verschilTussenHoeken *= -1;
                }


                //-----------------
                // Debug
                File.AppendAllText(filePath, $"Het verschil is : {verschilTussenHoeken} \n");
                //-----------------


                // als het veschil hoger is dan 180 zet dan klapover op true want dan gaat de robot over 360 / 0 heen
                if (verschilTussenHoeken > 180)
                {
                    klapOver360 = true;
                }



                // als de huidige hoek kleiner is dan de richting en we gaan over 360 / 0 heen gaat de gryro flippen
                // voorbeeld: van -20 naar -340 gaat de gyro lezen als -20 naar 20
                if ((huidigeHoek > negatieveRichting) && klapOver360)
                {
                    gyroGaatWisselen = true;
                    eindRichtingIsNegatief = false;
                }


            }
            else
            // postieve waardes
            {
                eindRichtingIsNegatief = false;
                verschilTussenHoeken = positieveRichting - huidigeHoek;
                if (verschilTussenHoeken < 0)
                {
                    verschilTussenHoeken *= -1;
                }

                //-----------------
                // Debug
                File.AppendAllText(filePath, $"Het verschil is : {verschilTussenHoeken} \n");
                //-----------------

                // als het veschil hoger is dan 180 zet dan klapover op true want dan gaat de robot over 360 / 0 heen
                if (verschilTussenHoeken > 180)
                {
                    klapOver360 = true;
                }

                // als de huidige hoek kleiner is dan de richting en we gaan over 360 / 0 heen gaat de gryro flippen
                // voorbeeld: van -20 naar -340 gaat de gyro lezen als -20 naar 20
                if ((huidigeHoek < positieveRichting) && klapOver360)
                {
                    gyroGaatWisselen = true;
                    eindRichtingIsNegatief = true;
                }
            }

            

            // De functie die daatwerkelijk het draaien doet met de motors. Doet dit met beide wielen
            void Draaien(int draaiRichting)
            {
                int echteSnelheid = draaiSnelhed;

                //-----------------
                // Debug
                File.AppendAllText(filePath, $"Ik draai naar {draaiRichting} , ik klap over 360 : {klapOver360} en gryo wisselt {gyroGaatWisselen} \n");
                //-----------------

                //-----------------
                // Debug
                File.AppendAllText(filePath, $"{huidigeHoek} P {positieveRichting} N {negatieveRichting} \n");
                //-----------------

                while (isAanHetDraaien)
                {
                    huidigeHoek = GyroLezen();

                    ////-----------------
                    //// Debug
                    //File.AppendAllText(filePath2, $"{huidigeHoek} \n");
                    ////-----------------


                    if (huidigeHoek < 0)
                    {
                        verschilTussenHoeken = negatieveRichting - huidigeHoek;
                        if (verschilTussenHoeken < 0)
                        {
                            verschilTussenHoeken *= -1;
                        }

                        // dicht bij doel gaat de snelheid door de helft
                        if (verschilTussenHoeken < 40)
                        {
                            echteSnelheid = 10;
                        } 
                        else if (verschilTussenHoeken < 60)
                        {
                            echteSnelheid = draaiSnelhed / 2;
                        }
                        else
                        {
                            echteSnelheid = draaiSnelhed;
                        }

                        // als het veschil hoger is dan 180 zet dan klapover op true want dan gaat de robot over 360 / 0 heen
                        if (verschilTussenHoeken > 180)
                        {
                            klapOver360 = true;
                        }
                        else
                        {
                            klapOver360 = false;
                        }


                    }
                    else
                    // postieve waardes
                    {
                        verschilTussenHoeken = positieveRichting - huidigeHoek;
                        if (verschilTussenHoeken < 0)
                        {
                            verschilTussenHoeken *= -1;
                        }

                        // dicht bij doel gaat de snelheid door de helft
                        if (verschilTussenHoeken < 20)
                        {
                            echteSnelheid = 2;
                        }

                        // als het veschil hoger is dan 180 zet dan klapover op true want dan gaat de robot over 360 / 0 heen
                        if (verschilTussenHoeken > 180)
                        {
                            klapOver360 = true;
                        }
                        else
                        {
                            klapOver360 = false;
                        }

                    }

                    if (draaiRichting != huidigeHoek)
                    {
                        // klappen over de 360 / 0
                        if (klapOver360)
                        {
                            // draai naar links
                            if (gyroGaatWisselen ? draaiRichting < huidigeHoek : draaiRichting > huidigeHoek)
                            {
                                motorLinks.SetPower(Convert.ToSByte(-echteSnelheid));
                                motorRechts.SetPower(Convert.ToSByte(echteSnelheid));
                            }
                            // draai naar Rcehts
                            else if (gyroGaatWisselen ? draaiRichting > huidigeHoek : draaiRichting < huidigeHoek)
                            {
                                motorLinks.SetPower(Convert.ToSByte(echteSnelheid));
                                motorRechts.SetPower(Convert.ToSByte(-echteSnelheid));
                            }
                            else
                            {
                                motorLinks.Brake();
                                motorRechts.Brake();
                            }
                        }
                        // Gaan niet over 360 / 0 heen
                        else
                        {
                            // draai naar links
                            if (draaiRichting < huidigeHoek)
                            {
                                motorLinks.SetPower(Convert.ToSByte(-echteSnelheid));
                                motorRechts.SetPower(Convert.ToSByte(echteSnelheid));
                            }
                            // draai naar Rcehts
                            else if (draaiRichting > huidigeHoek)
                            {
                                motorLinks.SetPower(Convert.ToSByte(echteSnelheid));
                                motorRechts.SetPower(Convert.ToSByte(-echteSnelheid));
                            }
                            else
                            {
                                motorLinks.Brake();
                                motorRechts.Brake();
                            }
                        }
                    }
                    else
                    {
                        motorLinks.Brake();
                        motorRechts.Brake();
                        isAanHetDraaien = false;

                    }

                    Thread.Sleep(1);

                }


                //-----------------
                // Debug
                File.AppendAllText(filePath2, $"Done \n");
                //-----------------





                huidigeHoek = GyroLezen();

                if (huidigeHoek != draaiRichting)
                {
                    //Draaien(draaiRichting);
                }
                else
                {
                    hoekTeller = huidigeHoek;
                }



            }



            if (eindRichtingIsNegatief)
            {
                isAanHetDraaien = true;
                Draaien(negatieveRichting);
                //-----------------
                // Debug
                File.AppendAllText(filePath, $"Ik draai met negatieve cijfers \n");
                //-----------------
            }
            else
            {
                isAanHetDraaien = true;
                Draaien(positieveRichting);
                //-----------------
                // Debug
                File.AppendAllText(filePath, $"Ik draai met postieve cijfers \n");
                //-----------------
            }


        }
        

        public void DraaienHoeveelheidGraden(int aantalGraden)
        // Draait een aantal graaden 
        // gebruikt DraaienNaar
        {
            int negHoekTeller;
            int posHoekTeller;

            TegenovergesteldeHoek(hoekTeller, out negHoekTeller, out posHoekTeller);




            int temp = hoekTeller + aantalGraden;





            if (((hoekTeller > 0) && (temp < 0))) 
            {
                temp = posHoekTeller + aantalGraden;
            } 
            
            else if ((hoekTeller < 0) && (temp > 0))
            {
                temp = negHoekTeller + aantalGraden;
            }

            int deNaarTeDraaienHoek = temp % 360;

            DraaienNaar(deNaarTeDraaienHoek);

            //LcdConsole.WriteLine($"{(hoekTeller + aantalGraden) % 360}");
            //hoekTeller = (hoekTeller + aantalGraden) % 360; //%360 om hoeken boven 360 op te vangen
            //DraaienNaar(hoekTeller); 

        }

        private int GyroLezen()
        // Bakent de Gyro af zodat die genoeg tijd heeft om te lezen
        {
            Thread.Sleep(4);
            return gyroSensor.Read();
        }


        private void TegenovergesteldeHoek(int richting , out int minRichting, out int plusRichting)
        // voor de Gyro
        // Geeft de gespiegelde waarde van de input gespiegeld op 0
        {


            // -360 + x == x als richting posietief is 
            // 360 + x == x als de richting negatief is 



            if ((richting > 0))
            {
                minRichting = -360 + richting;
                plusRichting = richting;
            }
            else if ((richting < 0))
            {
                minRichting = richting;
                plusRichting = 360 + richting;

            } else
            {
                minRichting = 0;
                plusRichting = 0;
            }
        }
    }
}
