# RaspberryCar
## Inhalt
1. Idee
2. Lernphase
  Gpio-Controller
  PWM-Controller
  XBox Controller
3. Hardware-Bau

## Idee
Schon in der Arbeit mit Ole fasten wir zusammen den Entschluss ein RC-Model zu bauen und programmieren. So experimentierte ich mit verschiedenen Möglichkeiten, mit dem Raspberry Pi dies zu schaffen.  

## Lernphase
Zuerst fing ich mit Python an, wollte dann aber früh lieber eine andere Programmiersprache, die mehr an der Basis des letzendlichen Programms ist, benutzen. So benutzte versuchte ich erstmal, das ganze mit C++ zu erreichen. Da für eine solche Programmiersprache allerdings ein Compiler von nöten ist, ist die Variante mit Linux und C++ schnell wieder aus dem Fokus geraten. 
Ich entschied mich für den Compiler Visual Studio, da ich bereits mit diesem bekannt war und dieser ein großen Umfang im kostenlosen Rahmen hat.  
Da allerdings keine kostenlose Möglichkeit bestand über Linux einen Remotezugriff zu erhalten, bin ich dann auf Windows IoT Core als Betriebssystem und C# als Programmiersprache umgestiegen. Für den Wechsel auf C# habe ich mich entschieden, da Microsoft für diese Variante mehr Tutorials anbietet, als für C++. Da beide Programmiersprachen auf C basieren, ist der Unterschied auch nicht zu groß. 
### Gpio-Controller
Nach einigen Schwierigkeiten bei der Installaton von Windows 10 IoT Core, begann ich dann mit dem Erlernen der Gpio-Programmierung. Für das korekte Vorgehen beim Aufsetzten von Windows IoT Core siehe <a href="https://jaywee.github.io/Gertboard-Tutorial/#C#">Gertboard Tutorial</a>. 
