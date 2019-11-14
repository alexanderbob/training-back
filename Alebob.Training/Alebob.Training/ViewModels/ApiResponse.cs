using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.ViewModels
{
    public class ApiResponse<T>
    {
        public T Payload { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }

        public ApiResponse(T data) 
        {
            Payload = data;
            Code = 1;
        }
    }
}
