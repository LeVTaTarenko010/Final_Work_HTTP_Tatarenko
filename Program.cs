using System.Net;
using System.Text;

Console.Title = "Итоговое задание - HTTP-сервер";
Console.SetWindowSize(80, 30);

while (true)
{
    var listener = new HttpListener();
    listener.Prefixes.Add("http://127.0.0.1:6789/");
    listener.Prefixes.Add("http://localhost:6789/");
    listener.Start();

    Console.WriteLine("Ожидание подключений...");
    HttpListenerContext context = listener.GetContext();
    HttpListenerRequest request = context.Request;
    Console.WriteLine($"Текст запроса: {request.RawUrl}");

    HttpListenerResponse response = context.Response;
    var responseStr = "";
    var fileName = "";

    switch (request.RawUrl)
    {
        case "/":
            fileName = "wwwroot/index.html";
            break;
        case "/seasons":
        case "/seasons/":
            fileName = "wwwroot/htdocs/seasons.html";
            break;
        case "/toys":
        case "/toys/":
            fileName = "wwwroot/htdocs/toys.html";
            break;
        case "/favicon.ico":
            // если иконка не будет найдена, то отдаем ошибку 404 и поток закрывается,
            // чиоб не было спама в консоли 
            response.StatusCode = 404;
            response.Close();
            listener.Stop();
            continue;
        default:
            fileName = "wwwroot/htdocs/error.html";
            break;
    } // switch-case

    // читаем файл
    if (File.Exists(fileName)) {
        responseStr = File.ReadAllText(fileName);
    } else {
        responseStr = "<h1>Файл не найден!</h1>";
    } // if

   
    if (fileName == "wwwroot/htdocs/toys.html") {
        List<string> toys = [
            "Кукла",
            "Машинка",
            "Мяч",
            "Кубики",
            "Робот"
        ]; // List

        var repl = "<li>" + string.Join("</li><li>", toys) + "</li>";
        var template = "{{ toys }}";

        while (responseStr.Contains(template)) {
            responseStr = responseStr.Replace(template, repl);
        } // while
    } // if

    // отправлчем ответ
    var buffer = Encoding.UTF8.GetBytes(responseStr);

    // указываем, что мы присылаем HTML-страницу в utf-8
    response.ContentType = "text/html; charset=utf-8";

    using Stream output = response.OutputStream;
    response.ContentLength64 = buffer.Length;
    output.Write(buffer, 0, buffer.Length);
    listener.Stop();
    Console.WriteLine();
} // while