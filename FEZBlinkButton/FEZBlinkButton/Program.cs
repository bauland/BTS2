using System.Diagnostics;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using DeviceInformation = System.Runtime.InteropServices.DeviceInformation;

namespace FEZBlinkButton
{
    static class Program
    {
        private static GpioPin _motherboardLed;
        private static GpioPin _buttonLed;
        private static GpioPin _button;

        static void Main()
        {
            // Appel de la fonction de paramétrages des composants connectés
            Setup();

            // Boucle sans fin
            while (true)
            {
                Loop();
                Thread.Sleep(20);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static void Setup()
        {
            switch (DeviceInformation.DeviceName)
            {
                case "Cerb":
                    // Utilisation de la carte FEZ Cerberus
                    SetupBoard(FEZCerberus.GpioPin.DebugLed,FEZCerberus.GpioPin.Socket5.Pin3, FEZCerberus.GpioPin.Socket5.Pin4);
                    break;
                case "EMX":
                    // Utilisation de la carte FEZ Spider 1.0
                    SetupBoard(FEZSpider.GpioPin.DebugLed, FEZSpider.GpioPin.Socket5.Pin3, FEZSpider.GpioPin.Socket5.Pin4);
                    break;
                case "G120":
                    // Utilisation de la carte FEZ Spider 2.0
                    SetupBoard(FEZSpiderII.GpioPin.DebugLed, FEZSpiderII.GpioPin.Socket5.Pin3, FEZSpiderII.GpioPin.Socket5.Pin4);
                    break;
                case "FEZCLR":
                    // Utilisation de la carte Brainpad
                    SetupBoard(BrainPad.GpioPin.LightBulbBlue,BrainPad.GpioPin.ButtonLeft,BrainPad.GpioPin.LightBulbGreen);
                    break;
                default:
                    Debug.WriteLine("unkwon board: " + DeviceInformation.DeviceName);
                    break;
            }
        }

        private static void SetupBoard(int ledMotherBoardPin, int buttonPin, int ledButtonPin)
        {
            // Création de la broche _led pour piloter la DEL.
            // Utilisation de la DEL de la carte
            _motherboardLed = GpioController.GetDefault().OpenPin(ledMotherBoardPin);
            _motherboardLed.SetDriveMode(GpioPinDriveMode.Output);
            _motherboardLed.Write(GpioPinValue.High);

            _buttonLed = GpioController.GetDefault().OpenPin(ledButtonPin);
            _buttonLed.SetDriveMode(GpioPinDriveMode.Output);
            _buttonLed.Write(GpioPinValue.Low);

            // Création de la broche _button pour piloter le bouton.
            // Branchement du module bouton sur la connexion 3 sur Cerberus
            _button = GpioController.GetDefault().OpenPin(buttonPin);
            _button.SetDriveMode(GpioPinDriveMode.InputPullUp);
            // La fonction _button_ValueChanged sera appelée à chaque fois que la valeur de la broche du bouton change.
        }

        private static void Loop()
        {
            if(_button.Read()==GpioPinValue.High)
                _buttonLed.Write(GpioPinValue.Low);
            if(_button.Read()==GpioPinValue.Low)
                _buttonLed.Write(GpioPinValue.Low);
        }
    }
}
