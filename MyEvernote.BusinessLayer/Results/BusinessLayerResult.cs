using MyEvernote.Entities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer.Results
{
    public class BusinessLayerResult<T> where T : class
    {
        public List<ErrorMessageObj> Errors { get; set; } // 2 tip liste ekleme yaparken KeyValuePair kullanılır.
        public T Result { get; set; }

        public BusinessLayerResult()
        {
            Errors = new List<ErrorMessageObj>();
        }

        // KeyValuePair metoduna ekleme yapak biraz uğraştırdığından dolayı aşağıdaki metodu kullanacağız
        public void AddError(ErrorMessageCode code , string message)
        {
            Errors.Add(new ErrorMessageObj() {Code = code, Message = message });
        }

    }
}
