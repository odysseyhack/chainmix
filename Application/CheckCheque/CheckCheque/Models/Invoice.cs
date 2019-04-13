using System;
using Xamarin.Forms;

namespace CheckCheque.Models
{
    public class Invoice
    {
        #region Properties and indexers
        public string InvoiceNumber { get; private set; }

        public ImageSource InvoiceImageSource { get; private set; }

        public bool IsValid { get; set; }
        #endregion Properties and indexers

        public Invoice(ImageSource imageSource, string invoiceNumber)
        {
            this.InvoiceImageSource = imageSource;
            this.InvoiceNumber = invoiceNumber;
        }
    }
}
