using System;
using System.ComponentModel;
using CheckCheque.ViewModels;
using Plugin.Media;
using Xamarin.Forms;

namespace CheckCheque.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class InvoicesPage : ContentPage
    {
        #region Properties and indexers
        /// <summary>
        /// The <see cref="InvoicesViewModel"/> for this <see cref="View"/>.
        /// </summary>
        private InvoicesViewModel ViewModel;
        #endregion Properties and indexers

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoicesPage"/> class.
        /// </summary>
        public InvoicesPage()
        {
            InitializeComponent();

            this.ViewModel = new InvoicesViewModel();
            this.BindingContext = this.ViewModel;
        }
        #endregion Constructors

        #region Event handlers
        /// <summary>
        /// Gets triggered when the user taps on the "+"-icon to scan a new invoice.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that triggered the event.</param>
        /// <param name="e">Provides details on the event.</param>
        protected virtual async void Handle_AddInvoice_ClickedAsync(object sender, EventArgs e)
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

            this.ViewModel.CreateNewInvoice(imageSource);

            this.ListOfInvoices.ItemsSource = null;
            this.ListOfInvoices.ItemsSource = this.ViewModel.Invoices;
        }
        #endregion Event handlers

    }
}
