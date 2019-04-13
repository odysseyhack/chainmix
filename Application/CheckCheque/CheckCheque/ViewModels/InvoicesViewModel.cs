using System;
using System.Collections.Generic;
using CheckCheque.Models;
using Xamarin.Forms;

namespace CheckCheque.ViewModels
{
    public class InvoicesViewModel : BindableObject
    {
        /// <summary>
        /// Backing field to the <see cref="Invoices"/> property.
        /// </summary>
        private List<Invoice> invoices;

        /// <summary>
        /// Gets or sets the list of <see cref="Invoice"/> objects.
        /// </summary>
        public List<Invoice> Invoices
        {
            get
            {
                return this.invoices;
            }
            set
            {
                if (this.invoices == value)
                {
                    return;
                }

                this.invoices = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoicesViewModel"/> class.
        /// </summary>
        public InvoicesViewModel()
        {
            this.Invoices = new List<Invoice>();
        }

        #region Private methods
        /// <summary>
        /// Creates a new <see cref="Invoice"/>-object from the given scanned image.
        /// </summary>
        /// <param name="imageSource">An <see cref="ImageSource"/> of the scanned image.</param>
        /// <param name="filePath">The actual path on the device to the image file.</param>
        public void CreateNewInvoice(ImageSource imageSource, string filePath)
        {
            if (imageSource == default(ImageSource))
            {
                throw new ArgumentNullException($"{nameof(imageSource)} cannot be null.");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException($"{nameof(filePath)} is either null or empty.");
            }

            Invoice invoice = new Invoice(imageSource, filePath, (Guid.NewGuid()).ToString(), this.Invoices.Count + 1);

            List<Invoice> newList = this.Invoices;
            newList.Add(invoice);

            this.Invoices = newList;

            this.OnPropertyChanged(nameof(this.Invoices));
        }
        #endregion Private methods

    }
}
