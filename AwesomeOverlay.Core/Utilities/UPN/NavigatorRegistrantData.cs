using Prism.Commands;

using System;
using System.Reflection;
using System.Windows.Controls;

namespace AwesomeOverlay.Core.Utilities.UPN
{
    public class NavigatorRegistrantData
    {
        private object instance;
        private PropertyInfo instancePropertyInfo;
        private CommandPalette palette;

        /// <summary>
        /// Create new instance of NavigatorRegistrantData with registrant instance and property info for selected page
        /// </summary>
        public NavigatorRegistrantData(object instance, PropertyInfo instancePropertyInfo)
        {
            this.instance = instance;
            this.instancePropertyInfo = instancePropertyInfo;
            this.palette = new CommandPalette();
        }

        /// <summary>
        /// Adds page to navigation list and set page controller value to the DataContext of the page
        /// </summary>
        /// <param name="page">Instance of page</param>
        /// <returns></returns>
        public NavigatorRegistrantData AddNavigatedPage<PageControllerType>(Page page, out PageControllerType pageController)
            where PageControllerType : class
        {
            palette.AddCommand(new DelegateCommand(() => {
                instancePropertyInfo.SetValue(instance, page);
            }));

            if (page.DataContext == null) {
                throw new Exception($"{page} data context cannot be null");
            } else if (page.DataContext.GetType() != typeof(PageControllerType)) {
                throw new Exception($"Type of DataContext of the page {page} must be {typeof(PageControllerType)}");
            }

            pageController = page.DataContext as PageControllerType;
            return this;
        }

        /// <summary>
        /// Adds page to navigation list and set page controller value to the DataContext of the page
        /// </summary>
        /// <param name="page">Instance of page</param>
        /// <returns></returns>
        public NavigatorRegistrantData AddNavigatedPage<PageControllerType>(Page page, PageControllerType pageController)
            where PageControllerType : class
        {
            palette.AddCommand(new DelegateCommand(() => {
                instancePropertyInfo.SetValue(instance, page);
            }));

            page.DataContext = pageController;
            return this;
        }

        /// <summary>
        /// Returns command palette
        /// </summary>
        /// <returns></returns>
        public CommandPalette BuildPalette()
        {
            return palette;
        }
    }
}
