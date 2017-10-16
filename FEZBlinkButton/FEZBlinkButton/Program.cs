using System.Diagnostics;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using DeviceInformation = System.Runtime.InteropServices.DeviceInformation;

namespace FEZBlinkButton
{
    static class Program
    {
        private static GpioPin _led;
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
                    // Utilisation de la carte FEZ Cerberus
                    SetupBoard(FEZSpider.GpioPin.DebugLed, FEZSpider.GpioPin.Socket5.Pin3,FEZSpider.GpioPin.Socket5.Pin4);
                    break;
                default:
                    Debug.WriteLine("unkwon board: " + DeviceInformation.DeviceName);
                    break;
            }
        }

        private static void SetupBoard(int ledPin, int buttonPin, int ledButtonPin)
        {
            // Création de la broche _led pour piloter la DEL.
            // Utilisation de la DEL de la carte
            _led = GpioController.GetDefault().OpenPin(ledPin);
            _led.SetDriveMode(GpioPinDriveMode.Output);

            var buttonLed = GpioController.GetDefault().OpenPin(ledButtonPin);
            buttonLed.SetDriveMode(GpioPinDriveMode.Output);
            buttonLed.Write(GpioPinValue.Low);

            // Création de la broche _button pour piloter le bouton.
            // Branchement du module bouton sur la connexion 3 sur Cerberus
            _button = GpioController.GetDefault().OpenPin(buttonPin);
            _button.SetDriveMode(GpioPinDriveMode.InputPullUp);
            // La fonction _button_ValueChanged sera appelée à chaque fois que la valeur de la broche du bouton change.
            _button.ValueChanged += _button_ValueChanged;
        }

        private static void _button_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            // La fonction de clignotement est appelée à chaque fois que l'on appuie sur le bouton.
            if (e.Edge == GpioPinEdge.FallingEdge)
                BlinkLed();
        }

        private static void BlinkLed()
        {
            // On fait clignoter rapidement 4 fois la DEL.
            for (int i = 0; i < 4; i++)
            {
                _led.Write(GpioPinValue.High);
                Thread.Sleep(200);
                _led.Write(GpioPinValue.Low);
                Thread.Sleep(200);
            }
        }

        private static void Loop()
        {

        }
    }
}
