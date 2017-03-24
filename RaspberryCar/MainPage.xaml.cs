using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices;
using Windows.Devices.Gpio;
using Windows.Devices.Pwm;
using Microsoft.IoT.Lightning.Providers;
using Windows.Gaming.Input;


namespace RaspberryCar
{
    public sealed partial class MainPage : Page
    {
        private const int GPIOPINS = 0;
        private const int PWMPINS = 2;
        private const int SERVOPINS = 1;
        private static readonly int[] GPIOPin = { };
        private static readonly int[] PWMPin = { 22, 27 };
        private static readonly int[] SERVOPin = { 17 };
        private static readonly GpioPin[] gpiopin = new GpioPin[GPIOPINS];
        private static readonly PwmPin[] pwmpin = new PwmPin[PWMPINS];
        private static readonly PwmPin[] servopin = new PwmPin[SERVOPINS];
        private double reduction = 0.5;
        private double direction = 1;


        public MainPage()
        {
            this.InitializeComponent();
            InitGpio();
        }

        private async void InitGpio()
        {
            if (LightningProvider.IsLightningEnabled)
            {
                LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();
            }

            var gpio = await GpioController.GetDefaultAsync();

            var PWM = await PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider());
            var pwm = PWM[1];
            pwm.SetDesiredFrequency(50);

            if (gpio == null)
            {
                ErrorMessage.Text = "No GpioController Connected";
                return;
            }

            for (int i = 0; i <= GPIOPin.Length; i++)
            {
                gpiopin[i] = gpio.OpenPin(GPIOPin[i]);
                gpiopin[i].SetDriveMode(GpioPinDriveMode.Output);
                gpiopin[i].Write(GpioPinValue.Low);
            }

            for (int i = 0; i <= PWMPin.Length; i++)
            {
                pwmpin[i] = pwm.OpenPin(PWMPin[i]);
                pwmpin[i].SetActiveDutyCyclePercentage(0);
                pwmpin[i].Start();
            }

            for (int i = 0; i <= SERVOPin.Length; i++)
            {
                servopin[i] = pwm.OpenPin(SERVOPin[i]);
                servopin[i].SetActiveDutyCyclePercentage(0.075);
                servopin[i].Start();
            }
        }

        private void SliderTh_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (direction == 1)
            {
                pwmpin[0].SetActiveDutyCyclePercentage(e.NewValue * reduction);
            }
            else if (direction == 0)
            {
                pwmpin[1].SetActiveDutyCyclePercentage(e.NewValue * reduction);
            }
            else
            {
                ErrorMessage.Text = "Dircetion Unclear";
            }
        }

        private void SliderSt_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (e.NewValue >= -45 && e.NewValue <= 45)
            {
                servopin[0].SetActiveDutyCyclePercentage(0.075 + (0.025 * e.NewValue / 45));
            }
        }

        private void ResetSt_Click(object sender, RoutedEventArgs e)
        {
            servopin[0].SetActiveDutyCyclePercentage(0.075);
        }

        private void Direction_Toggled(object sender, RoutedEventArgs e)
        {
            direction = Convert.ToDouble(Direction.IsOn);
        }
    }
}
