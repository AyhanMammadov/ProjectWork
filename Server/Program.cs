using Server.Models;
using Server.Repositories.EfCoreRepository;
using System.Net;
using System.Text.Json;

var context = new MyEfRepository();
context.Database.EnsureCreated(); // creating Database on other localhost

HttpListener httpListener = new HttpListener();
var contextEf = new EfRepository();
const int port = 8080;

httpListener.Prefixes.Add($"http://*:{port}/");
httpListener.Start();
System.Console.WriteLine($"Server started on '{port}' port");

while (true)
{
    var context = await httpListener.GetContextAsync();
    var rawUrl = context.Request.RawUrl.Trim('/').ToLower();
    using var writer = new StreamWriter(context.Response.OutputStream);
    var rawItems = rawUrl.Split('/');

    if (rawItems.First() == "cars")
    {
        // GET
        if (context.Request.HttpMethod == HttpMethod.Get.Method)
        {
            context.Response.ContentType = "application/json";
            var result = contextEf.Cars.ToList();

            if (rawItems.Last() != "cars")
            {
                var name = rawItems.Last();

                var foundByNameUsers = result.Where(u => u.Name != null && u.Name.ToLower().Contains(name));

                if (foundByNameUsers != null && foundByNameUsers.Any())
                {
                    var foundByNameUsersHtml = JsonSerializer.Serialize(foundByNameUsers);

                    context.Response.StatusCode = 200;
                    await writer.WriteLineAsync(foundByNameUsersHtml.ToString());
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            }
            else
            {
                var usersHtml = JsonSerializer.Serialize(result);

                await writer.WriteLineAsync(usersHtml.ToString());
            }
        }
        else if (context.Request.HttpMethod == HttpMethod.Post.Method)
        {
            // POST
            context.Response.ContentType = "application/json";

            using (var reader = new StreamReader(context.Request.InputStream))
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

                    context.Response.StatusCode = 200;
                    await writer.WriteLineAsync("POST request successful");
                }
                catch (JsonException)
                {
                    context.Response.StatusCode = 400; // Bad Request
                    await writer.WriteLineAsync("Invalid JSON format in the request body");
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500; // Internal Server Error
                    await writer.WriteLineAsync($"Error processing the request: {ex.Message}");
                }
            }
        }
        else if (context.Request.HttpMethod == HttpMethod.Delete.Method)
        {
            // DELETE 
            context.Response.ContentType = "application/json";

            var idToDelete = rawItems.Last(); // Assuming the last part of the URL is the ID to delete

            var carToDelete = contextEf.Cars.FirstOrDefault(c => c.Id.ToString() == idToDelete);

            if (carToDelete != null)
            {
                contextEf.Cars.Remove(carToDelete);
                contextEf.SaveChanges();
                context.Response.StatusCode = 200;
                await writer.WriteLineAsync("DELETE request successful");
            }
            else
            {
                context.Response.StatusCode = 404; // Not Found
                await writer.WriteLineAsync("Car not found for deletion");
            }
        }
    }
    else
    {
        context.Response.StatusCode = 404;
    }
}