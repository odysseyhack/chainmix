using System;
using Xamarin.Forms;

namespace CheckCheque.Models
{
    public class Invoice
    {
        private ImageSource imageSource;
        private string invoiceNumber;
        private bool isValid;

        public Invoice(ImageSource imageSource, string invoiceNumber)
        {
            this.imageSource = imageSource;
            this.invoiceNumber = invoiceNumber;
        }
    }
}
