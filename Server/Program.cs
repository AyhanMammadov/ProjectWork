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
    contextHttp.Response.ContentType = "application/json";
    var rawUrl = contextHttp.Request.RawUrl.Trim('/').ToLower();
    using var writer = new StreamWriter(contextHttp.Response.OutputStream);
    var rawItems = rawUrl.Split('/');

    if (rawItems.First() == "cars")
    {
        // GET
        //mojno bilo contextHttp.Request.HttpMethod cherez switch case proverat metodi , no reshil ostavit takim obrazom
        if (contextHttp.Request.HttpMethod == HttpMethod.Get.Method)
        {
            var result = contextEf.Cars.ToList();

            if (rawItems.Last() != "cars")
            {
                var name = rawItems.Last();

                var foundByNameUsers = result.Where(u => u.Model != null && u.Model.ToLower().Contains(name));

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
            using (var reader = new StreamReader(contextHttp.Request.InputStream))
            {
                var requestBody = await reader.ReadToEndAsync();

                try
                {
                    var result = JsonSerializer.Deserialize<Car>(requestBody);
                    contextEf.Add(result);

                    contextEf.SaveChanges();

                    contextHttp.Response.StatusCode = 200;
                }
                catch (JsonException)
                {
                    contextHttp.Response.StatusCode = 400; // Bad Request
                }
                catch (Exception ex)
                {
                    contextHttp.Response.StatusCode = 500; // Internal Server Error
                }
            }
        }
        else if (contextHttp.Request.HttpMethod == HttpMethod.Delete.Method)
        {

            var idToDelete = rawItems.Last();

            var carToDelete = contextEf.Cars.FirstOrDefault(c => c.Id.ToString() == idToDelete);

            if (carToDelete != null)
            {
                contextEf.Cars.Remove(carToDelete);
                contextEf.SaveChanges();
                contextHttp.Response.StatusCode = 200;
            }
            else
            {
                contextHttp.Response.StatusCode = 404; // Not Found
            }
        }
        else if (contextHttp.Request.HttpMethod == HttpMethod.Put.Method)
        {
            // PUT
            using (var reader = new StreamReader(contextHttp.Request.InputStream))
            {
                var requestBody = await reader.ReadToEndAsync();
                try
                {
                    var updatedCar = JsonSerializer.Deserialize<Car>(requestBody);

                    int carIdToUpdate = updatedCar.Id;

                    var existingCar = contextEf.Cars.Where(c => c.Id == carIdToUpdate).FirstOrDefault();

                    if (existingCar != null)
                    {
                        existingCar.Description = updatedCar.Description;
                        existingCar.Model = updatedCar.Model;
                        existingCar.PathImage = updatedCar.PathImage;


                        contextEf.SaveChanges();

                        contextHttp.Response.StatusCode = 200;
                    }
                    else
                    {
                        contextHttp.Response.StatusCode = 404; // Not Found
                    }
                }
                catch (JsonException)
                {
                    contextHttp.Response.StatusCode = 400; // Bad Request
                    await writer.WriteLineAsync("Invalid JSON format");
                }
                catch (Exception ex)
                {
                    contextHttp.Response.StatusCode = 500; // Internal Server Error
                    await writer.WriteLineAsync($"Error: {ex.Message}");
                }
            }
        }
    }
    else
    {
        contextHttp.Response.StatusCode = 404;
    }
}