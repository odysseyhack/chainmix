using CheckCheque.ViewModels;
using CheckCheque.Views;
using ReactiveUI;
using Splat;

namespace CheckCheque
{
    public class AppBootStrapper : ReactiveObject
    {
        public RoutingState Router { get; protected set; }

        public AppBootStrapper()
        {
            Router = new RoutingState();
            Locator.CurrentMutable.Register(() => new AddInvoicesPage(), typeof(IViewFor<AddInvoicesPageViewModel>));
            Locator.CurrentMutable.Register(() => new InvoicesPage(), typeof(IViewFor<InvoicesViewModel>));
        }
    }
}
