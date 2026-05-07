namespace AutoAtedimento.API.Exceptions
{
    public class ErrorOnValidationException : ErroException
    {

        public IList<string>? ErrosMessages { get; set; }

        public ErrorOnValidationException(IList<string> errosMessages)
        {
            ErrosMessages = errosMessages;

        }
    }
}
