# HolidayUtil
Utilidad de calendario para obtener días festivos y días hábiles, seleccionar día hábil previo y próximo en c#

El codigo original es de https://gist.github.com/marlonramirez en java.

Puede utilizarse en el Net.core con dependency injection ya que tiene un constructor sin argumentos el cual inicia el cálculo de la utilidad con el año en curso

builder.Services.AddSingleton<HolidayUtil>();
