namespace AutoAtedimento.API.Exceptions
{

    public class ErroException : SystemException { }
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message){}
    }

    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message){}
    }


    
}
