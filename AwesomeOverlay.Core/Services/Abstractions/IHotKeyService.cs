
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AwesomeOverlay.Core.Services.Abstractions
{
    public interface IHotKeyService
    {
        /// <summary>
        /// Raise when main setting window was hidened or opened
        /// </summary>
        event Action MainWindowStateChanged;

        /// <summary>
        /// Intitalize the service
        /// </summary>
        Task InitAsync(IGlobalServiceAggregator serviceAggregator, CancellationToken token = default);
    }
}
