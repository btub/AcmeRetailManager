using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMDesktopUI.EventModels;
using Caliburn.Micro;

namespace ARMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private SimpleContainer _container;
        private SalesViewModel _salesVM;

        public ShellViewModel(IEventAggregator events,SimpleContainer container, SalesViewModel salesVM)
        {
            _events = events;
            _container = container;
            _salesVM = salesVM;
            
            _events.Subscribe(this);
            ActivateItem(_container.GetInstance<LoginViewModel>());
        }

        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM);
        }
    }
}
