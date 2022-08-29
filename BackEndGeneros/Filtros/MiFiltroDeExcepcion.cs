using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace BackEndGeneros.Filtros
{
    public class MiFiltroDeExcepcion : ExceptionFilterAttribute
    {
        private readonly ILogger<MiFiltroDeExcepcion> _logger;

        public MiFiltroDeExcepcion(ILogger<MiFiltroDeExcepcion> logger)
        {
            this._logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
