using Server.Repositories.EfCoreRepository;
using SharedLib.Models;
using System.Net;
using System.Text.Json;

var context = new EfRepository();
context.Database.EnsureCreated(); // creating Database on other localhost

HttpListener httpListener = new HttpListener();
var contextEf = new EfRepository();
const int port = 8080;

httpListener.Prefixes.Add($"http://*:{port}/");
httpListener.Start();
System.Console.WriteLine($"Server started on '{port}' port");

while (true)
{
    var contextHttp = await httpListener.GetContextAsync();
    var rawUrl = contextHttp.Request.RawUrl.Trim('/').ToLower();
    using var writer = new StreamWriter(contextHttp.Response.OutputStream);
    var rawItems = rawUrl.Split('/');

    if (rawItems.First() == "cars")
    {
        // GET
        if (contextHttp.Request.HttpMethod == HttpMethod.Get.Method)
        {
            contextHttp.Response.ContentType = "application/json";
            var result = contextEf.Cars.ToList();

            if (rawItems.Last() != "cars")
            {
                var name = rawItems.Last();

                var foundByNameUsers = result.Where(u => u.Name != null && u.Name.ToLower().Contains(name));

                if (foundByNameUsers != null && foundByNameUsers.Any())
                {
                    var foundByNameUsersHtml = JsonSerializer.Serialize(foundByNameUsers);

                    contextHttp.Response.StatusCode = 200;
                    await writer.WriteLineAsync(foundByNameUsersHtml.ToString());
                }
                else
                {
                    contextHttp.Response.StatusCode = 404;
                }
            }
            else
            {
                var usersHtml = JsonSerializer.Serialize(result);

                await writer.WriteLineAsync(usersHtml.ToString());
            }
        }
        else if (contextHttp.Request.HttpMethod == HttpMethod.Post.Method)
        {
            // POST
            contextHttp.Response.ContentType = "application/json";

            using (var reader = new StreamReader(contextHttp.Request.InputStream))
            {
                var requestBody = await reader.ReadToEndAsync();

                try
                {
                    var newCars = JsonSerializer.Deserialize<List<Car>>(requestBody);

                    foreach (var car in newCars)
                    {
                        contextEf.Cars.Add(car);
                    }

                    contextEf.SaveChanges();

                    contextHttp.Response.StatusCode = 200;
                    await writer.WriteLineAsync("POST request successful");
                }
                catch (JsonException)
                {
                    contextHttp.Response.StatusCode = 400; // Bad Request
                    await writer.WriteLineAsync("Invalid JSON format in the request body");
                }
                catch (Exception ex)
                {
                    contextHttp.Response.StatusCode = 500; // Internal Server Error
                    await writer.WriteLineAsync($"Error processing the request: {ex.Message}");
                }
            }
        }
        else if (contextHttp.Request.HttpMethod == HttpMethod.Delete.Method)
        {
            // DELETE 
            contextHttp.Response.ContentType = "application/json";

            var idToDelete = rawItems.Last(); // Assuming the last part of the URL is the ID to delete

            var carToDelete = contextEf.Cars.FirstOrDefault(c => c.Id.ToString() == idToDelete);

            if (carToDelete != null)
            {
                contextEf.Cars.Remove(carToDelete);
                contextEf.SaveChanges();
                contextHttp.Response.StatusCode = 200;
                await writer.WriteLineAsync("DELETE request successful");
            }
            else
            {
                contextHttp.Response.StatusCode = 404; // Not Found
                await writer.WriteLineAsync("Car not found for deletion");
            }
        }
    }
    else
    {
        contextHttp.Response.StatusCode = 404;
    }
}