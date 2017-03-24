# RaspberryCar
## Inhalt
1. Idee
2. Lernphase
  Gpio-Controller
  PWM-Controller
  XBox Controller
  Netzwerk Programmierung
3. Hardware-Bau

## Idee
Schon in der Arbeit mit Ole fasten wir zusammen den Entschluss ein RC-Model zu bauen und programmieren. So experimentierte ich mit verschiedenen Möglichkeiten, mit dem Raspberry Pi dies zu schaffen.  
Nachdem sich unser Team auflöste, arbeitete ich dann selber weiter an diesem Projekt. 
Der letztendliche Plan ist es, ein Model-Auto zu bauen, das über WLAN kontrolliert werden kann. 
Dafür lerne ich erstmal mit Gpio-Controllern umzugehen, dann wie ich Motoren kontrolliere und zu letzt, wie man das ganze über WLAN kontrollieren kann.

## Lernphase
Zuerst fing ich mit Python an, wollte dann aber früh lieber eine andere Programmiersprache, die mehr an der Basis des letzendlichen Programms ist, benutzen. So benutzte versuchte ich erstmal, das ganze mit C++ zu erreichen. Da für eine solche Programmiersprache allerdings ein Compiler von nöten ist, ist die Variante mit Linux und C++ schnell wieder aus dem Fokus geraten. 
Ich entschied mich für den Compiler Visual Studio, da ich bereits mit diesem bekannt war und dieser ein großen Umfang im kostenlosen Rahmen hat.  
Da allerdings keine kostenlose Möglichkeit bestand über Linux einen Remotezugriff zu erhalten, bin ich dann auf Windows IoT Core als Betriebssystem und C# als Programmiersprache umgestiegen. Für den Wechsel auf C# habe ich mich entschieden, da Microsoft für diese Variante mehr Tutorials anbietet, als für C++. Da beide Programmiersprachen auf C basieren, ist der Unterschied auch nicht zu groß. 
### Gpio-Controller
Nach einigen Schwierigkeiten bei der Installaton von Windows 10 IoT Core, begann ich dann mit dem Erlernen der Gpio-Programmierung. Für das korekte Vorgehen beim Aufsetzten von Windows IoT Core siehe <a href="https://jaywee.github.io/Gertboard-Tutorial/#C#">Gertboard Tutorial</a>. 
### PWM-Controller
Um verschiedene Geschwindigkeiten mit einem Motor zu erreichen, muss das GPIO-Signal, dass nur an oder aus sein kann, auch Zwischenwerte erreichen. 
Dafür wird das sogenannte *Pulse Width Modulating* benutz. Dabei wird der Pin in kurzen Zeit ein- und wieder ausgeschaltet. Stadardmäßig wird dabei ein Zyklus von insgesammt 20ms benutzt. Wenn davon jetzt 2ms der Pin an ist und 18 aus, haben wir ein Prozentsatz von 10%, die übermittelt werden. Bei 18ms an und 2ms aus, wird ein Prozentsatz von 90% übermittelt. 
![PWM](https://github.com/JayWee/RaspberryCar/blob/master/Pictures/learn_raspberry_pi_how_pwm_works.jpg)  
Da Windows IoT Core das Hardware PWM, welches normalerweise besser funktioniert, als das, welches per Software erstellt wird, nicht nutzen kann, muss das ganze über Software PWM laufen. Der normale GPIO-Treiber unterstützt PWM allerdings nicht. Deshalb muss der im Dropdownmenü beim Unterpunkt "*Devices*" des Device Portals einzige weitere Treiber genutzt werden.
Zur weiteren Information und zum Lernen der Schritte, die noch zum Verwenden nötig sind kann man <a href="https://developer.microsoft.com/en-us/windows/iot/docs/lightningproviders">hier</a> nachlesen. 
Hier jetzt den Code zur Aktivieren von PWM: 
```
private async void InitGpio()
  {
    if (LightningProvider.IsLightningEnabled)
    {
      LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();
    }
            
    var PWM = await PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider());
    var pwm = PWM[1];
    pwm.SetDesiredFrequency(50);
  }
``` 
Mit der ersten Zeile, die mit *var* beginnt, wir der PWM-Controller auf eine kürzere Variable gesetzt. In der letzten Zeile wird dann die Zyklusfrequenz definiert. Hier eine Frequenz von 50Hz, also einer Zeit von 20ms pro Zyklus. 
Mit ``pwm.OpenPin(int)`` wird dann der zu benutzende Pin definiert. Die Variable des Pins muss vom Typ *PwmPin* sein.
Mit ``SetActiveDutyCyclePercentage()`` und dem Prozentsatz als *double*, mit dem der Pin an sein soll, in den Klammern als Befehl für die Pin Variable, kann die Eingeschaltete Zeit bestimmt werden.  
Letztendlich muss der DutyCycle noch gestartet werden. Dies geschieht mit dem Befehl ``Start()`` für die Pinvariable.

### XBox-Controller Input
Für das Benutzen des XBox-Controllers wird der Namespace ``Windows.Gaming.Input`` benötigt. Am Anfang muss dann eine Variable des Typs *Gampad* initialisiert werden. 
In der Methose, die ich benutzte, wird der Input über eine asynkrone *while*-Schleife gesammelt und nicht über EventHandler.  
Dabei muss diese zuerst über einen anderen EventHandler gestartet werden. 
```
private async void Start_Click(object sender, RoutedEventArgs e)
{
  Gamepad.GamepadAdded += Gamepad_GamepadAdded;
  Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;
  while (true)
  {
    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
    {
      if (Controller == null)
      {
        return;
      }
      var reading = Controller.GetCurrentReading();
               
      StInfo.Text = (reading.LeftThumbstickX * 45).ToString();

      ThInfo.Text = (reading.RightTrigger * 100).ToString();

      if ((reading.Buttons & GamepadButtons.A) == GamepadButtons.A)
      {
          BlockBt.Text = "Lights On";
      }
      else
      {
          BlockBt.Text = "Lights Off";
      }

    });
    await Task.Delay(TimeSpan.FromMilliseconds(5));
  }
}
```
Der Befehl ``await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>{});`` führt den gesamten Code, der in den geschweiften Klammern steht asynkron aus. 
Die If-Funktion überprüft, ob ein Controller verbunden ist. Die Variable *Controller* wird in den EventHandlern, die am Anfang des Codeausschnittes stehen, auf *null* gestzt, wenn kein Controller verbunden ist, und auf einen anderen Wert, wenn ein Controller verbunden ist. 
Die nach der IF-Funktion bestimmte *var* gibt ein fast ein Namespace zusammen, der auf den vom Controller eingehenden Input zugreift. 
Mit ``reading.`` und dem gewünschten Inputpart, können die Prozente der einzelnen Trigger des Controllers abgerufen werden. 
