﻿using Microsoft.AspNetCore.Mvc;

namespace WebStore.WebAPI.Controllers;

[ApiController]
[Route("api/console")]
public class ConsoleApiController : ControllerBase
{
    [HttpGet("clear")]
    public void Clear() => Console.Clear();
        
    [HttpGet("write({str})")]
    public void Write(string str) => Console.WriteLine(str);
}
