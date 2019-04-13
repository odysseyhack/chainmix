using System;
using Xamarin.Forms;

namespace CheckCheque.Models
{
    public class Invoice : BindableObject
    {
        #region fields
        private string invoiceId;
        private ImageSource imageSource;
        private bool isValid;
        private bool isVerifying;
        private string invoiceName;
        private Color backgroundColor =  Color.Silver;
        private Color verificationColor = Color.Red;
        private string verificationStatus = "Invalid Invoice!";
        #endregion fields

        #region Properties and indexers
        public string InvoiceId 
        { 
            get
            {
                return this.invoiceId;
            }

            private set
            {
                if (this.invoiceId == value)
                {
                    return;
                }

                this.invoiceId = value;
                this.OnPropertyChanged();
            }
        }

        public ImageSource InvoiceImageSource 
        { 
            get
            {
                return this.imageSource;
            }

            private set
            {
                if (this.imageSource == value)
                {
                    return;
                }

                this.imageSource = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsValid 
        { 
            get
            {
                return this.isValid;
            }

            set
            {
                if (this.isValid == value)
                {
                    return;
                }

                this.isValid = value;
                this.OnPropertyChanged();
            } 
        }

        public bool IsVerifying
        {
            get
            {
                return this.isVerifying;
            }

            set
            {
                if (this.isVerifying == value)
                {
                    return;
                }

                this.isVerifying = value;
                this.OnPropertyChanged();
            }
        }

        public string InvoiceName
        {
            get
            {
                return this.invoiceName;
            }

            set
            {
                if (this.invoiceName == value)
                {
                    return;
                }

                this.invoiceName = value;
                this.OnPropertyChanged();
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }

            set
            {
                if (this.backgroundColor == value)
                {
                    return;
                }

                this.backgroundColor = value;
                this.OnPropertyChanged();
            }
        }

        public Color VerificationColor
        {
            get
            {
                return this.verificationColor;
            }

            set
            {
                if (this.verificationColor == value)
                {
                    return;
                }

                this.verificationColor = value;
                this.OnPropertyChanged();
            }
        }

        public string VerificationStatus
        {
            get
            {
                return this.verificationStatus;
            }

            set
            {
                if (this.verificationStatus == value)
                {
                    return;
                }

                this.verificationStatus = value;
                this.OnPropertyChanged();
            }

        }
        #endregion Properties and indexers

        public Invoice(ImageSource imageSource, string invoiceId, int invoiceNumber)
        {
            this.InvoiceImageSource = imageSource;
            this.InvoiceId = invoiceId;
            this.InvoiceName = "Invoice Number " + invoiceNumber;
            this.invoiceNumber = invoiceNumber;

            this.IsVerifying = true;

            Device.StartTimer(TimeSpan.FromSeconds(2), this.UpdateInvoiceVerificationStatus);
        }

        private int invoiceNumber;
        private bool UpdateInvoiceVerificationStatus()
        {
            this.IsVerifying = false;

            if (this.invoiceNumber == 2 || this.invoiceNumber == 5 || this.invoiceNumber == 8)
            {
                this.IsValid = false;
            }
            else
            {
                this.IsValid = true;
                this.BackgroundColor = Color.White;
                this.VerificationStatus = "VALID";
                this.VerificationColor = Color.Green;
            }

            return false;
        }
    }
}
