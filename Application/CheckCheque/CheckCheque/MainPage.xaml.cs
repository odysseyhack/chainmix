using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckCheque.Models;
using Plugin.Media;
using Xamarin.Forms;

namespace CheckCheque
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        private List<Invoice> invoices;

        private List<Invoice> Invoices
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

        public MainPage()
        {
            InitializeComponent();

            this.Invoices = new List<Invoice>();

            this.ListOfInvoices.ItemsSource = this.Invoices;
        }


        async void Handle_AddInvoice_ClickedAsync(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (file == null)
                return;

            await DisplayAlert("File Location", file.Path, "OK");

            ImageSource imageSource = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

            this.CreateNewInvoice(imageSource);
        }

        private void CreateNewInvoice(ImageSource imageSource)
        {
            if (imageSource == default(ImageSource))
            {
                throw new ArgumentNullException($"{nameof(imageSource)} cannot be null.");
            }

            Invoice invoice = new Invoice(imageSource, (new Guid()).ToString());

            this.Invoices.Add(invoice);
            this.OnPropertyChanged(nameof(this.Invoices));
        }
    }
}
