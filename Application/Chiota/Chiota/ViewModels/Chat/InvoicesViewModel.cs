using System;
using System.Collections.Generic;
using System.Windows.Input;
using Chiota.Exceptions;
using Chiota.Extensions;
using Chiota.Models.Binding;
using Chiota.ViewModels.Base;
using Chiota.Views.Chat;
using Chiota.Views.Contact;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace Chiota.ViewModels.Chat
{
    public class InvoicesViewModel : BaseViewModel
    {
        #region Attributes

        private const int InvoiceItemHeight = 72;

        private static List<InvoiceBinding> _invoiceList;

        private int _chatListHeight;

        private bool _isUpdating;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the list of <see cref="InvoiceBinding"/> of a company.
        /// </summary>
        public List<InvoiceBinding> InvoiceList
        {
            get => _invoiceList;
            set
            {
                _invoiceList = value;
                OnPropertyChanged(nameof(InvoiceList));
            }
        }

        public int ChatListHeight
        {
            get => _chatListHeight;
            set
            {
                _chatListHeight = value;
                OnPropertyChanged(nameof(ChatListHeight));
            }
        }

        #endregion

        #region Constructors

        public InvoicesViewModel()
        {
            _invoiceList = new List<InvoiceBinding>();
        }

        #endregion

        #region Init

        public override void Init(object data = null)
        {
            base.Init(data);

            UpdateView();
        }

        #endregion

        #region Reverse

        public override void Reverse(object data = null)
        {
            base.Reverse(data);

            UpdateView();
        }

        #endregion

        #region ViewIsAppearing

        protected override void ViewIsAppearing()
        {
            base.ViewIsAppearing();
        
            _isUpdating = true;
            Device.StartTimer(TimeSpan.FromSeconds(10), UpdateView);
        }

        #endregion

        #region ViewIsDisappearing

        protected override void ViewIsDisappearing()
        {
            base.ViewIsDisappearing();

            _isUpdating = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Init the view with the user data of the database and the contact requests by valid connection.
        /// </summary>
        private bool UpdateView()
        {
            return _isUpdating;
        }

        #endregion

        #region Commands

        #region Contacts

        //public ICommand ContactsCommand
        //{
        //    get
        //    {
        //        return new Command(async () =>
        //        {
        //            await PushAsync<ContactsView>();
        //        });
        //    }
        //}

        public ICommand ScanInvoiceCommand
        {
            get
            {
                return new Command(async () =>
                {
                    // show camera
                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        return;
                    }

                    MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                    });

                    if (file == null)
                    {
                        return;
                    }
                });
            }
        }

        #endregion

        #region Tap

        public ICommand TapCommand
        {
            get
            {
                return new Command(async (param) =>
                {
                    if (param is ChatBinding chat)
                    {
                        //Show the chat view.
                        await PushAsync<ChatView>(chat.Contact);
                        return;
                    }
                    else if (param is ContactBinding contact)
                    {
                        //Show the chat view, or a dialog for a contact request acceptation.
                        if (!contact.IsApproved)
                        {
                            await PushAsync<ContactRequestView>(contact.Contact);
                            return;
                        }
                    }

                    //Show an unknown exception.
                    await new UnknownException(new ExcInfo()).ShowAlertAsync();
                });
            }
        }

        #endregion

        #endregion
    }
}
