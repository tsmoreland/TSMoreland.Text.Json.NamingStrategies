//
// Copyright © 2022 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TSMoreland.Text.Json.NamingStrategies.SampleApi.Models;

namespace TSMoreland.Text.Json.NamingStrategies.SampleApi.Controllers;

[Route("api/animal_says")]
[ApiController]
[Tags("animal_says")]
public class AnimalSaysController : ControllerBase
{
    public sealed record AnimalSays(Animal AnimalType, string Says);

    [HttpGet("{animal}")]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    public IActionResult Speak(Animal animal)
    {
        return animal switch
        {
            Animal.Cat => Ok(new AnimalSays(animal, "meow")),
            Animal.Dog => Ok(new AnimalSays(animal, "woof")),
            Animal.Mouse => Ok(new AnimalSays(animal, "squeak")),
            Animal.REDFox => Ok(new AnimalSays(animal, "sqeee")),
            Animal.TimberWolf => Ok(new AnimalSays(animal, "OwwwOoOoO")),
            _ => BadRequest()
        };
    }

}
