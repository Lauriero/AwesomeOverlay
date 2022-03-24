using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Controls;

namespace AwesomeOverlay.Core.Utilities.UPN
{
    public static class UniversalPageNavigator
    {
        private static Dictionary<object, NavigatorRegistrantData> registrants = new Dictionary<object, NavigatorRegistrantData>();

        /// <summary> 
        /// Register new controller instance with name of property-container for selected page and returns registrant data for further operations
        /// </summary>
        /// <param name="controller">Controller instance</param>
        /// <param name="currentPagePropertyName">Name of property for selected page inside of the controller</param>
        public static NavigatorRegistrantData RegisterController(object controller, string currentPagePropertyName)
        {
            if (registrants.ContainsKey(controller)) {
                throw new Exception($"{controller} already registered");
            }

            PropertyInfo currentPageProperty = controller.GetType().GetProperty(currentPagePropertyName); 
            if (currentPageProperty == null) {
                throw new Exception($"{currentPagePropertyName} property not found on object {controller}"); 
            } else if (currentPageProperty.PropertyType != typeof(Page)) {
                throw new Exception($"{currentPagePropertyName} property type must be {typeof(Page)}");
            }

            NavigatorRegistrantData registrantData = new NavigatorRegistrantData(controller, currentPageProperty);
            registrants.Add(controller, registrantData);

            return registrantData;
        }
    }
}
