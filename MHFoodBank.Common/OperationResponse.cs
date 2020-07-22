using System;
using System.Collections.Generic;
using System.Text;

namespace MHFoodBank.Common
{
    public class OperationResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
