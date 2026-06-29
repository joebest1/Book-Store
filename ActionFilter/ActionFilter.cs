using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class NotFoundFilter : IActionFilter
{
    private readonly ILogger<NotFoundFilter> _logger;

    public NotFoundFilter(ILogger<NotFoundFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult result && result.Value == null)
        {
            _logger.LogWarning("⚠️ Resource not found at {Path}", context.HttpContext.Request.Path);
            context.Result = new NotFoundObjectResult(new { message = "Resource not found" });
        }
    }
}
