namespace TestWebApp
{
    using Microsoft.AspNetCore.Mvc;

    [Route("")]
    public class HomeController : ControllerBase
    {
        [Route("")]
        public string Index()
        {
            return "Hello, World!";
        }
    }
}
