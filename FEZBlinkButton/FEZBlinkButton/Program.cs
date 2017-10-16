using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;

namespace FEZBlinkButton
{
    class Program
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
        }

        private static void Setup()
        {
            // Utilisation de la carte FEZ Cerberus

            // Création de la broche _led pour piloter la DEL.
            _led = GpioController.GetDefault().OpenPin(FEZCerberus.GpioPin.DebugLed);
            _led.SetDriveMode(GpioPinDriveMode.Output);

            // Création de la broche _button pour piloter le bouton.
            _button = GpioController.GetDefault().OpenPin(FEZCerberus.GpioPin.Socket3.Pin3);
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
