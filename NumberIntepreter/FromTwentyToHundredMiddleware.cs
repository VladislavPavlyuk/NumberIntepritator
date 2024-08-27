﻿namespace NumberIntepreter
{
    public class FromTwentyToHundredMiddleware
    {
        private readonly RequestDelegate _next;

        public FromTwentyToHundredMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string? token = context.Request.Query["number"]; // Получим число из контекста запроса
            try
            {
                int number = Convert.ToInt32(token);

                number = Math.Abs(number);

                string[] Tens = { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number == 100)
                {
                    // Выдаем окончательный ответ клиенту
                    await context.Response.WriteAsync("Your number is one Hundred");
                } 
                if (number % 10 == 0 && number < 100)
                    {
                        // Выдаем окончательный ответ клиенту
                        await context.Response.WriteAsync("Your number Is " + Tens[number / 10 - 2]);
                    }
                if (number < 20) 
                {
                    await _next.Invoke(context); //Контекст запроса передаем следующему компоненту
                }
                else     // если 20 < number 
                {                  

                    await _next.Invoke(context); // Контекст запроса передаем следующему компоненту

                    string? result = context.Session.GetString("number"); // получим число от компонента FromOneToTenMiddleware

                    if (100 < number)   
                    {
                        while (100 < number) { number %= 100; }

                        if (20 < number) 
                            // Записываем в сессионную переменную number результат для компонента 
                            context.Session.SetString("number", Tens[number / 10 - 2] + " " + result);
                        else
                            await _next.Invoke(context);  //Контекст запроса передаем следующему компоненту
                    } else 
                        // Выдаем окончательный ответ клиенту
                         await context.Response.WriteAsync("Your number is " + Tens[number / 10 - 2] + " " + result);
                }                
            }
            catch (Exception)
            {
                // Выдаем окончательный ответ клиенту
                await context.Response.WriteAsync("Incorrect parameter on 20 ... 100");
            }
        }
    }
}
