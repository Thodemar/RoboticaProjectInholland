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
using MonoBrickFirmware.UserInput;



namespace LegoRobot
{
    public class Robot
    {

        private Motor motorLinks;
        private Motor motorRechts;

        private int[] correctieMotorLR;

        private Motor motorArm;


        private EV3UltrasonicSensor ultrasonicSensor;
        private EV3GyroSensor gyroSensor;


        private bool isAanHetRijden = false;

        private int hoekTeller;

        public Robot(
            Motor imotorLinks, Motor imotorRechts, Motor imotorArm, 
            EV3GyroSensor igyroSensor,
            EV3UltrasonicSensor iultrasonicSensor,
            int[] icorrectieArray)
        {
            // motors 
            motorLinks = imotorLinks;
            motorRechts = imotorRechts;
            motorArm = imotorArm;
            // sensor
            gyroSensor = igyroSensor;
            hoekTeller = GyroLezen();
            ultrasonicSensor = iultrasonicSensor;
            //correctie moter
            correctieMotorLR = icorrectieArray;
        }


        public void VoortBewegen(int tijd_ms, int snelheid, bool vooruit = true)
        // tijd_ms: Hoelang bewegen
        // snelheid: Int value tussen 0 en 100
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
                bool flipped = true;

                while (isAanHetRijden)
                {
                   

                    int huidigeHoek = GyroLezen();

                    //-----------------
                    // Debug
                    LcdConsole.WriteLine($"{huidigeHoek}");
                    File.AppendAllText(filePath, $"the huidige difference is (huid-begin) {huidigeHoek - beginGraad } , de rechtermoter doet: {RH_motorSnelheidRechts} , en de LINKER moter doet: {RH_motorSnelheidLinks} , naar links : {flipped} \n");
                    //-----------------

                    // Als niet recht door rijd
                    if (beginGraad != huidigeHoek)
                    {
                        // Naar rechts gedraaid
                        if (beginGraad < huidigeHoek)
                        {
                            // Als de robot van een correctie voor de andere kant af komt wordt dat hier op gevangen
                            if (flipped == true)
                            {
                                flipped = false;
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
                            if (flipped == false)
                            {
                                flipped = true;
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
            Thread rechtHoudenT = new Thread(() => Rijden(motorSnelheidLinks, motorSnelheidRechts));

            // start thread om recht te houden
            rechtHoudenT.Start();

            // werkt als timer
            Thread.Sleep(tijd_ms);

            // Zorgt dat rij process stopt
            isAanHetRijden = false;

            // brengt de threads samen
            rechtHoudenT.Join();


        }

        public void Reizen(int tijd_ms, bool vooruit)
        // tijd_ms: Hoelang bewegen
        // vooruit: Welke richting, True is vooruit, False is achteruit
        {
            VoortBewegen(tijd_ms, 50, vooruit);
        }

        public void DraaienNaar(int richting, sbyte draaiSnelhed = 40)
        // Draait naar de richting met de gyro
        {
            //void SLEEP()
            //{
            //    Thread.Sleep(2);
            //}


            richting = richting % 360; //Om hoeken boven 360 op te vangen
            hoekTeller = richting;

            Thread.Sleep(100);
            motorLinks.Brake();
            motorRechts.Brake();




            int startHoek = GyroLezen();
            //SLEEP();
            int huidigeHoek = startHoek;
            //bool naarRechts;
            while (richting != huidigeHoek)
            {
                //SLEEP();
                if (startHoek < richting) //Naar rechts
                {
                    while (richting > huidigeHoek)
                    {
                        //SLEEP();
                        LcdConsole.WriteLine($"Gyro sensor: {GyroLezen()}");
                        motorLinks.SetPower(draaiSnelhed);
                        motorRechts.SetPower((sbyte)(-draaiSnelhed));
                        //SLEEP();
                        huidigeHoek = GyroLezen();
                    }
                    motorLinks.Brake();
                    motorRechts.Brake();
                }
                else if (startHoek > richting) //Naar links
                {
                    while (richting < huidigeHoek)
                    {
                        //SLEEP();
                        LcdConsole.WriteLine($"Gyro sensor: {GyroLezen()}");
                        motorLinks.SetPower((sbyte)(-draaiSnelhed));
                        motorRechts.SetPower(draaiSnelhed);
                        //SLEEP();
                        huidigeHoek = GyroLezen();
                    }
                    motorLinks.Brake();
                    motorRechts.Brake();
                }
                else { }  
            }

        }

        public void DraaienHoeveelheidGraden(int aantalGraden)
        // Draait een aantal graaden 
        // gebruikt DraaienNaar
        {
            motorLinks.Brake();
            motorRechts.Brake();
            LcdConsole.WriteLine($"{(hoekTeller + aantalGraden) % 360}");
            hoekTeller = (hoekTeller + aantalGraden) % 360; //%360 om hoeken boven 360 op te vangen
            DraaienNaar(hoekTeller); 
        }

        private int GyroLezen()
        // Bakant de Gyro af zodat die genoeg tijd heeft om te lezen
        {
            Thread.Sleep(4);
            return gyroSensor.Read();
        }

        public void Armbewegen(sbyte snel, uint rampUp, uint cons, uint rampDown)
        {
            motorArm.SpeedProfile(snel, rampUp, cons, rampDown, true);
        }

        public void Ultrasonic()
        {
            int afstand = ultrasonicSensor.Read();
            LcdConsole.WriteLine($"{afstand}");

        }
      
        
    }
}
