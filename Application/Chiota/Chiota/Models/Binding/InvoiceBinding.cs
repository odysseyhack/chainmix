using System;
using Chiota.Models.Database.Base;
using Xamarin.Forms;

namespace Chiota.Models.Binding
{
    public class InvoiceBinding : BaseModel
    {
        #region Properties

        public string Name { get; }

        public string LastMessage { get; set; }

        public DateTime LastMessageDateTime { get; set; }

        public ImageSource ImageSource { get; set; }

        public bool IsValid { get; set; }
        #endregion

        #region Constructors

        public InvoiceBinding(Guid invoiceGuid, string lastMessage, DateTime lastMessageDateTime)
        {
            Name = invoiceGuid.ToString();
            LastMessage = lastMessage;
            LastMessageDateTime = lastMessageDateTime;
        }

        #endregion
    }
}
