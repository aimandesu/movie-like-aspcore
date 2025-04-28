using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class ResultResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public Error? Error { get; set; }

        // Static helper methods to create Result easily
        public static ResultResponse<T> Success(T data)
        {
            return new ResultResponse<T> { IsSuccess = true, Data = data };
        }

        public static ResultResponse<T> Fail(Error error)
        {
            return new ResultResponse<T> { IsSuccess = false, Error = error };
        }
    }

    public abstract class Error
    {
        public string Description { get; set; } = string.Empty;
    }

    public class NotFoundError : Error { 
        
    }
    public class ValidationError : Error { 
        
    }
    public class ServerError : Error { }
    public class ConflictError : Error { }

}