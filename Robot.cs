using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;



using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Sensors;


//using LegoRobot;

namespace LegoRobot
{
    public class Robot
    {

        private Motor motorLinks;
        private Motor motorRechts;
        //private MotorSync motorLR;
       


        private Motor motorArm;

        private EV3GyroSensor gyroSensor;


        private bool isAanHetRijden = false;

        private int hoekTeller;
        private bool vooruitRijden;

        public Robot(
            Motor imotorLinks, Motor imotorRechts,
            //MotorPort imotorLinks, MotorPort imotorRechts,
            Motor imotorArm, EV3GyroSensor igyroSensor)
        {
            motorLinks = imotorLinks;
            motorRechts = imotorRechts;
            motorArm = imotorArm;
            gyroSensor = igyroSensor;
            hoekTeller = GyroLezen();
            //motorLR = new MotorSync(imotorLinks, imotorRechts);
        }


        public void VoortBewegen(int tijd_ms, int snelheid, bool vooruit)
        // tijd_ms: Hoelang bewegen
        // snelheid: welk percentage snelheid bewegen
        // vooruit: Welke richting, True is vooruit, False is achteruit
        {
            // maakt variable aan dat gebruikt kan worden om te kijken of de robot recht rijd
            int beginGraad = hoekTeller; // lees gyro uit
            vooruitRijden = vooruit;


            // flipt de snelheid als hij achteruit moet rijden
            if (vooruitRijden == false)
            {
                snelheid = snelheid * -1;
            }

            // Snelheid een waarde maken waar de motors wat mee kunnen
            sbyte snelheidSB = Convert.ToSByte(snelheid);


            //functie voor gryo om de robot recht te houden
            void RechtHouden(int moterSnelheid)
            {
                int motorSnelheidLinks = moterSnelheid;
                int motorSnelheidRechts = moterSnelheid;
                  
                while (isAanHetRijden)
                {
                   
                    int huidigeHoek = GyroLezen();
                    LcdConsole.WriteLine($"{huidigeHoek}");
                    if (beginGraad != huidigeHoek)
                    {
                        if (snelheid > 0)
                        {
                            if (beginGraad < huidigeHoek)
                            {
                                if ((beginGraad + 5) < huidigeHoek)
                                {
                                    motorSnelheidLinks = moterSnelheid - 5;
                                    motorSnelheidRechts = moterSnelheid + 5;
                                    motorLinks.SetPower(Convert.ToSByte(motorSnelheidLinks));
                                    motorRechts.SetPower(Convert.ToSByte(motorSnelheidRechts));
                                }
                                else
                                {
                                    motorSnelheidLinks -= 1;
                                    motorSnelheidRechts += 1;
                                    motorLinks.SetPower(Convert.ToSByte(motorSnelheidLinks));
                                    motorRechts.SetPower(Convert.ToSByte(motorSnelheidRechts));
                                }

                            }
                            else
                            {
                                if ((beginGraad - 5) > huidigeHoek)
                                {
                                    motorSnelheidLinks = moterSnelheid + 5;
                                    motorSnelheidRechts = moterSnelheid - 5;
                                    motorLinks.SetPower(Convert.ToSByte(motorSnelheidLinks));
                                    motorRechts.SetPower(Convert.ToSByte(motorSnelheidRechts));
                                }
                                else
                                {
                                    motorSnelheidLinks += 1;
                                    motorSnelheidRechts -= 1;
                                    motorLinks.SetPower(Convert.ToSByte(motorSnelheidLinks));
                                    motorRechts.SetPower(Convert.ToSByte(motorSnelheidRechts));
                                }
                            }
                        } else
                        {
                            if (beginGraad < huidigeHoek)
                            {
                                    if ((beginGraad - 5) < huidigeHoek)
                                    {
                                        motorSnelheidLinks = moterSnelheid + 5;
                                        motorSnelheidRechts = moterSnelheid - 5;
                                        motorLinks.SetPower(Convert.ToSByte(motorSnelheidLinks));
                                        motorRechts.SetPower(Convert.ToSByte(motorSnelheidRechts));
                                    }
                                    else
                                    {
                                        motorSnelheidLinks += 1;
                                        motorSnelheidRechts -= 1;
                                        motorLinks.SetPower(Convert.ToSByte(motorSnelheidLinks));
                                        motorRechts.SetPower(Convert.ToSByte(motorSnelheidRechts));
                                    }
                                }
                            else
                            {
                                    if((beginGraad + 5) > huidigeHoek)
                                {
                                        motorSnelheidLinks = moterSnelheid - 5;
                                        motorSnelheidRechts = moterSnelheid + 5;
                                        motorLinks.SetPower(Convert.ToSByte(motorSnelheidLinks));
                                        motorRechts.SetPower(Convert.ToSByte(motorSnelheidRechts));
                                    } else
                                    {
                                        motorSnelheidLinks -= 1;
                                        motorSnelheidRechts += 1;
                                        motorLinks.SetPower(Convert.ToSByte(motorSnelheidLinks));
                                        motorRechts.SetPower(Convert.ToSByte(motorSnelheidRechts));
                                    }
                                }
                        }
                        Thread.Sleep(20);

                        

                    }


                }

                motorLinks.Brake();
                motorRechts.Brake();
                LcdConsole.WriteLine($"geremd");
                
            }

            //Gaat rijden
            isAanHetRijden = true;

            //maakt thread aan om tijdens het rijden recht te houden
            Thread rechtHoudenT = new Thread(() => RechtHouden(snelheid));

            // start thread om recht te houden
            rechtHoudenT.Start();

            // Print de snelheid
            LcdConsole.WriteLine($"Zet snelheid naar {snelheidSB}");

            // Zet motors aan en wacht voor een bepaalde tijd
            motorLinks.SetPower(snelheidSB);
            motorRechts.SetPower(snelheidSB);
            Thread.Sleep(tijd_ms);

            // Remd
            LcdConsole.WriteLine("Remmen");
            motorLinks.Brake();
            motorRechts.Brake();

            // Zet dit op false zodat de rechthouden weet dat ie kan stoppen
            isAanHetRijden = false;

            // brengt de threads samen
            rechtHoudenT.Join();

            Thread.Sleep(3000);


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

    }
}
